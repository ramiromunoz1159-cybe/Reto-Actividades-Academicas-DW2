using System.Collections.Generic;

namespace ActividadesAcademicas.Modelos
{
    /// <summary>Resultado comprensible que Negocio devuelve al Presenter.</summary>
    public class ResultadoOperacion
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public int IdGenerado { get; set; }
        public List<string> Errores { get; private set; }

        public ResultadoOperacion()
        {
            Errores = new List<string>();
        }

        public static ResultadoOperacion Ok(string mensaje, int id)
        {
            return new ResultadoOperacion { Exito = true, Mensaje = mensaje, IdGenerado = id };
        }

        public static ResultadoOperacion Fallo(string mensaje, IEnumerable<string> errores)
        {
            var r = new ResultadoOperacion { Exito = false, Mensaje = mensaje };
            if (errores != null) r.Errores.AddRange(errores);
            return r;
        }
    }
}
