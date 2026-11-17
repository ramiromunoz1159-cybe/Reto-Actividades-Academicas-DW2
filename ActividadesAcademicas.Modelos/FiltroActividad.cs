using System;

namespace ActividadesAcademicas.Modelos
{
    /// <summary>Criterios opcionales de busqueda (null = sin filtro).</summary>
    public class FiltroActividad
    {
        public string Texto { get; set; }
        public int? TipoId { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
