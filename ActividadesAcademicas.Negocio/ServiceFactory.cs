using ActividadesAcademicas.Datos;

namespace ActividadesAcademicas.Negocio
{
    /// <summary>
    /// Raiz de composicion: ensambla Negocio con Datos para que la capa Web
    /// solo dependa de Negocio (Web -> Negocio -> Datos).
    /// </summary>
    public static class ServiceFactory
    {
        public static IActividadService CrearActividadService(string connectionString)
        {
            return new ActividadService(new ActividadRepository(connectionString));
        }
    }
}
