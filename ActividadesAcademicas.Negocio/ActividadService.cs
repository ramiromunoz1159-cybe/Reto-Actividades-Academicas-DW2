using System;
using System.Collections.Generic;
using System.Linq;
using ActividadesAcademicas.Datos;
using ActividadesAcademicas.Modelos;

namespace ActividadesAcademicas.Negocio
{
    /// <summary>
    /// Reglas de negocio y validaciones del servidor. Coordina al repositorio.
    /// No conoce controles visuales ni SQL.
    /// </summary>
    public class ActividadService : IActividadService
    {
        public static readonly string[] Modalidades = { "Presencial", "Virtual", "Hibrida" };
        public static readonly string[] Estados = { "Activa", "Proxima", "Finalizada", "Inactiva" };

        private readonly IActividadRepository _repo;

        public ActividadService(IActividadRepository repo)
        {
            if (repo == null) throw new ArgumentNullException("repo");
            _repo = repo;
        }

        public ResultadoOperacion Registrar(Actividad a)
        {
            var errores = Validar(a);
            if (errores.Count > 0)
                return ResultadoOperacion.Fallo("Corrija los datos indicados.", errores);

            try
            {
                if (!_repo.ExisteTipo(a.TipoId))
                    return ResultadoOperacion.Fallo("Corrija los datos indicados.",
                        new[] { "El tipo de actividad seleccionado no existe." });

                a.Titulo = a.Titulo.Trim();
                a.Descripcion = a.Descripcion.Trim();
                a.Lugar = a.Lugar.Trim();

                int id = _repo.Insertar(a);
                return ResultadoOperacion.Ok("Actividad registrada correctamente.", id);
            }
            catch (Exception)
            {
                // El detalle tecnico se registraria en un log; al usuario solo un mensaje controlado.
                return ResultadoOperacion.Fallo("No fue posible guardar la actividad. Intente de nuevo.", null);
            }
        }

        public Actividad ObtenerPorId(int id)
        {
            return id <= 0 ? null : _repo.ObtenerPorId(id);
        }

        public List<Actividad> Buscar(FiltroActividad filtro)
        {
            return _repo.Buscar(filtro);
        }

        public List<TipoActividad> ObtenerTipos()
        {
            return _repo.ObtenerTipos();
        }

        public List<Inscrito> ObtenerInscritos(int actividadId)
        {
            return actividadId <= 0 ? new List<Inscrito>() : _repo.ObtenerInscritos(actividadId);
        }

        /// <summary>Validaciones de campos y reglas. Se ejecuta SIEMPRE en el servidor.</summary>
        public static List<string> Validar(Actividad a)
        {
            var e = new List<string>();
            if (a == null) { e.Add("No se recibieron datos."); return e; }

            if (string.IsNullOrWhiteSpace(a.Titulo)) e.Add("El título es obligatorio.");
            else if (a.Titulo.Trim().Length > 150) e.Add("El título no puede superar 150 caracteres.");

            if (string.IsNullOrWhiteSpace(a.Descripcion)) e.Add("La descripción es obligatoria.");
            else if (a.Descripcion.Trim().Length > 500) e.Add("La descripción no puede superar 500 caracteres.");

            if (a.TipoId <= 0) e.Add("Seleccione el tipo de actividad.");

            if (a.FechaInicio == default(DateTime)) e.Add("La fecha de inicio es obligatoria.");
            if (a.FechaFin == default(DateTime)) e.Add("La fecha de fin es obligatoria.");
            if (a.FechaInicio != default(DateTime) && a.FechaFin != default(DateTime))
            {
                if (a.FechaFin.Date < a.FechaInicio.Date)
                    e.Add("La fecha de fin no puede ser anterior a la fecha de inicio.");
                else if (a.FechaFin.Date == a.FechaInicio.Date && a.HoraFin <= a.HoraInicio)
                    e.Add("La hora de fin debe ser posterior a la hora de inicio.");
            }

            if (!Modalidades.Contains(a.Modalidad)) e.Add("La modalidad no es válida.");

            if (string.IsNullOrWhiteSpace(a.Lugar)) e.Add("El lugar es obligatorio.");
            else if (a.Lugar.Trim().Length > 150) e.Add("El lugar no puede superar 150 caracteres.");

            if (a.Cupo <= 0) e.Add("El cupo debe ser un número entero mayor que cero.");
            else if (a.Cupo > 10000) e.Add("El cupo no puede superar 10000.");

            if (!Estados.Contains(a.Estado)) e.Add("El estado no es válido.");

            return e;
        }
    }
}
