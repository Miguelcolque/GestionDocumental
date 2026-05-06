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
    public class EntregaDocumentoesController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;

        public EntregaDocumentoesController(GestionDocumentalContext context)
        {
            _context = context;
        }

        // GET: api/EntregaDocumentoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntregaDocumento>>> GetEntregaDocumento()
        {
            return await _context.EntregaDocumento.ToListAsync();
        }

        // GET: api/EntregaDocumentoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntregaDocumento>> GetEntregaDocumento(int id)
        {
            var entregaDocumento = await _context.EntregaDocumento.FindAsync(id);

            if (entregaDocumento == null)
            {
                return NotFound();
            }

            return entregaDocumento;
        }

        // PUT: api/EntregaDocumentoes/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<EntregaDocumento>> PutEntregaDocumento(string codigo, EntregaDocumento entregaDocumento)
        {
            // Buscar la entrega por código
            var entregaExistente = await _context.EntregaDocumento.FirstOrDefaultAsync(e => e.CodigoEntrega == codigo);
            
            if (entregaExistente == null)
            {
                return NotFound($"Entrega con código '{codigo}' no encontrada.");
            }

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (entregaDocumento.CodigoSolicitud != null && entregaDocumento.CodigoSolicitud != "string")
                entregaExistente.CodigoSolicitud = entregaDocumento.CodigoSolicitud;
            
            if (entregaDocumento.CodigoTipoDoc != null && entregaDocumento.CodigoTipoDoc != "string")
                entregaExistente.CodigoTipoDoc = entregaDocumento.CodigoTipoDoc;
            
            if (entregaDocumento.CodigoFormato != null && entregaDocumento.CodigoFormato != "string")
                entregaExistente.CodigoFormato = entregaDocumento.CodigoFormato;
            
            if (entregaDocumento.CodigoMedio != null && entregaDocumento.CodigoMedio != "string")
                entregaExistente.CodigoMedio = entregaDocumento.CodigoMedio;
            
            if (entregaDocumento.FechaEntrega != default && entregaDocumento.FechaEntrega > DateTime.MinValue)
                entregaExistente.FechaEntrega = entregaDocumento.FechaEntrega;
            
            if (entregaDocumento.Estado != null && entregaDocumento.Estado != "string")
                entregaExistente.Estado = entregaDocumento.Estado;

            // Mantener el código original del parámetro
            entregaExistente.CodigoEntrega = codigo;

            _context.Entry(entregaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntregaDocumentoExists(entregaExistente.EntregaId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar el registro actualizado
            return Ok(entregaExistente);
        }

        // POST: api/EntregaDocumentoes
        [HttpPost]
        public async Task<ActionResult<EntregaDocumento>> PostEntregaDocumento([FromBody] EntregaDocumentoCreateRequest request)
        {
            var entregaDocumento = new EntregaDocumento
            {
                CodigoEntrega = request.CodigoEntrega,
                CodigoSolicitud = request.CodigoSolicitud,
                CodigoTipoDoc = request.CodigoTipoDoc,
                CodigoFormato = request.CodigoFormato,
                CodigoMedio = request.CodigoMedio,
                FechaEntrega = DateTime.SpecifyKind(request.FechaEntrega, DateTimeKind.Utc),
                Estado = request.Estado,
                SolicitudId = 1, // S001 tiene SolicitudId = 1
                TipoDocumentoTipoDocId = 1, // TD001 tiene TipoDocId = 1
                FormatoEntregaFormatoId = 1, // F001 tiene FormatoId = 1
                MedioEnvioMedioId = 1 // ME001 tiene MedioId = 1
            };

            _context.EntregaDocumento.Add(entregaDocumento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntregaDocumento", new { id = entregaDocumento.EntregaId }, entregaDocumento);
        }

        // DELETE: api/EntregaDocumentoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntregaDocumento(int id)
        {
            var entregaDocumento = await _context.EntregaDocumento.FindAsync(id);
            if (entregaDocumento == null)
            {
                return NotFound();
            }

            _context.EntregaDocumento.Remove(entregaDocumento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntregaDocumentoExists(int id)
        {
            return _context.EntregaDocumento.Any(e => e.EntregaId == id);
        }
    }
}
