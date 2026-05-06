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
    public class PacienteDocsController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;
        private readonly ConsultasGenericas _consultas;

        public PacienteDocsController(GestionDocumentalContext context)
        {
            _context = context;
            _consultas = new ConsultasGenericas(context);
        }

        // GET: api/PacienteDocs
        [HttpGet]
        public async Task<ActionResult<object>> GetPacienteDoc()
        {
            try
            {
                // Obtener todos los pacientes
                var pacientes = await _context.PacienteDoc.ToListAsync();

                // Obtener pacientes que tienen documentos asociados
                var pacientesConDocumentos = await _context.DocumentoSolicitado
                    .Where(d => d.Estado == "Activo")
                    .GroupBy(d => d.CodigoSolicitud)
                    .Select(g => new
                    {
                        CodigoPaciente = g.Key,
                        TotalDocumentos = g.Count(),
                        UltimoDocumento = g.Max(d => d.FechaEmision)
                    })
                    .ToListAsync();

                // Unir información completa
                var resultado = pacientes.Select(p => new
                {
                    p.PacienteId,
                    p.CodigoPaciente,
                    p.NombrePaciente,
                    p.Estado,
                    
                    // Calcular EstadoAtencion
                    EstadoAtencion = p.Estado == "Desactivado" ? "Desactivado" :
                                   pacientesConDocumentos.Any(pd => pd.CodigoPaciente == p.CodigoPaciente) ? "Atendido" : "Sin atender",
                    
                    // Información de documentos si tiene
                    TotalDocumentos = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.TotalDocumentos ?? 0,
                    UltimoDocumento = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.UltimoDocumento
                }).OrderBy(p => p.NombrePaciente).ToList();

                return Ok(new
                {
                    Mensaje = "Lista completa de pacientes",
                    TotalPacientes = resultado.Count,
                    Pacientes = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pacientes: {ex.Message}");
            }
        }

        // GET: api/PacienteDocs/Activos
        [HttpGet("Activos")]
        public async Task<ActionResult<object>> GetPacienteDocActivos()
        {
            try
            {
                // Obtener pacientes activos
                var pacientesActivos = await _context.PacienteDoc
                    .Where(p => p.Estado == "Activo")
                    .ToListAsync();

                // Obtener pacientes que tienen documentos asociados
                var pacientesConDocumentos = await _context.DocumentoSolicitado
                    .Where(d => d.Estado == "Activo")
                    .GroupBy(d => d.CodigoSolicitud)
                    .Select(g => new
                    {
                        CodigoPaciente = g.Key,
                        TotalDocumentos = g.Count(),
                        UltimoDocumento = g.Max(d => d.FechaEmision)
                    })
                    .ToListAsync();

                // Unir información completa
                var resultado = pacientesActivos.Select(p => new
                {
                    p.PacienteId,
                    p.CodigoPaciente,
                    p.NombrePaciente,
                    p.Estado,
                    
                    // Calcular EstadoAtencion para pacientes activos
                    EstadoAtencion = pacientesConDocumentos.Any(pd => pd.CodigoPaciente == p.CodigoPaciente) ? "Atendido" : "Sin atender",
                    
                    // Información de documentos si tiene
                    TotalDocumentos = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.TotalDocumentos ?? 0,
                    UltimoDocumento = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.UltimoDocumento
                }).OrderBy(p => p.NombrePaciente).ToList();

                return Ok(new
                {
                    Mensaje = "Pacientes activos",
                    TotalPacientes = resultado.Count,
                    Pacientes = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pacientes activos: {ex.Message}");
            }
        }

        // GET: api/PacienteDocs/Desactivados
        [HttpGet("Desactivados")]
        public async Task<ActionResult<object>> GetPacienteDocDesactivados()
        {
            try
            {
                // Obtener pacientes desactivados
                var pacientesDesactivados = await _context.PacienteDoc
                    .Where(p => p.Estado == "Desactivado")
                    .ToListAsync();

                // Obtener pacientes que tienen documentos asociados (para información histórica)
                var pacientesConDocumentos = await _context.DocumentoSolicitado
                    .Where(d => d.Estado == "Activo")
                    .GroupBy(d => d.CodigoSolicitud)
                    .Select(g => new
                    {
                        CodigoPaciente = g.Key,
                        TotalDocumentos = g.Count(),
                        UltimoDocumento = g.Max(d => d.FechaEmision)
                    })
                    .ToListAsync();

                // Unir información completa
                var resultado = pacientesDesactivados.Select(p => new
                {
                    p.PacienteId,
                    p.CodigoPaciente,
                    p.NombrePaciente,
                    p.Estado,
                    
                    // Para desactivados, EstadoAtencion siempre es "Desactivado"
                    EstadoAtencion = "Desactivado",
                    
                    // Información histórica de documentos si tuvo
                    TotalDocumentos = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.TotalDocumentos ?? 0,
                    UltimoDocumento = pacientesConDocumentos
                        .FirstOrDefault(pd => pd.CodigoPaciente == p.CodigoPaciente)?.UltimoDocumento
                }).OrderBy(p => p.NombrePaciente).ToList();

                return Ok(new
                {
                    Mensaje = "Pacientes desactivados",
                    TotalPacientes = resultado.Count,
                    Pacientes = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pacientes desactivados: {ex.Message}");
            }
        }

        // GET: api/PacienteDocs/SinAtender
        [HttpGet("SinAtender")]
        public async Task<ActionResult<object>> GetPacientesSinAtender()
        {
            try
            {
                // Obtener todos los pacientes activos
                var pacientesActivos = await _context.PacienteDoc
                    .Where(p => p.Estado == "Activo")
                    .ToListAsync();

                // Obtener pacientes que tienen documentos asociados
                var pacientesConDocumentos = await _context.DocumentoSolicitado
                    .Where(d => d.Estado == "Activo")
                    .Select(d => d.CodigoSolicitud)
                    .Distinct()
                    .ToListAsync();

                // Filtrar pacientes sin documentos
                var pacientesSinDocumentos = pacientesActivos
                    .Where(p => !pacientesConDocumentos.Contains(p.CodigoPaciente))
                    .Select(p => new
                    {
                        p.PacienteId,
                        p.CodigoPaciente,
                        p.NombrePaciente,
                        p.Estado,
                        EstadoAtencion = "Sin atender",
                        TotalDocumentos = 0,
                        UltimoDocumento = (DateTime?)null
                    })
                    .OrderBy(p => p.NombrePaciente)
                    .ToList();

                return Ok(new
                {
                    Mensaje = "Pacientes sin atender",
                    TotalPacientes = pacientesSinDocumentos.Count,
                    Pacientes = pacientesSinDocumentos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pacientes sin atender: {ex.Message}");
            }
        }

        // GET: api/PacienteDocs/Atendidos
        [HttpGet("Atendidos")]
        public async Task<ActionResult<object>> GetPacientesAtendidos()
        {
            try
            {
                // Obtener pacientes que tienen documentos asociados
                var pacientesConDocumentos = await _context.DocumentoSolicitado
                    .Where(d => d.Estado == "Activo")
                    .GroupBy(d => d.CodigoSolicitud)
                    .Select(g => new
                    {
                        CodigoPaciente = g.Key,
                        TotalDocumentos = g.Count(),
                        UltimoDocumento = g.Max(d => d.FechaEmision),
                        Documentos = g.OrderByDescending(d => d.FechaEmision)
                                  .Select(d => new
                                  {
                                      d.CodigoDocSolicitado,
                                      d.FechaEmision,
                                      d.ArchivoUrl,
                                      NombreArchivo = Path.GetFileName(d.ArchivoUrl),
                                      TipoArchivo = Path.GetExtension(d.ArchivoUrl)
                                  }).ToList()
                    })
                    .ToListAsync();

                // Unir con información completa del paciente
                var resultado = await Task.Run(async () =>
                {
                    var lista = new List<object>();
                    foreach (var pacDoc in pacientesConDocumentos)
                    {
                        var paciente = await _context.PacienteDoc
                            .FirstOrDefaultAsync(p => p.CodigoPaciente == pacDoc.CodigoPaciente);

                        if (paciente != null)
                        {
                            lista.Add(new
                            {
                                paciente.PacienteId,
                                paciente.CodigoPaciente,
                                paciente.NombrePaciente,
                                paciente.Estado,
                                EstadoAtencion = "Atendido",
                                pacDoc.TotalDocumentos,
                                pacDoc.UltimoDocumento,
                                DocumentosRecientes = pacDoc.Documentos.Take(3).ToList() // Últimos 3 documentos
                            });
                        }
                    }
                    return lista.OrderByDescending(p => ((dynamic)p).UltimoDocumento).ToList();
                });

                return Ok(new
                {
                    Mensaje = "Pacientes atendidos con documentos",
                    TotalPacientes = resultado.Count,
                    Pacientes = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener pacientes atendidos: {ex.Message}");
            }
        }

        // GET: api/PacienteDocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteDoc>> GetPacienteDoc(int id)
        {
            var pacienteDoc = await _context.PacienteDoc.FindAsync(id);

            if (pacienteDoc == null)
            {
                return NotFound();
            }

            return pacienteDoc;
        }

        // PUT: api/PacienteDocs/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<object>> PutPacienteDoc(string codigo, PacienteDoc pacienteDoc)
        {
            // Buscar el paciente por código
            var pacienteExistente = await _context.PacienteDoc.FirstOrDefaultAsync(p => p.CodigoPaciente == codigo);
            
            if (pacienteExistente == null)
            {
                return NotFound($"Paciente con código '{codigo}' no encontrado.");
            }

            // Mostrar datos existentes antes de actualizar
            var datosActuales = new
            {
                Mensaje = "Datos actuales del registro que se va a actualizar",
                DatosAntesDeActualizar = new
                {
                    pacienteExistente.PacienteId,
                    pacienteExistente.CodigoPaciente,
                    pacienteExistente.NombrePaciente,
                    pacienteExistente.Estado
                }
            };

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (pacienteDoc.NombrePaciente != null && pacienteDoc.NombrePaciente != "string")
                pacienteExistente.NombrePaciente = pacienteDoc.NombrePaciente;
            
            if (pacienteDoc.Estado != null && pacienteDoc.Estado != "string")
                pacienteExistente.Estado = pacienteDoc.Estado;

            // Mantener el código original del parámetro
            pacienteExistente.CodigoPaciente = codigo;

            _context.Entry(pacienteExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteDocExists(pacienteExistente.PacienteId))
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
                    pacienteExistente.PacienteId,
                    pacienteExistente.CodigoPaciente,
                    pacienteExistente.NombrePaciente,
                    pacienteExistente.Estado
                },
                CamposModificados = new
                {
                    NombrePaciente = pacienteDoc.NombrePaciente != null && pacienteDoc.NombrePaciente != "string" ? pacienteDoc.NombrePaciente : "No modificado",
                    Estado = pacienteDoc.Estado != null && pacienteDoc.Estado != "string" ? pacienteDoc.Estado : "No modificado"
                }
            });
        }

        // POST: api/PacienteDocs
        [HttpPost]
        public async Task<ActionResult<PacienteDoc>> PostPacienteDoc([FromBody] PacienteDocCreateRequest request)
        {
            var pacienteDoc = new PacienteDoc
            {
                CodigoPaciente = request.CodigoPaciente,
                NombrePaciente = request.NombrePaciente,
                Estado = request.FueAtendido ? "Atendido" : "Activo"
            };

            _context.PacienteDoc.Add(pacienteDoc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPacienteDoc", new { id = pacienteDoc.PacienteId }, pacienteDoc);
        }

        // DELETE: api/PacienteDocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePacienteDoc(int id)
        {
            var pacienteDoc = await _context.PacienteDoc.FindAsync(id);
            if (pacienteDoc == null)
            {
                return NotFound();
            }

            // Cambiar estado a Desactivado en lugar de eliminar
            pacienteDoc.Estado = "Desactivado";
            _context.Entry(pacienteDoc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Paciente desactivado exitosamente", Codigo = pacienteDoc.CodigoPaciente, Estado = pacienteDoc.Estado });
        }

        // GET: api/PacienteDocs/BuscarPorCodigo/{codigo}
        [HttpGet("BuscarPorCodigo/{codigo}")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarPorCodigo(string codigo)
        {
            var resultados = await _consultas.BuscarSolicitudesPorCodigoPacienteAsync(codigo);
            return Ok(resultados);
        }

        // GET: api/PacienteDocs/DocumentosPorPaciente/{codigoPaciente}
        [HttpGet("DocumentosPorPaciente/{codigoPaciente}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDocumentosPorPaciente(string codigoPaciente)
        {
            var documentos = await _consultas.ConsultarDocumentosPorPacienteAsync(codigoPaciente);
            return Ok(documentos);
        }

        // GET: api/PacienteDocs/ConSolicitudes
        [HttpGet("ConSolicitudes")]
        public async Task<ActionResult<IEnumerable<object>>> GetPacientesConSolicitudes()
        {
            var query = from p in _context.PacienteDoc
                        join sd in _context.SolicitudDocumentacion on p.CodigoPaciente equals sd.CodigoPaciente into ps
                        from sd in ps.DefaultIfEmpty()
                        select new
                        {
                            PacienteId = p.PacienteId,
                            CodigoPaciente = p.CodigoPaciente,
                            NombrePaciente = p.NombrePaciente,
                            Estado = p.Estado,
                            TieneSolicitudes = ps != null,
                            TotalSolicitudes = _context.SolicitudDocumentacion.Count(x => x.CodigoPaciente == p.CodigoPaciente)
                        };

            return await query.ToListAsync();
        }

        private bool PacienteDocExists(int id)
        {
            return _context.PacienteDoc.Any(e => e.PacienteId == id);
        }
    }
}
