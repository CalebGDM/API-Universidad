using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Universidad.Data;
using API_Universidad.Models;

namespace API_Universidad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        private readonly UniversidadContext _context;

        public EstudiantesController(UniversidadContext context)
        {
            _context = context;
        }

        // GET: api/Estudiantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estudiante>>> GetEstudiantes()
        {
            return await _context.Estudiantes
                .Include(e => e.Inscripciones)
                .ThenInclude(i => i.Curso)
                .ToListAsync();
        }

        // GET: api/Estudiantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Estudiante>> GetEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Inscripciones)
                .ThenInclude(i => i.Curso)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (estudiante == null)
                return NotFound(new { mensaje = "Estudiante no encontrado" });

            return estudiante;
        }

        // POST: api/Estudiantes
        [HttpPost]
        public async Task<ActionResult<Estudiante>> PostEstudiante(Estudiante estudiante)
        {
            // Establecer una fecha automatica por defecto
            if (estudiante.FechaIngreso == default(DateTime))
            {
                estudiante.FechaIngreso = DateTime.Now;
            }

            if (estudiante.FechaIngreso > DateTime.Now)
            {
                return BadRequest(new { mensaje = "La fecha de ingreso no puede ser futura" });
            }

            // Verificar que el numero de cuenta no sea repetido 
            if (await _context.Estudiantes.AnyAsync(e => e.NumCuenta == estudiante.NumCuenta))
            {
                return BadRequest(new { mensaje = "Ya existe un estudiante con esa matrícula" });
            }

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEstudiante),
                new { id = estudiante.Id }, estudiante);
        }

        // PUT: api/Estudiantes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEstudiante(int id, Estudiante estudiante)
        {
            if (id != estudiante.Id)
                return BadRequest();

            _context.Entry(estudiante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Estudiantes.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Estudiantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
                return NotFound();

            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
