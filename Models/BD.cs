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
}
