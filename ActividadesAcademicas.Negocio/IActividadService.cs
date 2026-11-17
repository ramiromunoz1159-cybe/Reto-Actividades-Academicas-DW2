using System.Collections.Generic;
using ActividadesAcademicas.Modelos;

namespace ActividadesAcademicas.Negocio
{
    public interface IActividadService
    {
        ResultadoOperacion Registrar(Actividad actividad);
        Actividad ObtenerPorId(int id);
        List<Actividad> Buscar(FiltroActividad filtro);
        List<TipoActividad> ObtenerTipos();
        List<Inscrito> ObtenerInscritos(int actividadId);
    }
}
