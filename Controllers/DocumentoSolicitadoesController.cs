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
    public class DocumentoSolicitadoesController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;

        public DocumentoSolicitadoesController(GestionDocumentalContext context)
        {
            _context = context;
        }

        // GET: api/DocumentoSolicitadoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoSolicitado>>> GetDocumentoSolicitado()
        {
            return await _context.DocumentoSolicitado.ToListAsync();
        }

        // GET: api/DocumentoSolicitadoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoSolicitado>> GetDocumentoSolicitado(int id)
        {
            var documentoSolicitado = await _context.DocumentoSolicitado.FindAsync(id);

            if (documentoSolicitado == null)
            {
                return NotFound();
            }

            return documentoSolicitado;
        }

        // GET: api/DocumentoSolicitadoes/Visualizar/{codigo}
        [HttpGet("Visualizar/{codigo}")]
        public async Task<ActionResult<object>> VisualizarDocumento(string codigo)
        {
            try
            {
                var documento = await _context.DocumentoSolicitado
                    .FirstOrDefaultAsync(d => d.CodigoDocSolicitado == codigo);

                if (documento == null)
                {
                    return NotFound($"Documento con código '{codigo}' no encontrado.");
                }

                // Obtener información relacionada
                var paciente = await _context.PacienteDoc.FirstOrDefaultAsync(p => p.CodigoPaciente.Contains(documento.CodigoSolicitud));
                var medico = await _context.MedicoDoc.FirstOrDefaultAsync(m => m.CodigoMedico.Contains(documento.CodigoSolicitud));

                return Ok(new
                {
                    Mensaje = "Información del documento",
                    CodigoDocumento = documento.CodigoDocSolicitado,
                    FechaEmision = documento.FechaEmision,
                    Estado = documento.Estado,
                    RutaArchivo = documento.ArchivoUrl,
                    CodigoSolicitud = documento.CodigoSolicitud,
                    CodigoTipoDoc = documento.CodigoTipoDoc,
                    InformacionPaciente = paciente != null ? new
                    {
                        CodigoPaciente = paciente.CodigoPaciente,
                        NombrePaciente = paciente.NombrePaciente,
                        Estado = paciente.Estado
                    } : null,
                    InformacionMedico = medico != null ? new
                    {
                        CodigoMedico = medico.CodigoMedico,
                        NombreMedico = medico.NombreMedico,
                        Estado = medico.Estado
                    } : null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener documento: {ex.Message}");
            }
        }

        // GET: api/DocumentoSolicitadoes/Descargar/{codigo}
        [HttpGet("Descargar/{codigo}")]
        public async Task<IActionResult> DescargarDocumento(string codigo)
        {
            try
            {
                var documento = await _context.DocumentoSolicitado
                    .FirstOrDefaultAsync(d => d.CodigoDocSolicitado == codigo);

                if (documento == null)
                {
                    return NotFound($"Documento con código '{codigo}' no encontrado.");
                }

                // Construir la ruta física del archivo
                var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), documento.ArchivoUrl.TrimStart('/'));

                if (!System.IO.File.Exists(rutaArchivo))
                {
                    return NotFound("El archivo físico no existe en el servidor.");
                }

                // Leer el archivo y retornarlo
                var archivoBytes = await System.IO.File.ReadAllBytesAsync(rutaArchivo);
                var contentType = GetContentType(Path.GetExtension(rutaArchivo));
                var nombreArchivo = $"Documento_{codigo}_{Path.GetFileName(rutaArchivo)}";

                return File(archivoBytes, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al descargar documento: {ex.Message}");
            }
        }

        // GET: api/DocumentoSolicitadoes/ListarTodos
        [HttpGet("ListarTodos")]
        public async Task<ActionResult<object>> ListarTodosDocumentos()
        {
            try
            {
                var documentos = await _context.DocumentoSolicitado
                    .Select(d => new
                    {
                        d.DocSolicitadoId,
                        d.CodigoDocSolicitado,
                        d.FechaEmision,
                        d.Estado,
                        d.ArchivoUrl,
                        d.CodigoSolicitud,
                        d.CodigoTipoDoc,
                        NombreArchivo = Path.GetFileName(d.ArchivoUrl),
                        TipoArchivo = Path.GetExtension(d.ArchivoUrl),
                        FechaCreacion = d.FechaEmision
                    })
                    .OrderByDescending(d => d.FechaEmision)
                    .ToListAsync();

                // Calcular tamaños fuera de la consulta LINQ
                var resultado = documentos.Select(d => new
                {
                    d.DocSolicitadoId,
                    d.CodigoDocSolicitado,
                    d.FechaEmision,
                    d.Estado,
                    d.ArchivoUrl,
                    d.CodigoSolicitud,
                    d.CodigoTipoDoc,
                    d.NombreArchivo,
                    d.TipoArchivo,
                    d.FechaCreacion,
                    TamañoArchivo = GetFileSize(d.ArchivoUrl)
                }).ToList();

                return Ok(new
                {
                    Mensaje = "Lista de todos los documentos",
                    TotalDocumentos = resultado.Count,
                    Documentos = resultado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al listar documentos: {ex.Message}");
            }
        }

        // PUT: api/DocumentoSolicitadoes/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<DocumentoSolicitado>> PutDocumentoSolicitado(string codigo, DocumentoSolicitado documentoSolicitado)
        {
            // Buscar el documento solicitado por código
            var documentoExistente = await _context.DocumentoSolicitado.FirstOrDefaultAsync(d => d.CodigoDocSolicitado == codigo);
            
            if (documentoExistente == null)
            {
                return NotFound($"Documento solicitado con código '{codigo}' no encontrado.");
            }

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (documentoSolicitado.CodigoSolicitud != null && documentoSolicitado.CodigoSolicitud != "string")
                documentoExistente.CodigoSolicitud = documentoSolicitado.CodigoSolicitud;
            
            if (documentoSolicitado.CodigoTipoDoc != null && documentoSolicitado.CodigoTipoDoc != "string")
                documentoExistente.CodigoTipoDoc = documentoSolicitado.CodigoTipoDoc;
            
            if (documentoSolicitado.FechaEmision != default && documentoSolicitado.FechaEmision > DateTime.MinValue)
                documentoExistente.FechaEmision = documentoSolicitado.FechaEmision;
            
            if (documentoSolicitado.ArchivoUrl != null && documentoSolicitado.ArchivoUrl != "string")
                documentoExistente.ArchivoUrl = documentoSolicitado.ArchivoUrl;
            
            if (documentoSolicitado.Estado != null && documentoSolicitado.Estado != "string")
                documentoExistente.Estado = documentoSolicitado.Estado;

            // Mantener el código original del parámetro
            documentoExistente.CodigoDocSolicitado = codigo;

            _context.Entry(documentoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentoSolicitadoExists(documentoExistente.DocSolicitadoId))
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

        // POST: api/DocumentoSolicitadoes
        [HttpPost]
        public async Task<ActionResult<DocumentoSolicitado>> PostDocumentoSolicitado([FromBody] DocumentoSolicitadoCreateRequest request)
        {
            var documentoSolicitado = new DocumentoSolicitado
            {
                CodigoDocSolicitado = request.CodigoDocSolicitado,
                CodigoSolicitud = request.CodigoSolicitud,
                CodigoTipoDoc = request.CodigoTipoDoc,
                FechaEmision = DateTime.SpecifyKind(request.FechaEmision, DateTimeKind.Utc),
                ArchivoUrl = request.ArchivoUrl,
                Estado = request.Estado,
                SolicitudId = 1, // Por ahora usamos la primera solicitud (S001 tiene SolicitudId = 1)
                TipoDocumentoTipoDocId = 1 // Por ahora usamos el primer tipo de documento
            };

            _context.DocumentoSolicitado.Add(documentoSolicitado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDocumentoSolicitado", new { id = documentoSolicitado.DocSolicitadoId }, documentoSolicitado);
        }

        // POST: api/DocumentoSolicitadoes/SubirArchivo
        [HttpPost("SubirArchivo")]
        public async Task<ActionResult<object>> SubirArchivo([FromForm] SubirArchivoRequest request)
        {
            try
            {
                // Validar que se haya subido un archivo
                if (request.Archivo == null || request.Archivo.Length == 0)
                {
                    return BadRequest("No se ha subido ningún archivo.");
                }

                // Validar extensiones permitidas
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip" };
                var extension = Path.GetExtension(request.Archivo.FileName).ToLowerInvariant();
                
                if (!extensionesPermitidas.Contains(extension))
                {
                    return BadRequest($"Extensión de archivo no permitida. Extensiones permitidas: {string.Join(", ", extensionesPermitidas)}");
                }

                // Generar nombre único para el archivo
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "Archivos", "Documentos");
                
                // Crear carpeta si no existe
                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                // Guardar archivo físicamente
                var rutaCompleta = Path.Combine(rutaCarpeta, nombreUnico);
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await request.Archivo.CopyToAsync(stream);
                }

                // Generar código único para el documento
                var codigoDocumento = $"DOC{DateTime.Now:yyyyMMddHHmmss}";

                // Buscar paciente para obtener nombre completo
                var paciente = await _context.PacienteDoc.FirstOrDefaultAsync(p => p.CodigoPaciente == request.CodigoPaciente);

                // Crear registro del documento
                var documentoSolicitado = new DocumentoSolicitado
                {
                    CodigoDocSolicitado = codigoDocumento,
                    CodigoSolicitud = request.CodigoSolicitud ?? "S001", // Por defecto S001 si no se especifica
                    CodigoTipoDoc = request.CodigoTipoDoc ?? "TD001", // Por defecto TD001 si no se especifica
                    FechaEmision = DateTime.UtcNow,
                    ArchivoUrl = $"/Archivos/Documentos/{nombreUnico}",
                    Estado = "Activo",
                    SolicitudId = 1, // Por ahora usamos la primera solicitud
                    TipoDocumentoTipoDocId = 1 // Por ahora usamos el primer tipo de documento
                };

                _context.DocumentoSolicitado.Add(documentoSolicitado);
                await _context.SaveChangesAsync();

                // Retornar respuesta con información del archivo subido
                return Ok(new
                {
                    Mensaje = "Archivo subido exitosamente",
                    CodigoDocumento = codigoDocumento,
                    NombreArchivo = request.Archivo.FileName,
                    NombreOriginal = request.NombreArchivo ?? request.Archivo.FileName,
                    DescripcionContenido = request.DescripcionContenido,
                    NombreMedico = request.NombreMedico,
                    NombreDepartamento = request.NombreDepartamento,
                    CodigoPaciente = request.CodigoPaciente,
                    NombrePaciente = paciente?.NombrePaciente ?? "Paciente no encontrado",
                    FechaCreacion = DateTime.UtcNow,
                    Estado = "Activo",
                    RutaArchivo = documentoSolicitado.ArchivoUrl,
                    TamañoArchivo = request.Archivo.Length,
                    TipoArchivo = extension
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al subir el archivo: {ex.Message}");
            }
        }

        // DELETE: api/DocumentoSolicitadoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentoSolicitado(int id)
        {
            var documentoSolicitado = await _context.DocumentoSolicitado.FindAsync(id);
            if (documentoSolicitado == null)
            {
                return NotFound();
            }

            _context.DocumentoSolicitado.Remove(documentoSolicitado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DocumentoSolicitadoExists(int id)
        {
            return _context.DocumentoSolicitado.Any(e => e.DocSolicitadoId == id);
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".txt":
                    return "text/plain";
                case ".zip":
                    return "application/zip";
                default:
                    return "application/octet-stream";
            }
        }

        private string GetFileSize(string rutaArchivo)
        {
            try
            {
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo.TrimStart('/'));
                if (System.IO.File.Exists(rutaCompleta))
                {
                    var bytes = new FileInfo(rutaCompleta).Length;
                    if (bytes < 1024) return $"{bytes} B";
                    if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
                    return $"{bytes / (1024 * 1024)} MB";
                }
                return "N/A";
            }
            catch
            {
                return "N/A";
            }
        }
    }

    // DTO para la subida de archivos
    public class SubirArchivoRequest
    {
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string DescripcionContenido { get; set; }
        public string NombreMedico { get; set; }
        public string NombreDepartamento { get; set; }
        public string CodigoPaciente { get; set; }
        public string CodigoSolicitud { get; set; }
        public string CodigoTipoDoc { get; set; }
    }
}
