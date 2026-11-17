using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ActividadesAcademicas.Modelos;
using ActividadesAcademicas.Negocio;

namespace ActividadesAcademicas.Web.Presenters
{
    /// <summary>
    /// Presenter (MVP): coordina la accion de la vista, convierte los datos,
    /// llama al servicio de negocio y le dice a la vista que mostrar.
    /// No ejecuta SQL ni toca controles visuales.
    /// </summary>
    public class ActividadPresenter
    {
        private readonly IActividadView _view;
        private readonly IActividadService _service;

        public ActividadPresenter(IActividadView view, IActividadService service)
        {
            if (view == null) throw new ArgumentNullException("view");
            if (service == null) throw new ArgumentNullException("service");
            _view = view;
            _service = service;
        }

        public void Inicializar()
        {
            try
            {
                _view.MostrarTipos(_service.ObtenerTipos());
                Buscar();
            }
            catch (Exception)
            {
                _view.MostrarMensaje("No fue posible cargar la información inicial. Verifique la conexión a la base de datos.", false);
            }
        }

        /// <summary>Flujo de la guia: Guardar -> Presenter -> Negocio -> Datos -> SQL Server.</summary>
        public void Guardar()
        {
            var errores = new List<string>();
            var actividad = LeerActividadDeLaVista(errores);

            if (errores.Count > 0)
            {
                _view.MostrarErrores(errores);
                _view.MostrarMensaje("Corrija los datos indicados.", false);
                return;
            }

            ResultadoOperacion resultado = _service.Registrar(actividad);

            if (resultado.Exito)
            {
                _view.MostrarErrores(null);
                _view.MostrarMensaje(resultado.Mensaje, true);
                _view.LimpiarFormulario();
                Buscar();
            }
            else
            {
                _view.MostrarErrores(resultado.Errores);
                _view.MostrarMensaje(resultado.Mensaje, false);
            }
        }

        public void Buscar()
        {
            try
            {
                var filtro = new FiltroActividad
                {
                    Texto = _view.FiltroTexto,
                    TipoId = ParsearEntero(_view.FiltroTipoId),
                    Estado = string.IsNullOrWhiteSpace(_view.FiltroEstado) ? null : _view.FiltroEstado,
                    Fecha = ParsearFecha(_view.FiltroFecha)
                };
                _view.MostrarActividades(_service.Buscar(filtro));

                // Resumen general (sin filtros) para las tarjetas superiores
                var todas = _service.Buscar(new FiltroActividad());
                _view.MostrarResumen(
                    todas.Count,
                    todas.Sum(x => x.Inscritos),
                    todas.Count(x => x.Estado == "Proxima"),
                    todas.Count(x => x.Estado == "Finalizada"));
            }
            catch (Exception)
            {
                _view.MostrarMensaje("No fue posible consultar las actividades.", false);
            }
        }

        public void LimpiarFiltros()
        {
            _view.LimpiarFiltros();
            Buscar();
        }

        public void VerDetalle(int actividadId)
        {
            try
            {
                var a = _service.ObtenerPorId(actividadId);
                if (a == null)
                {
                    _view.MostrarMensaje("La actividad solicitada no existe.", false);
                    return;
                }
                _view.MostrarDetalle(a);
                _view.MostrarInscritos(_service.ObtenerInscritos(actividadId));
            }
            catch (Exception)
            {
                _view.MostrarMensaje("No fue posible consultar el detalle.", false);
            }
        }

        // ------------------------------------------------------------------
        private Actividad LeerActividadDeLaVista(List<string> errores)
        {
            var a = new Actividad
            {
                Titulo = _view.Titulo,
                Descripcion = _view.Descripcion,
                Modalidad = _view.Modalidad,
                Lugar = _view.Lugar,
                Estado = _view.Estado
            };

            int tipo;
            if (!string.IsNullOrWhiteSpace(_view.TipoId) && int.TryParse(_view.TipoId, out tipo)) a.TipoId = tipo;

            // Si el campo esta vacio se deja el valor por defecto y Negocio informa que es obligatorio.
            if (!string.IsNullOrWhiteSpace(_view.Cupo))
            {
                int cupo;
                if (int.TryParse(_view.Cupo.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out cupo)) a.Cupo = cupo;
                else errores.Add("El cupo debe ser un número entero mayor que cero.");
            }

            DateTime d;
            if (!string.IsNullOrWhiteSpace(_view.FechaInicio))
            {
                if (TryFecha(_view.FechaInicio, out d)) a.FechaInicio = d; else errores.Add("La fecha de inicio no tiene un formato válido.");
            }
            if (!string.IsNullOrWhiteSpace(_view.FechaFin))
            {
                if (TryFecha(_view.FechaFin, out d)) a.FechaFin = d; else errores.Add("La fecha de fin no tiene un formato válido.");
            }

            TimeSpan h;
            if (string.IsNullOrWhiteSpace(_view.HoraInicio)) errores.Add("La hora de inicio es obligatoria.");
            else if (TimeSpan.TryParse(_view.HoraInicio, CultureInfo.InvariantCulture, out h)) a.HoraInicio = h;
            else errores.Add("La hora de inicio no tiene un formato válido.");

            if (string.IsNullOrWhiteSpace(_view.HoraFin)) errores.Add("La hora de fin es obligatoria.");
            else if (TimeSpan.TryParse(_view.HoraFin, CultureInfo.InvariantCulture, out h)) a.HoraFin = h;
            else errores.Add("La hora de fin no tiene un formato válido.");

            return a;
        }

        private static bool TryFecha(string texto, out DateTime fecha)
        {
            return DateTime.TryParseExact(texto.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
        }

        private static DateTime? ParsearFecha(string texto)
        {
            DateTime f;
            return !string.IsNullOrWhiteSpace(texto) && TryFecha(texto, out f) ? (DateTime?)f : null;
        }

        private static int? ParsearEntero(string texto)
        {
            int n;
            return !string.IsNullOrWhiteSpace(texto) && int.TryParse(texto, out n) ? (int?)n : null;
        }
    }
}
