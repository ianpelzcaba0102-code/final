namespace TallerescExtracurriculares.Models;

public class Encuentro
{
    public int Id { get; set; }
    public int IdTaller { get; set; }
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; }
    public int Cupo { get; set; }
    public int LugaresDisponibles { get; set; }
}
