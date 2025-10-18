namespace API_Universidad.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int CursoId { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public decimal Calificacion { get; set; }

        public Estudiante Estudiante { get; set; } = null!;
        public Curso Curso { get; set; } = null!;
    }
}
