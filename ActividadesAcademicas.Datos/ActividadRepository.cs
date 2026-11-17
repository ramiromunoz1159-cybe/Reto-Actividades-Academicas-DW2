using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ActividadesAcademicas.Modelos;

namespace ActividadesAcademicas.Datos
{
    /// <summary>
    /// Acceso a SQL Server con ADO.NET. TODAS las consultas son parametrizadas.
    /// No depende de ningun control visual ni de System.Web.
    /// </summary>
    public class ActividadRepository : IActividadRepository
    {
        private readonly string _connectionString;

        private const string ColumnasSelect =
            "a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre AS TipoNombre, " +
            "a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin, a.Modalidad, a.Lugar, a.Cupo, a.Estado, " +
            "(SELECT COUNT(*) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos ";

        public ActividadRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("La cadena de conexión no está configurada.", "connectionString");
            _connectionString = connectionString;
        }

        public int Insertar(Actividad a)
        {
            const string sql =
                "INSERT INTO dbo.Actividad " +
                "(Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupo, Estado) " +
                "VALUES (@Titulo, @Descripcion, @TipoId, @FechaInicio, @FechaFin, @HoraInicio, @HoraFin, @Modalidad, @Lugar, @Cupo, @Estado); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = a.Titulo;
                cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 500).Value = a.Descripcion;
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = a.TipoId;
                cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = a.FechaInicio.Date;
                cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = a.FechaFin.Date;
                cmd.Parameters.Add("@HoraInicio", SqlDbType.Time).Value = a.HoraInicio;
                cmd.Parameters.Add("@HoraFin", SqlDbType.Time).Value = a.HoraFin;
                cmd.Parameters.Add("@Modalidad", SqlDbType.NVarChar, 20).Value = a.Modalidad;
                cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value = a.Lugar;
                cmd.Parameters.Add("@Cupo", SqlDbType.Int).Value = a.Cupo;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = a.Estado;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public Actividad ObtenerPorId(int id)
        {
            const string sql =
                "SELECT " + ColumnasSelect +
                "FROM dbo.Actividad a INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId " +
                "WHERE a.ActividadId = @Id;";

            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    return rd.Read() ? Mapear(rd) : null;
                }
            }
        }

        public List<Actividad> Buscar(FiltroActividad f)
        {
            if (f == null) f = new FiltroActividad();

            const string sql =
                "SELECT " + ColumnasSelect +
                "FROM dbo.Actividad a INNER JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId " +
                "WHERE (@Texto IS NULL OR a.Titulo LIKE N'%' + @Texto + N'%') " +
                "  AND (@TipoId IS NULL OR a.TipoId = @TipoId) " +
                "  AND (@Estado IS NULL OR a.Estado = @Estado) " +
                "  AND (@Fecha  IS NULL OR @Fecha BETWEEN a.FechaInicio AND a.FechaFin) " +
                "ORDER BY a.FechaInicio DESC, a.ActividadId DESC;";

            var lista = new List<Actividad>();
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Texto", SqlDbType.NVarChar, 150).Value = ValorODbNull(f.Texto);
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = f.TipoId.HasValue ? (object)f.TipoId.Value : DBNull.Value;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = ValorODbNull(f.Estado);
                cmd.Parameters.Add("@Fecha", SqlDbType.Date).Value = f.Fecha.HasValue ? (object)f.Fecha.Value.Date : DBNull.Value;

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read()) lista.Add(Mapear(rd));
                }
            }
            return lista;
        }

        public List<TipoActividad> ObtenerTipos()
        {
            const string sql = "SELECT TipoId, Nombre FROM dbo.TipoActividad ORDER BY Nombre;";
            var lista = new List<TipoActividad>();
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new TipoActividad { TipoId = rd.GetInt32(0), Nombre = rd.GetString(1) });
                }
            }
            return lista;
        }

        public List<Inscrito> ObtenerInscritos(int actividadId)
        {
            const string sql =
                "SELECT u.Nombre, ISNULL(u.Programa, N'') AS Programa, i.FechaInscripcion " +
                "FROM dbo.Inscripcion i INNER JOIN dbo.Usuario u ON u.UsuarioId = i.UsuarioId " +
                "WHERE i.ActividadId = @ActividadId AND i.Estado = N'Activa' " +
                "ORDER BY i.FechaInscripcion, u.Nombre;";

            var lista = new List<Inscrito>();
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(new Inscrito
                        {
                            Nombre = rd.GetString(0),
                            Programa = rd.GetString(1),
                            FechaInscripcion = rd.GetDateTime(2)
                        });
                }
            }
            return lista;
        }

        public bool ExisteTipo(int tipoId)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.TipoActividad WHERE TipoId = @TipoId;";
            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = tipoId;
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private static object ValorODbNull(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim();
        }

        private static Actividad Mapear(IDataRecord r)
        {
            return new Actividad
            {
                ActividadId = (int)r["ActividadId"],
                Titulo = (string)r["Titulo"],
                Descripcion = (string)r["Descripcion"],
                TipoId = (int)r["TipoId"],
                TipoNombre = (string)r["TipoNombre"],
                FechaInicio = (DateTime)r["FechaInicio"],
                FechaFin = (DateTime)r["FechaFin"],
                HoraInicio = (TimeSpan)r["HoraInicio"],
                HoraFin = (TimeSpan)r["HoraFin"],
                Modalidad = (string)r["Modalidad"],
                Lugar = (string)r["Lugar"],
                Cupo = (int)r["Cupo"],
                Estado = (string)r["Estado"],
                Inscritos = (int)r["Inscritos"]
            };
        }
    }
}
