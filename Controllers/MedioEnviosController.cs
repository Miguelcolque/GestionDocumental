using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDocumental.Data;
using GestionDocumental.Dominio;
using GestionDocumental.DTOs.CreateRequests;

namespace GestionDocumental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedioEnviosController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;

        public MedioEnviosController(GestionDocumentalContext context)
        {
            _context = context;
        }

        // GET: api/MedioEnvios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedioEnvio>>> GetMedioEnvio()
        {
            return await _context.MedioEnvio.ToListAsync();
        }

        // GET: api/MedioEnvios/Activos
        [HttpGet("Activos")]
        public async Task<ActionResult<IEnumerable<MedioEnvio>>> GetMedioEnvioActivos()
        {
            var activos = await _context.MedioEnvio
                .Where(m => m.Estado == "Activo")
                .ToListAsync();
            return Ok(activos);
        }

        // GET: api/MedioEnvios/Desactivados
        [HttpGet("Desactivados")]
        public async Task<ActionResult<IEnumerable<MedioEnvio>>> GetMedioEnvioDesactivados()
        {
            var desactivados = await _context.MedioEnvio
                .Where(m => m.Estado == "Desactivado")
                .ToListAsync();
            return Ok(desactivados);
        }

        // GET: api/MedioEnvios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedioEnvio>> GetMedioEnvio(int id)
        {
            var medioEnvio = await _context.MedioEnvio.FindAsync(id);

            if (medioEnvio == null)
            {
                return NotFound();
            }

            return medioEnvio;
        }

        // PUT: api/MedioEnvios/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<object>> PutMedioEnvio(string codigo, MedioEnvio medioEnvio)
        {
            // Buscar el medio de envío por código
            var medioExistente = await _context.MedioEnvio.FirstOrDefaultAsync(m => m.CodigoMedio == codigo);
            
            if (medioExistente == null)
            {
                return NotFound($"Medio de envío con código '{codigo}' no encontrado.");
            }

            // Mostrar datos existentes antes de actualizar
            var datosActuales = new
            {
                Mensaje = "Datos actuales del registro que se va a actualizar",
                DatosAntesDeActualizar = new
                {
                    medioExistente.MedioId,
                    medioExistente.CodigoMedio,
                    medioExistente.NombreMedio,
                    medioExistente.Estado
                }
            };

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (medioEnvio.NombreMedio != null && medioEnvio.NombreMedio != "string")
                medioExistente.NombreMedio = medioEnvio.NombreMedio;
            
            if (medioEnvio.Estado != null && medioEnvio.Estado != "string")
                medioExistente.Estado = medioEnvio.Estado;

            // Mantener el código original del parámetro
            medioExistente.CodigoMedio = codigo;

            _context.Entry(medioExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedioEnvioExists(medioExistente.MedioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar respuesta completa con datos antes y después
            return Ok(new
            {
                Mensaje = "Registro actualizado exitosamente",
                DatosAntes = datosActuales.DatosAntesDeActualizar,
                DatosDespues = new
                {
                    medioExistente.MedioId,
                    medioExistente.CodigoMedio,
                    medioExistente.NombreMedio,
                    medioExistente.Estado
                },
                CamposModificados = new
                {
                    NombreMedio = medioEnvio.NombreMedio != null && medioEnvio.NombreMedio != "string" ? medioEnvio.NombreMedio : "No modificado",
                    Estado = medioEnvio.Estado != null && medioEnvio.Estado != "string" ? medioEnvio.Estado : "No modificado"
                }
            });
        }

        // POST: api/MedioEnvios
        [HttpPost]
        public async Task<ActionResult<MedioEnvio>> PostMedioEnvio([FromBody] MedioEnvioCreateRequest request)
        {
            var medioEnvio = new MedioEnvio
            {
                CodigoMedio = request.CodigoMedio,
                NombreMedio = request.NombreMedio,
                Estado = request.Estado
            };

            _context.MedioEnvio.Add(medioEnvio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedioEnvio", new { id = medioEnvio.MedioId }, medioEnvio);
        }

        // DELETE: api/MedioEnvios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedioEnvio(int id)
        {
            var medioEnvio = await _context.MedioEnvio.FindAsync(id);
            if (medioEnvio == null)
            {
                return NotFound();
            }

            // Cambiar estado a Desactivado en lugar de eliminar
            medioEnvio.Estado = "Desactivado";
            _context.Entry(medioEnvio).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Medio de envío desactivado exitosamente", Codigo = medioEnvio.CodigoMedio, Estado = medioEnvio.Estado });
        }

        private bool MedioEnvioExists(int id)
        {
            return _context.MedioEnvio.Any(e => e.MedioId == id);
        }
    }
}
