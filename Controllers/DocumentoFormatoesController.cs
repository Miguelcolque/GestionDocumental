using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionDocumental.Data;
using GestionDocumental.Dominio;
using GestionDocumental.Consultas;
using GestionDocumental.DTOs.CreateRequests;

namespace GestionDocumental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoFormatoesController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;
        private readonly ConsultasGenericas _consultas;

        public DocumentoFormatoesController(GestionDocumentalContext context)
        {
            _context = context;
            _consultas = new ConsultasGenericas(context);
        }

        // GET: api/DocumentoFormatoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoFormato>>> GetDocumentoFormato()
        {
            return await _context.DocumentoFormato.ToListAsync();
        }

        // GET: api/DocumentoFormatoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoFormato>> GetDocumentoFormato(int id)
        {
            var documentoFormato = await _context.DocumentoFormato.FindAsync(id);

            if (documentoFormato == null)
            {
                return NotFound();
            }

            return documentoFormato;
        }

        // PUT: api/DocumentoFormatoes/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<DocumentoFormato>> PutDocumentoFormato(string codigo, DocumentoFormato documentoFormato)
        {
            // Buscar el documento formato por código
            var formatoExistente = await _context.DocumentoFormato.FirstOrDefaultAsync(d => d.CodigoDocFormato == codigo);
            
            if (formatoExistente == null)
            {
                return NotFound($"Documento formato con código '{codigo}' no encontrado.");
            }

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (documentoFormato.CodigoDocSolicitado != null && documentoFormato.CodigoDocSolicitado != "string")
                formatoExistente.CodigoDocSolicitado = documentoFormato.CodigoDocSolicitado;
            
            if (documentoFormato.CodigoFormato != null && documentoFormato.CodigoFormato != "string")
                formatoExistente.CodigoFormato = documentoFormato.CodigoFormato;
            
            if (documentoFormato.Estado != null && documentoFormato.Estado != "string")
                formatoExistente.Estado = documentoFormato.Estado;

            // Mantener el código original del parámetro
            formatoExistente.CodigoDocFormato = codigo;

            _context.Entry(formatoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentoFormatoExists(formatoExistente.DocFormatoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar el registro actualizado
            return Ok(formatoExistente);
        }

        // POST: api/DocumentoFormatoes
        [HttpPost]
        public async Task<ActionResult<DocumentoFormato>> PostDocumentoFormato([FromBody] DocumentoFormatoCreateRequest request)
        {
            var documentoFormato = new DocumentoFormato
            {
                CodigoDocFormato = request.CodigoDocFormato,
                CodigoDocSolicitado = request.CodigoDocSolicitado,
                CodigoFormato = request.CodigoFormato,
                Estado = request.Estado,
                DocumentoSolicitadoDocSolicitadoId = 1, // DS001 tiene DocSolicitadoId = 1
                FormatoEntregaFormatoId = 1 // F001 tiene FormatoId = 1
            };

            _context.DocumentoFormato.Add(documentoFormato);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocumentoFormato", new { id = documentoFormato.DocFormatoId }, documentoFormato);
        }

        // DELETE: api/DocumentoFormatoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentoFormato(int id)
        {
            var documentoFormato = await _context.DocumentoFormato.FindAsync(id);
            if (documentoFormato == null)
            {
                return NotFound();
            }

            _context.DocumentoFormato.Remove(documentoFormato);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/DocumentoFormatoes/DocumentosPorTipoYFormato
        [HttpGet("DocumentosPorTipoYFormato")]
        public async Task<ActionResult<IEnumerable<object>>> GetDocumentosPorTipoYFormato()
        {
            var documentos = await _consultas.ListarDocumentosPorTipoYFormatoAsync();
            return Ok(documentos);
        }

        // GET: api/DocumentoFormatoes/ConDetalles
        [HttpGet("ConDetalles")]
        public async Task<ActionResult<IEnumerable<object>>> GetDocumentoFormatoConDetalles()
        {
            var query = from df in _context.DocumentoFormato
                        join ds in _context.DocumentoSolicitado on df.CodigoDocSolicitado equals ds.CodigoDocSolicitado
                        join fe in _context.FormatoEntrega on df.CodigoFormato equals fe.CodigoFormato
                        join td in _context.TipoDocumento on ds.CodigoTipoDoc equals td.CodigoTipoDoc
                        select new
                        {
                            DocFormatoId = df.DocFormatoId,
                            CodigoDocFormato = df.CodigoDocFormato,
                            CodigoDocSolicitado = df.CodigoDocSolicitado,
                            CodigoFormato = df.CodigoFormato,
                            Estado = df.Estado,
                            DetalleDocumento = new
                            {
                                NombreDocumento = td.NombreDocumento,
                                DepartamentoOrigen = td.DepartamentoOrigen,
                                FechaEmision = ds.FechaEmision,
                                ArchivoUrl = ds.ArchivoUrl
                            },
                            DetalleFormato = new
                            {
                                NombreFormato = fe.NombreFormato,
                                CodigoFormato = fe.CodigoFormato
                            }
                        };

            return await query.ToListAsync();
        }

        // GET: api/DocumentoFormatoes/SinMedioEnvio
        [HttpGet("SinMedioEnvio")]
        public async Task<ActionResult<IEnumerable<object>>> GetDocumentosSinMedioEnvio()
        {
            var documentos = await _consultas.DocumentosSinMedioEnvioAsync();
            return Ok(documentos);
        }

        // POST: api/DocumentoFormatoes/AsignarFormato
        [HttpPost("AsignarFormato")]
        public async Task<ActionResult<object>> AsignarFormatoADocumento([FromBody] AsignarFormatoRequest request)
        {
            // Verificar si el documento solicitado existe
            var documento = await _context.DocumentoSolicitado
                .FirstOrDefaultAsync(ds => ds.CodigoDocSolicitado == request.CodigoDocSolicitado);

            if (documento == null)
                return NotFound("Documento solicitado no encontrado");

            // Verificar si el formato existe
            var formato = await _context.FormatoEntrega
                .FirstOrDefaultAsync(fe => fe.CodigoFormato == request.CodigoFormato);

            if (formato == null)
                return NotFound("Formato no encontrado");

            // Crear la relación documento-formato
            var documentoFormato = new DocumentoFormato
            {
                CodigoDocFormato = Guid.NewGuid().ToString(),
                CodigoDocSolicitado = request.CodigoDocSolicitado,
                CodigoFormato = request.CodigoFormato,
                Estado = "Activo"
            };

            _context.DocumentoFormato.Add(documentoFormato);
            await _context.SaveChangesAsync();

            return Ok(new { 
                Mensaje = "Formato asignado exitosamente",
                DocFormatoId = documentoFormato.DocFormatoId,
                CodigoDocFormato = documentoFormato.CodigoDocFormato
            });
        }

        private bool DocumentoFormatoExists(int id)
        {
            return _context.DocumentoFormato.Any(e => e.DocFormatoId == id);
        }
    }
}
