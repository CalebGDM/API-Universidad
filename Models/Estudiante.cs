using System.ComponentModel.DataAnnotations;

namespace API_Universidad.Models
{
    public class Estudiante
    {
       
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        public string Apellido { get; set; } = string.Empty;
        [Required(ErrorMessage = "El número de cuenta es obligatorio")]
        [RegularExpression(@"\d{8}$", ErrorMessage = "El número de cuenta debe tener  8 dígitos (ej: 00123456)")]
        public string NumCuenta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string Email { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }

        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
