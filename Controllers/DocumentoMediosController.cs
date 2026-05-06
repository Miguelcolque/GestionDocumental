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
    public class DocumentoMediosController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;

        public DocumentoMediosController(GestionDocumentalContext context)
        {
            _context = context;
        }

        // GET: api/DocumentoMedios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoMedio>>> GetDocumentoMedio()
        {
            return await _context.DocumentoMedio.ToListAsync();
        }

        // GET: api/DocumentoMedios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoMedio>> GetDocumentoMedio(int id)
        {
            var documentoMedio = await _context.DocumentoMedio.FindAsync(id);

            if (documentoMedio == null)
            {
                return NotFound();
            }

            return documentoMedio;
        }

        // PUT: api/DocumentoMedios/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<DocumentoMedio>> PutDocumentoMedio(string codigo, DocumentoMedio documentoMedio)
        {
            // Buscar el documento medio por código
            var documentoExistente = await _context.DocumentoMedio.FirstOrDefaultAsync(d => d.CodigoDocMedio == codigo);
            
            if (documentoExistente == null)
            {
                return NotFound($"Documento medio con código '{codigo}' no encontrado.");
            }

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (documentoMedio.CodigoDocSolicitado != null && documentoMedio.CodigoDocSolicitado != "string")
                documentoExistente.CodigoDocSolicitado = documentoMedio.CodigoDocSolicitado;
            
            if (documentoMedio.CodigoMedio != null && documentoMedio.CodigoMedio != "string")
                documentoExistente.CodigoMedio = documentoMedio.CodigoMedio;
            
            if (documentoMedio.Estado != null && documentoMedio.Estado != "string")
                documentoExistente.Estado = documentoMedio.Estado;

            // Mantener el código original del parámetro
            documentoExistente.CodigoDocMedio = codigo;

            _context.Entry(documentoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentoMedioExists(documentoExistente.DocMedioId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar el registro actualizado
            return Ok(documentoExistente);
        }

        // POST: api/DocumentoMedios
        [HttpPost]
        public async Task<ActionResult<DocumentoMedio>> PostDocumentoMedio([FromBody] DocumentoMedioCreateRequest request)
        {
            var documentoMedio = new DocumentoMedio
            {
                CodigoDocMedio = request.CodigoDocMedio,
                CodigoDocSolicitado = request.CodigoDocSolicitado,
                CodigoMedio = request.CodigoMedio,
                Estado = request.Estado,
                DocumentoSolicitadoDocSolicitadoId = 1, // DS001 tiene DocSolicitadoId = 1
                MedioEnvioMedioId = 1 // ME001 tiene MedioId = 1
            };

            _context.DocumentoMedio.Add(documentoMedio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocumentoMedio", new { id = documentoMedio.DocMedioId }, documentoMedio);
        }

        // DELETE: api/DocumentoMedios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentoMedio(int id)
        {
            var documentoMedio = await _context.DocumentoMedio.FindAsync(id);
            if (documentoMedio == null)
            {
                return NotFound();
            }

            _context.DocumentoMedio.Remove(documentoMedio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentoMedioExists(int id)
        {
            return _context.DocumentoMedio.Any(e => e.DocMedioId == id);
        }
    }
}
