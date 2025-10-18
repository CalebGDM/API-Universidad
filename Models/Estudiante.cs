namespace API_Universidad.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NumCuenta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
