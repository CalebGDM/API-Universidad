using Microsoft.EntityFrameworkCore;
using API1_pweb.Models;
using Microsoft.Identity.Client;

namespace API1_pweb.Data

{
    public class UniversidadContext : DbContext
    {
        public UniversidadContext(DbContextOptions<UniversidadContext> options) : base(options)
        {

        }
        public DbSet<Estudiante> Estudiantes { get; set; } = null!;
        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<Inscripcion> Inscripciones { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación Estudiante-Inscripcion
            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Estudiante)
                .WithMany(e => e.Inscripciones)
                .HasForeignKey(i => i.EstudianteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar relación Curso-Inscripcion
            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Curso)
                .WithMany(c => c.Inscripciones)
                .HasForeignKey(i => i.CursoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscripcion>()
               .Property(i => i.Calificacion)
               .HasPrecision(5, 2);
            // Índice único para matrícula
            modelBuilder.Entity<Estudiante>()
                .HasIndex(e => e.NumCuenta)
                .IsUnique();

            // Índice único para código de curso
            modelBuilder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();
        }
    }
}
