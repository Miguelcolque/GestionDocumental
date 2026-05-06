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
    public class TipoDocumentoesController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;
        private readonly ConsultasGenericas _consultas;

        public TipoDocumentoesController(GestionDocumentalContext context)
        {
            _context = context;
            _consultas = new ConsultasGenericas(context);
        }

        // GET: api/TipoDocumentoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoDocumento>>> GetTipoDocumento()
        {
            return await _context.TipoDocumento.ToListAsync();
        }

        // GET: api/TipoDocumentoes/Activos
        [HttpGet("Activos")]
        public async Task<ActionResult<IEnumerable<TipoDocumento>>> GetTipoDocumentoActivos()
        {
            var activos = await _context.TipoDocumento
                .Where(t => t.Estado == "Activo")
                .ToListAsync();
            return Ok(activos);
        }

        // GET: api/TipoDocumentoes/Desactivados
        [HttpGet("Desactivados")]
        public async Task<ActionResult<IEnumerable<TipoDocumento>>> GetTipoDocumentoDesactivados()
        {
            var desactivados = await _context.TipoDocumento
                .Where(t => t.Estado == "Desactivado")
                .ToListAsync();
            return Ok(desactivados);
        }

        // GET: api/TipoDocumentoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoDocumento>> GetTipoDocumento(int id)
        {
            var tipoDocumento = await _context.TipoDocumento.FindAsync(id);

            if (tipoDocumento == null)
            {
                return NotFound();
            }

            return tipoDocumento;
        }

        // PUT: api/TipoDocumentoes/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<object>> PutTipoDocumento(string codigo, TipoDocumento tipoDocumento)
        {
            // Buscar el tipo de documento por código
            var tipoExistente = await _context.TipoDocumento.FirstOrDefaultAsync(t => t.CodigoTipoDoc == codigo);
            
            if (tipoExistente == null)
            {
                return NotFound($"Tipo de documento con código '{codigo}' no encontrado.");
            }

            // Mostrar datos existentes antes de actualizar
            var datosActuales = new
            {
                Mensaje = "Datos actuales del registro que se va a actualizar",
                DatosAntesDeActualizar = new
                {
                    tipoExistente.TipoDocId,
                    tipoExistente.CodigoTipoDoc,
                    tipoExistente.NombreDocumento,
                    tipoExistente.DepartamentoOrigen,
                    tipoExistente.Estado
                }
            };

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (tipoDocumento.NombreDocumento != null && tipoDocumento.NombreDocumento != "string")
                tipoExistente.NombreDocumento = tipoDocumento.NombreDocumento;
            
            if (tipoDocumento.DepartamentoOrigen != null && tipoDocumento.DepartamentoOrigen != "string")
                tipoExistente.DepartamentoOrigen = tipoDocumento.DepartamentoOrigen;
            
            if (tipoDocumento.Estado != null && tipoDocumento.Estado != "string")
                tipoExistente.Estado = tipoDocumento.Estado;

            // Mantener el código original del parámetro
            tipoExistente.CodigoTipoDoc = codigo;

            _context.Entry(tipoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoDocumentoExists(tipoExistente.TipoDocId))
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
                    tipoExistente.TipoDocId,
                    tipoExistente.CodigoTipoDoc,
                    tipoExistente.NombreDocumento,
                    tipoExistente.DepartamentoOrigen,
                    tipoExistente.Estado
                },
                CamposModificados = new
                {
                    NombreDocumento = tipoDocumento.NombreDocumento != null && tipoDocumento.NombreDocumento != "string" ? tipoDocumento.NombreDocumento : "No modificado",
                    DepartamentoOrigen = tipoDocumento.DepartamentoOrigen != null && tipoDocumento.DepartamentoOrigen != "string" ? tipoDocumento.DepartamentoOrigen : "No modificado",
                    Estado = tipoDocumento.Estado != null && tipoDocumento.Estado != "string" ? tipoDocumento.Estado : "No modificado"
                }
            });
        }

        // POST: api/TipoDocumentoes
        [HttpPost]
        public async Task<ActionResult<TipoDocumento>> PostTipoDocumento([FromBody] TipoDocumentoCreateRequest request)
        {
            var tipoDocumento = new TipoDocumento
            {
                CodigoTipoDoc = request.CodigoTipoDoc,
                NombreDocumento = request.NombreDocumento,
                DepartamentoOrigen = request.DepartamentoOrigen,
                Estado = request.Estado
            };

            _context.TipoDocumento.Add(tipoDocumento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoDocumento", new { id = tipoDocumento.TipoDocId }, tipoDocumento);
        }

        // DELETE: api/TipoDocumentoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoDocumento(int id)
        {
            var tipoDocumento = await _context.TipoDocumento.FindAsync(id);
            if (tipoDocumento == null)
            {
                return NotFound();
            }

            // Cambiar estado a Desactivado en lugar de eliminar
            tipoDocumento.Estado = "Desactivado";
            _context.Entry(tipoDocumento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Tipo de documento desactivado exitosamente", Codigo = tipoDocumento.CodigoTipoDoc, Estado = tipoDocumento.Estado });
        }

        // GET: api/TipoDocumentoes/MasSolicitados
        [HttpGet("MasSolicitados")]
        public async Task<ActionResult<IEnumerable<object>>> GetTiposDocumentosMasSolicitados()
        {
            var masSolicitados = await _consultas.TiposDocumentosMasSolicitadosAsync();
            return Ok(masSolicitados);
        }

        // GET: api/TipoDocumentoes/ConFormatos
        [HttpGet("ConFormatos")]
        public async Task<ActionResult<IEnumerable<object>>> GetTiposDocumentoConFormatos()
        {
            var query = from td in _context.TipoDocumento
                        join ds in _context.DocumentoSolicitado on td.CodigoTipoDoc equals ds.CodigoTipoDoc into tds
                        from ds in tds.DefaultIfEmpty()
                        join df in _context.DocumentoFormato on ds.CodigoDocSolicitado equals df.CodigoDocSolicitado into dfs
                        from df in dfs.DefaultIfEmpty()
                        join fe in _context.FormatoEntrega on df.CodigoFormato equals fe.CodigoFormato into fes
                        from fe in fes.DefaultIfEmpty()
                        group new { td, fe } by new { td.TipoDocId, td.CodigoTipoDoc, td.NombreDocumento, td.DepartamentoOrigen } into g
                        select new
                        {
                            TipoDocId = g.Key.TipoDocId,
                            CodigoTipoDoc = g.Key.CodigoTipoDoc,
                            NombreDocumento = g.Key.NombreDocumento,
                            DepartamentoOrigen = g.Key.DepartamentoOrigen,
                            TotalSolicitudes = _context.DocumentoSolicitado.Count(x => x.CodigoTipoDoc == g.Key.CodigoTipoDoc),
                            FormatosDisponibles = g.Select(x => x.fe.NombreFormato).Where(x => x != null).Distinct().ToList(),
                            TieneFormatosAsignados = g.Any(x => x.fe != null)
                        };

            return await query.ToListAsync();
        }

        // GET: api/TipoDocumentoes/EstadisticasPorDepartamento
        [HttpGet("EstadisticasPorDepartamento")]
        public async Task<ActionResult<IEnumerable<object>>> GetEstadisticasPorDepartamento()
        {
            var estadisticas = await _consultas.EstadisticasDocumentosPorDepartamentoAsync();
            return Ok(estadisticas);
        }

        // GET: api/TipoDocumentoes/SinFormatoAsignado/{codigoTipoDoc}
        [HttpGet("SinFormatoAsignado/{codigoTipoDoc}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDocumentosSinFormatoAsignado(string codigoTipoDoc)
        {
            var documentos = await _consultas.DocumentosSinFormatoAsignadoAsync();
            var filtrados = documentos.Where(x => x.GetType().GetProperty("CodigoTipoDoc")?.GetValue(x)?.ToString() == codigoTipoDoc);
            return Ok(filtrados);
        }

        private bool TipoDocumentoExists(int id)
        {
            return _context.TipoDocumento.Any(e => e.TipoDocId == id);
        }
    }
}
