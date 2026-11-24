using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TallerescExtracurriculares.Models;

public static class BD
{
    public static string ConnectionString =
        "Server=localhost;DataBase=TalleresDB;Integrated Security=True;TrustServerCertificate=True;";

    public static List<Categoria> ObtenerCategorias()
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.Query<Categoria>(
                "SELECT Id, Nombre FROM Categorias"
            ).ToList();
        }
    }

    public static List<dynamic> ObtenerTalleres(int cantidadYaMostrada)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.Query(
                "SELECT t.Id, t.Nombre, t.Imagen, " +
                "c.Nombre AS NombreCategoria " +
                "FROM dbo.Talleres AS t " +
                "INNER JOIN dbo.Categorias AS c ON t.IdCategoria = c.Id " +
                "WHERE t.Activo = 1 " +
                "ORDER BY t.Id " +
                "OFFSET @pOffset ROWS FETCH NEXT 8 ROWS ONLY",
                new { pOffset = cantidadYaMostrada }
            ).ToList();
        }
    }

    public static bool HayMasTalleres(int cantidadYaMostrada)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            int totalTalleresActivos = connection.QuerySingle<int>(
                "SELECT COUNT(*) FROM dbo.Talleres WHERE Activo = 1"
            );

            return totalTalleresActivos > cantidadYaMostrada;
        }
    }

    public static dynamic? ObtenerTaller(int id)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstOrDefault<dynamic>(
                "SELECT t.Id, t.Nombre, t.Descripcion, t.Imagen, t.IdCategoria, t.Activo, " +
                "c.Nombre AS NombreCategoria " +
                "FROM dbo.Talleres AS t " +
                "INNER JOIN dbo.Categorias AS c ON t.IdCategoria = c.Id " +
                "WHERE t.Id = @pId",
                new { pId = id }
            );
        }
    }

    public static List<dynamic> ObtenerEncuentrosFuturos(int idTaller)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.Query(
                "SELECT Id, IdTaller, FechaHora, DuracionMinutos, Cupo " +
                "FROM dbo.Encuentros " +
                "WHERE IdTaller = @pIdTaller AND FechaHora > GETDATE() " +
                "ORDER BY FechaHora",
                new { pIdTaller = idTaller }
            ).ToList();
        }
    }

    public static int? ObtenerLugaresDisponibles(int idEncuentro)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstOrDefault<int?>(
                "SELECT e.Cupo - COUNT(i.Id) " +
                "FROM dbo.Encuentros AS e " +
                "LEFT JOIN dbo.Inscripciones AS i " +
                "ON i.IdEncuentro = e.Id AND i.Cancelado = 0 " +
                "WHERE e.Id = @pIdEncuentro " +
                "GROUP BY e.Cupo",
                new { pIdEncuentro = idEncuentro }
            );
        }
    }

    public static dynamic? ObtenerDatosEncuentro(int idEncuentro)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstOrDefault<dynamic>(
                "SELECT Id, IdTaller, Cupo " +
                "FROM dbo.Encuentros " +
                "WHERE Id = @pIdEncuentro",
                new { pIdEncuentro = idEncuentro }
            );
        }
    }

    public static int ContarInscripcionesActivas(int idEncuentro)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return ContarInscripcionesActivas(connection, null, idEncuentro);
        }
    }

    private static int ContarInscripcionesActivas(
        SqlConnection connection,
        SqlTransaction? transaction,
        int idEncuentro)
    {
        return connection.QuerySingle<int>(
            "SELECT COUNT(*) " +
            "FROM dbo.Inscripciones " +
            "WHERE IdEncuentro = @pIdEncuentro AND Cancelado = 0",
            new { pIdEncuentro = idEncuentro },
            transaction
        );
    }

    public static bool RegistrarInscripcion(int idEncuentro, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                dynamic? encuentro = connection.QueryFirstOrDefault<dynamic>(
                    "SELECT Id, Cupo " +
                    "FROM dbo.Encuentros WITH (UPDLOCK, HOLDLOCK) " +
                    "WHERE Id = @pIdEncuentro",
                    new { pIdEncuentro = idEncuentro },
                    transaction
                );

                if (encuentro is null ||
                    ContarInscripcionesActivas(connection, transaction, idEncuentro) >= (int)encuentro.Cupo)
                {
                    transaction.Commit();
                    return false;
                }

                connection.Execute(
                    "INSERT INTO dbo.Inscripciones " +
                    "(IdEncuentro, IdUsuario, FechaInscripcion, Cancelado, FechaCancelacion) " +
                    "VALUES (@pIdEncuentro, @pIdUsuario, GETDATE(), 0, NULL)",
                    new { pIdEncuentro = idEncuentro, pIdUsuario = idUsuario },
                    transaction
                );

                transaction.Commit();
                return true;
            }
        }
    }

    public static List<dynamic> ObtenerInscripciones(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.Query(
                "SELECT i.Id, i.IdEncuentro, e.IdTaller, i.IdUsuario, " +
                "i.FechaInscripcion, i.Cancelado, i.FechaCancelacion, " +
                "t.Nombre AS NombreTaller " +
                "FROM dbo.Inscripciones AS i " +
                "INNER JOIN dbo.Encuentros AS e ON i.IdEncuentro = e.Id " +
                "INNER JOIN dbo.Talleres AS t ON e.IdTaller = t.Id " +
                "WHERE i.IdUsuario = @pIdUsuario " +
                "ORDER BY i.FechaInscripcion DESC",
                new { pIdUsuario = idUsuario }
            ).ToList();
        }
    }

    public static bool ExisteNombreUsuario(string pNombreUsuario)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstOrDefault<int?>(
                "SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario = @pNombreUsuario",
                new { pNombreUsuario }
            ) == 1;
        }
    }

    public static void RegistrarUsuario(Usuario usuario)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Execute(
                "INSERT INTO dbo.Usuarios (NombreUsuario, Contrasena, Nombre, Apellido) " +
                "VALUES (@NombreUsuario, @Contrasena, @Nombre, @Apellido)",
                usuario
            );
        }
    }

    public static Usuario? Login(string pNombreUsuario, string pContrasena)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            return connection.QueryFirstOrDefault<Usuario>(
                "SELECT Id, NombreUsuario, Contrasena, Nombre, Apellido " +
                "FROM dbo.Usuarios " +
                "WHERE NombreUsuario = @pNombreUsuario AND Contrasena = @pContrasena",
                new { pNombreUsuario, pContrasena }
            );
        }
    }
}
