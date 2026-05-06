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
    public class FormatoEntregasController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;

        public FormatoEntregasController(GestionDocumentalContext context)
        {
            _context = context;
        }

        // GET: api/FormatoEntregas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormatoEntrega>>> GetFormatoEntrega()
        {
            return await _context.FormatoEntrega.ToListAsync();
        }

        // GET: api/FormatoEntregas/Activos
        [HttpGet("Activos")]
        public async Task<ActionResult<IEnumerable<FormatoEntrega>>> GetFormatoEntregaActivos()
        {
            var activos = await _context.FormatoEntrega
                .Where(f => f.Estado == "Activo")
                .ToListAsync();
            return Ok(activos);
        }

        // GET: api/FormatoEntregas/Desactivados
        [HttpGet("Desactivados")]
        public async Task<ActionResult<IEnumerable<FormatoEntrega>>> GetFormatoEntregaDesactivados()
        {
            var desactivados = await _context.FormatoEntrega
                .Where(f => f.Estado == "Desactivado")
                .ToListAsync();
            return Ok(desactivados);
        }

        // GET: api/FormatoEntregas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FormatoEntrega>> GetFormatoEntrega(int id)
        {
            var formatoEntrega = await _context.FormatoEntrega.FindAsync(id);

            if (formatoEntrega == null)
            {
                return NotFound();
            }

            return formatoEntrega;
        }

        // PUT: api/FormatoEntregas/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<object>> PutFormatoEntrega(string codigo, FormatoEntrega formatoEntrega)
        {
            // Buscar el formato de entrega por código
            var formatoExistente = await _context.FormatoEntrega.FirstOrDefaultAsync(f => f.CodigoFormato == codigo);
            
            if (formatoExistente == null)
            {
                return NotFound($"Formato de entrega con código '{codigo}' no encontrado.");
            }

            // Mostrar datos existentes antes de actualizar
            var datosActuales = new
            {
                Mensaje = "Datos actuales del registro que se va a actualizar",
                DatosAntesDeActualizar = new
                {
                    formatoExistente.FormatoId,
                    formatoExistente.CodigoFormato,
                    formatoExistente.NombreFormato,
                    formatoExistente.Estado
                }
            };

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (formatoEntrega.NombreFormato != null && formatoEntrega.NombreFormato != "string")
                formatoExistente.NombreFormato = formatoEntrega.NombreFormato;
            
            if (formatoEntrega.Estado != null && formatoEntrega.Estado != "string")
                formatoExistente.Estado = formatoEntrega.Estado;

            // Mantener el código original del parámetro
            formatoExistente.CodigoFormato = codigo;

            _context.Entry(formatoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormatoEntregaExists(formatoExistente.FormatoId))
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
                    formatoExistente.FormatoId,
                    formatoExistente.CodigoFormato,
                    formatoExistente.NombreFormato,
                    formatoExistente.Estado
                },
                CamposModificados = new
                {
                    NombreFormato = formatoEntrega.NombreFormato != null && formatoEntrega.NombreFormato != "string" ? formatoEntrega.NombreFormato : "No modificado",
                    Estado = formatoEntrega.Estado != null && formatoEntrega.Estado != "string" ? formatoEntrega.Estado : "No modificado"
                }
            });
        }

        // POST: api/FormatoEntregas
        [HttpPost]
        public async Task<ActionResult<FormatoEntrega>> PostFormatoEntrega([FromBody] FormatoEntregaCreateRequest request)
        {
            var formatoEntrega = new FormatoEntrega
            {
                CodigoFormato = request.CodigoFormato,
                NombreFormato = request.NombreFormato,
                Estado = request.Estado
            };

            _context.FormatoEntrega.Add(formatoEntrega);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormatoEntrega", new { id = formatoEntrega.FormatoId }, formatoEntrega);
        }

        // DELETE: api/FormatoEntregas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormatoEntrega(int id)
        {
            var formatoEntrega = await _context.FormatoEntrega.FindAsync(id);
            if (formatoEntrega == null)
            {
                return NotFound();
            }

            // Cambiar estado a Desactivado en lugar de eliminar
            formatoEntrega.Estado = "Desactivado";
            _context.Entry(formatoEntrega).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Formato de entrega desactivado exitosamente", Codigo = formatoEntrega.CodigoFormato, Estado = formatoEntrega.Estado });
        }

        private bool FormatoEntregaExists(int id)
        {
            return _context.FormatoEntrega.Any(e => e.FormatoId == id);
        }
    }
}
