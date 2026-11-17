using System;

namespace ActividadesAcademicas.Modelos
{
    /// <summary>DTO de la entidad Actividad. No contiene controles visuales.</summary>
    public class Actividad
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int TipoId { get; set; }
        public string TipoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Modalidad { get; set; }
        public string Lugar { get; set; }
        public int Cupo { get; set; }
        public string Estado { get; set; }
        public int Inscritos { get; set; }
    }

    /// <summary>Estudiante inscrito en una actividad (para mostrar en el detalle).</summary>
    public class Inscrito
    {
        public string Nombre { get; set; }
        public string Programa { get; set; }
        public DateTime FechaInscripcion { get; set; }
    }
}
