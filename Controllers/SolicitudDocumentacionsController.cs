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
    public class SolicitudDocumentacionsController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;
        private readonly ConsultasGenericas _consultas;

        public SolicitudDocumentacionsController(GestionDocumentalContext context)
        {
            _context = context;
            _consultas = new ConsultasGenericas(context);
        }

        // GET: api/SolicitudDocumentacions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudDocumentacion>>> GetSolicitudDocumentacion()
        {
            return await _context.SolicitudDocumentacion.ToListAsync();
        }

        // GET: api/SolicitudDocumentacions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudDocumentacion>> GetSolicitudDocumentacion(int id)
        {
            var solicitudDocumentacion = await _context.SolicitudDocumentacion.FindAsync(id);

            if (solicitudDocumentacion == null)
            {
                return NotFound();
            }

            return solicitudDocumentacion;
        }

        // PUT: api/SolicitudDocumentacions/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<SolicitudDocumentacion>> PutSolicitudDocumentacion(string codigo, SolicitudDocumentacion solicitudDocumentacion)
        {
            // Buscar la solicitud por código
            var solicitudExistente = await _context.SolicitudDocumentacion.FirstOrDefaultAsync(s => s.CodigoSolicitud == codigo);
            
            if (solicitudExistente == null)
            {
                return NotFound($"Solicitud con código '{codigo}' no encontrada.");
            }

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (solicitudDocumentacion.FechaSolicitud != default && solicitudDocumentacion.FechaSolicitud > DateTime.MinValue)
                solicitudExistente.FechaSolicitud = solicitudDocumentacion.FechaSolicitud;
            
            if (solicitudDocumentacion.CodigoPaciente != null && solicitudDocumentacion.CodigoPaciente != "string")
                solicitudExistente.CodigoPaciente = solicitudDocumentacion.CodigoPaciente;
            
            if (solicitudDocumentacion.DepartamentoSolicitante != null && solicitudDocumentacion.DepartamentoSolicitante != "string")
                solicitudExistente.DepartamentoSolicitante = solicitudDocumentacion.DepartamentoSolicitante;
            
            if (solicitudDocumentacion.CodigoMedico != null && solicitudDocumentacion.CodigoMedico != "string")
                solicitudExistente.CodigoMedico = solicitudDocumentacion.CodigoMedico;
            
            if (solicitudDocumentacion.Estado != null && solicitudDocumentacion.Estado != "string")
                solicitudExistente.Estado = solicitudDocumentacion.Estado;

            // Mantener el código original del parámetro
            solicitudExistente.CodigoSolicitud = codigo;

            _context.Entry(solicitudExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudDocumentacionExists(solicitudExistente.SolicitudId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Retornar el registro actualizado
            return Ok(solicitudExistente);
        }

        // POST: api/SolicitudDocumentacions
        [HttpPost]
        public async Task<ActionResult<SolicitudDocumentacion>> PostSolicitudDocumentacion([FromBody] SolicitudDocumentacionCreateRequest request)
        {
            var solicitudDocumentacion = new SolicitudDocumentacion
            {
                CodigoSolicitud = request.CodigoSolicitud,
                FechaSolicitud = DateTime.SpecifyKind(request.FechaSolicitud, DateTimeKind.Utc),
                CodigoPaciente = request.CodigoPaciente,
                DepartamentoSolicitante = request.DepartamentoSolicitante,
                CodigoMedico = request.CodigoMedico,
                Estado = request.Estado,
                MedicoDocMedicoId = null, // Será null hasta que tengamos relación directa
                PacienteDocPacienteId = null // Será null hasta que tengamos relación directa
            };

            _context.SolicitudDocumentacion.Add(solicitudDocumentacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSolicitudDocumentacion", new { id = solicitudDocumentacion.SolicitudId }, solicitudDocumentacion);
        }

        // DELETE: api/SolicitudDocumentacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudDocumentacion(int id)
        {
            var solicitudDocumentacion = await _context.SolicitudDocumentacion.FindAsync(id);
            if (solicitudDocumentacion == null)
            {
                return NotFound();
            }

            _context.SolicitudDocumentacion.Remove(solicitudDocumentacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SolicitudDocumentacionExists(int id)
        {
            return _context.SolicitudDocumentacion.Any(e => e.SolicitudId == id);
        }

        // ENDPOINTS MIS CONSULTAS PARA TOMA DE DECISIONES

        // Consultar documentos por paciente (JOIN)
        [HttpGet("consultas/documentos-por-paciente/{codigoPaciente}")]
        public async Task<ActionResult<IEnumerable<object>>> ConsultarDocumentosPorPaciente(string codigoPaciente)
        {
            var result = await _consultas.ConsultarDocumentosPorPacienteAsync(codigoPaciente);
            return Ok(result);
        }

        // Estadísticas de documentos por departamento (GROUP BY + COUNT)
        [HttpGet("consultas/estadisticas-por-departamento")]
        public async Task<ActionResult<IEnumerable<object>>> EstadisticasPorDepartamento()
        {
            var result = await _consultas.EstadisticasDocumentosPorDepartamentoAsync();
            return Ok(result);
        }

        // Total de solicitudes por médico (GROUP BY + COUNT)
        [HttpGet("consultas/solicitudes-por-medico")]
        public async Task<ActionResult<IEnumerable<object>>> SolicitudesPorMedico()
        {
            var result = await _consultas.TotalSolicitudesPorMedicoAsync();
            return Ok(result);
        }

        // Buscar solicitudes por código de paciente (BÚSQUEDA FILTRADA)
        [HttpGet("consultas/buscar-por-paciente/{codigoPaciente}")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarPorCodigoPaciente(string codigoPaciente)
        {
            var result = await _consultas.BuscarSolicitudesPorCodigoPacienteAsync(codigoPaciente);
            return Ok(result);
        }

        // Documentos sin formato asignado (NOT EXISTS)
        [HttpGet("consultas/documentos-sin-formato")]
        public async Task<ActionResult<IEnumerable<object>>> DocumentosSinFormato()
        {
            var result = await _consultas.DocumentosSinFormatoAsignadoAsync();
            return Ok(result);
        }

        // Listar documentos por tipo y formato disponible
        [HttpGet("consultas/documentos-por-tipo-formato")]
        public async Task<ActionResult<IEnumerable<object>>> DocumentosPorTipoYFormato()
        {
            var result = await _consultas.ListarDocumentosPorTipoYFormatoAsync();
            return Ok(result);
        }

        // Entregas pendientes por medio de envío
        [HttpGet("consultas/entregas-pendientes")]
        public async Task<ActionResult<IEnumerable<object>>> EntregasPendientes()
        {
            var result = await _consultas.EntregasPendientesPorMedioEnvioAsync();
            return Ok(result);
        }

        // Reporte de entregas completadas por fecha
        [HttpGet("consultas/reporte-entregas")]
        public async Task<ActionResult<IEnumerable<object>>> ReporteEntregas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            var result = await _consultas.ReporteEntregasCompletadasPorFechaAsync(fechaInicio, fechaFin);
            return Ok(result);
        }

        // Tipos de documentos más solicitados
        [HttpGet("consultas/documentos-mas-solicitados")]
        public async Task<ActionResult<IEnumerable<object>>> DocumentosMasSolicitados()
        {
            var result = await _consultas.TiposDocumentosMasSolicitadosAsync();
            return Ok(result);
        }

        // Documentos sin medio de envío registrado
        [HttpGet("consultas/documentos-sin-medio-envio")]
        public async Task<ActionResult<IEnumerable<object>>> DocumentosSinMedioEnvio()
        {
            var result = await _consultas.DocumentosSinMedioEnvioAsync();
            return Ok(result);
        }
    }
}
