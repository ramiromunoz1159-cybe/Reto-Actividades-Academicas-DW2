using System.Collections.Generic;
using ActividadesAcademicas.Modelos;

namespace ActividadesAcademicas.Web.Presenters
{
    /// <summary>
    /// Contrato de la Vista (MVP). El Presenter solo conoce esta interfaz,
    /// nunca los controles de Web Forms.
    /// </summary>
    public interface IActividadView
    {
        // ---- Datos del formulario (texto crudo tal como lo escribio el usuario) ----
        string Titulo { get; }
        string Descripcion { get; }
        string TipoId { get; }
        string FechaInicio { get; }
        string FechaFin { get; }
        string HoraInicio { get; }
        string HoraFin { get; }
        string Modalidad { get; }
        string Lugar { get; }
        string Cupo { get; }
        string Estado { get; }

        // ---- Criterios de busqueda ----
        string FiltroTexto { get; }
        string FiltroTipoId { get; }
        string FiltroEstado { get; }
        string FiltroFecha { get; }

        // ---- Salidas hacia la vista ----
        void MostrarTipos(IEnumerable<TipoActividad> tipos);
        void MostrarActividades(IEnumerable<Actividad> actividades);
        void MostrarDetalle(Actividad actividad);
        void MostrarResumen(int total, int inscritos, int proximas, int finalizadas);
        void MostrarInscritos(IEnumerable<Inscrito> inscritos);
        void MostrarMensaje(string mensaje, bool esExito);
        void MostrarErrores(IEnumerable<string> errores);
        void LimpiarFormulario();
        void LimpiarFiltros();
    }
}
