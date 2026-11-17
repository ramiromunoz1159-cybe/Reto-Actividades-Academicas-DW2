using System.Collections.Generic;
using ActividadesAcademicas.Modelos;

namespace ActividadesAcademicas.Datos
{
    public interface IActividadRepository
    {
        int Insertar(Actividad actividad);
        Actividad ObtenerPorId(int id);
        List<Actividad> Buscar(FiltroActividad filtro);
        List<TipoActividad> ObtenerTipos();
        bool ExisteTipo(int tipoId);
        List<Inscrito> ObtenerInscritos(int actividadId);
    }
}
