namespace TallerescExtracurriculares.Models;

public class Inscripcion
{
    public int Id { get; set; }
    public int IdEncuentro { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public bool Cancelado { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public string NombreTaller { get; set; } = string.Empty;
}
