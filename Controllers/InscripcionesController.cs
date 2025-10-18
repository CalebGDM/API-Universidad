using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Universidad.Data;
using API_Universidad.Models;

namespace API_Universidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscripcionesController : ControllerBase
    {
        private readonly UniversidadContext _context;

        public InscripcionesController(UniversidadContext context)
        {
            _context = context;
        }

        // GET: api/Inscripciones
        // Obtener TODAS las inscripciones con estudiante y curso
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetInscripciones()
        {
            var inscripciones = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .Select(i => new
                {
                    i.Id,
                    i.EstudianteId,
                    EstudianteNombre = i.Estudiante.Nombre + " " + i.Estudiante.Apellido,
                    EstudianteMatricula = i.Estudiante.NumCuenta,
                    i.CursoId,
                    CursoNombre = i.Curso.Nombre,
                    CursoCodigo = i.Curso.Codigo,
                    i.FechaInscripcion,
                    i.Calificacion
                })
                .ToListAsync();

            return Ok(inscripciones);
        }

        // GET: api/Inscripciones/5
        // Obtener una inscripción específica
        [HttpGet("{id}")]
        public async Task<ActionResult<Inscripcion>> GetInscripcion(int id)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null)
                return NotFound(new { mensaje = "Inscripción no encontrada" });

            return Ok(new
            {
                inscripcion.Id,
                inscripcion.EstudianteId,
                EstudianteNombre = inscripcion.Estudiante.Nombre + " " + inscripcion.Estudiante.Apellido,
                inscripcion.CursoId,
                CursoNombre = inscripcion.Curso.Nombre,
                inscripcion.FechaInscripcion,
                inscripcion.Calificacion
            });
        }

        // GET: api/Inscripciones/Estudiante/5
        // Obtener todas las inscripciones de un estudiante
        [HttpGet("Estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetInscripcionesPorEstudiante(int estudianteId)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null)
                return NotFound(new { mensaje = "Estudiante no encontrado" });

            var inscripciones = await _context.Inscripciones
                .Include(i => i.Curso)
                .Where(i => i.EstudianteId == estudianteId)
                .Select(i => new
                {
                    i.Id,
                    i.CursoId,
                    CursoNombre = i.Curso.Nombre,
                    CursoCodigo = i.Curso.Codigo,
                    CursoCreditos = i.Curso.Creditos,
                    i.FechaInscripcion,
                    i.Calificacion,
                    Aprobado = i.Calificacion >= 70
                })
                .ToListAsync();

            return Ok(inscripciones);
        }

        // GET: api/Inscripciones/Curso/5
        // Obtener todos los estudiantes inscritos en un curso
        [HttpGet("Curso/{cursoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetInscripcionesPorCurso(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                return NotFound(new { mensaje = "Curso no encontrado" });

            var inscripciones = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Where(i => i.CursoId == cursoId)
                .Select(i => new
                {
                    i.Id,
                    i.EstudianteId,
                    EstudianteNombre = i.Estudiante.Nombre + " " + i.Estudiante.Apellido,
                    EstudianteMatricula = i.Estudiante.NumCuenta,
                    EstudianteEmail = i.Estudiante.Email,
                    i.FechaInscripcion,
                    i.Calificacion,
                    Aprobado = i.Calificacion >= 70
                })
                .ToListAsync();

            return Ok(inscripciones);
        }

        // POST: api/Inscripciones
        // Inscribir un estudiante a un curso
        [HttpPost]
        public async Task<ActionResult<Inscripcion>> PostInscripcion(Inscripcion inscripcion)
        {
            // Validar que el estudiante existe
            var estudiante = await _context.Estudiantes.FindAsync(inscripcion.EstudianteId);
            if (estudiante == null)
                return BadRequest(new { mensaje = "El estudiante no existe" });

            // Validar que el curso existe
            var curso = await _context.Cursos.FindAsync(inscripcion.CursoId);
            if (curso == null)
                return BadRequest(new { mensaje = "El curso no existe" });

            // Validar que no esté ya inscrito
            var yaInscrito = await _context.Inscripciones
                .AnyAsync(i => i.EstudianteId == inscripcion.EstudianteId &&
                              i.CursoId == inscripcion.CursoId);

            if (yaInscrito)
                return BadRequest(new { mensaje = "El estudiante ya está inscrito en este curso" });

            // Establecer fecha de inscripción
            inscripcion.FechaInscripcion = DateTime.Now;

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            // Recargar con datos relacionados
            await _context.Entry(inscripcion)
                .Reference(i => i.Estudiante)
                .LoadAsync();
            await _context.Entry(inscripcion)
                .Reference(i => i.Curso)
                .LoadAsync();

            return CreatedAtAction(nameof(GetInscripcion),
                new { id = inscripcion.Id },
                new
                {
                    inscripcion.Id,
                    inscripcion.EstudianteId,
                    EstudianteNombre = inscripcion.Estudiante.Nombre + " " + inscripcion.Estudiante.Apellido,
                    inscripcion.CursoId,
                    CursoNombre = inscripcion.Curso.Nombre,
                    inscripcion.FechaInscripcion,
                    inscripcion.Calificacion
                });
        }

        // PUT: api/Inscripciones/5
        // Actualizar toda la inscripción (calificación principalmente)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInscripcion(int id, Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
                return BadRequest(new { mensaje = "El ID no coincide" });

            var inscripcionExistente = await _context.Inscripciones.FindAsync(id);
            if (inscripcionExistente == null)
                return NotFound(new { mensaje = "Inscripción no encontrada" });

            // Solo actualizar la calificación (no permitir cambiar estudiante o curso)
            inscripcionExistente.Calificacion = inscripcion.Calificacion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InscripcionExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Inscripciones/5/calificacion
        // Actualizar SOLO la calificación (más común)
        [HttpPatch("{id}/calificacion")]
        public async Task<IActionResult> UpdateCalificacion(int id, [FromBody] decimal calificacion)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null)
                return NotFound(new { mensaje = "Inscripción no encontrada" });

            if (calificacion < 0 || calificacion > 100)
                return BadRequest(new { mensaje = "La calificación debe estar entre 0 y 100" });

            inscripcion.Calificacion = calificacion;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Calificación actualizada exitosamente",
                inscripcionId = inscripcion.Id,
                nuevaCalificacion = calificacion,
                aprobado = calificacion >= 70
            });
        }

        // DELETE: api/Inscripciones/5
        // Dar de baja a un estudiante de un curso
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Curso)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inscripcion == null)
                return NotFound(new { mensaje = "Inscripción no encontrada" });

            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estudiante dado de baja del curso exitosamente",
                estudiante = inscripcion.Estudiante.Nombre + " " + inscripcion.Estudiante.Apellido,
                curso = inscripcion.Curso.Nombre
            });
        }

        // GET: api/Inscripciones/Estadisticas/Curso/5
        // Estadísticas de un curso (promedio, aprobados, reprobados)
        [HttpGet("Estadisticas/Curso/{cursoId}")]
        public async Task<ActionResult<object>> GetEstadisticasCurso(int cursoId)
        {
            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
                return NotFound(new { mensaje = "Curso no encontrado" });

            var inscripciones = await _context.Inscripciones
                .Where(i => i.CursoId == cursoId)
                .ToListAsync();

            if (!inscripciones.Any())
                return Ok(new { mensaje = "No hay estudiantes inscritos en este curso" });

            var estadisticas = new
            {
                CursoNombre = curso.Nombre,
                TotalEstudiantes = inscripciones.Count,
                PromedioGeneral = Math.Round(inscripciones.Average(i => i.Calificacion), 2),
                Aprobados = inscripciones.Count(i => i.Calificacion >= 70),
                Reprobados = inscripciones.Count(i => i.Calificacion < 70),
                CalificacionMaxima = inscripciones.Max(i => i.Calificacion),
                CalificacionMinima = inscripciones.Min(i => i.Calificacion)
            };

            return Ok(estadisticas);
        }

        // GET: api/Inscripciones/Estadisticas/Estudiante/5
        // Estadísticas de un estudiante (promedio general, cursos aprobados)
        [HttpGet("Estadisticas/Estudiante/{estudianteId}")]
        public async Task<ActionResult<object>> GetEstadisticasEstudiante(int estudianteId)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null)
                return NotFound(new { mensaje = "Estudiante no encontrado" });

            var inscripciones = await _context.Inscripciones
                .Where(i => i.EstudianteId == estudianteId)
                .ToListAsync();

            if (!inscripciones.Any())
                return Ok(new { mensaje = "El estudiante no tiene inscripciones" });

            var estadisticas = new
            {
                EstudianteNombre = estudiante.Nombre + " " + estudiante.Apellido,
                EstudianteMatricula = estudiante.NumCuenta,
                TotalCursos = inscripciones.Count,
                PromedioGeneral = Math.Round(inscripciones.Average(i => i.Calificacion), 2),
                CursosAprobados = inscripciones.Count(i => i.Calificacion >= 70),
                CursosReprobados = inscripciones.Count(i => i.Calificacion < 70),
                MejorCalificacion = inscripciones.Max(i => i.Calificacion),
                PeorCalificacion = inscripciones.Min(i => i.Calificacion)
            };

            return Ok(estadisticas);
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(e => e.Id == id);
        }
    }
}
