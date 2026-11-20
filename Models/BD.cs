using Dapper;
using Microsoft.Data.SqlClient;

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
