using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Universidad.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del estudiante es obligatorio")]
        public int EstudianteId { get; set; }

        [Required(ErrorMessage = "El ID del curso es obligatorio")]
        public int CursoId { get; set; }
        public DateTime FechaInscripcion { get; set; }

        [Range(0, 10, ErrorMessage = "La calificación debe estar entre 0 y 10")]
        public decimal Calificacion { get; set; }

        [JsonIgnore] 
        public Estudiante? Estudiante { get; set; }

        [JsonIgnore]
        public Curso? Curso { get; set; }
    }
}
