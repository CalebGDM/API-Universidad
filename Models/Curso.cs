using System.ComponentModel.DataAnnotations;

namespace API_Universidad.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^[A-Z]{2}\d{3}$", ErrorMessage = "El código debe tener formato: 2 letras mayúsculas + 3 números (ej: CS101)")]
        public string Codigo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Los créditos son obligatorios")]
        [Range(1, 12, ErrorMessage = "Los créditos deben estar entre 1 y 12")]
        public int Creditos { get; set; }
        [Required(ErrorMessage = "El nombre del profesor es obligatorio")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "El nombre del profesor debe tener entre 5 y 150 caracteres")]
        public string Profesor { get; set; } = string.Empty;

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
