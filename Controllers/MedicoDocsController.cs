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
    public class MedicoDocsController : ControllerBase
    {
        private readonly GestionDocumentalContext _context;
        private readonly ConsultasGenericas _consultas;

        public MedicoDocsController(GestionDocumentalContext context)
        {
            _context = context;
            _consultas = new ConsultasGenericas(context);
        }

        // GET: api/MedicoDocs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicoDoc>>> GetMedicoDoc()
        {
            return await _context.MedicoDoc.ToListAsync();
        }

        // GET: api/MedicoDocs/Activos
        [HttpGet("Activos")]
        public async Task<ActionResult<IEnumerable<MedicoDoc>>> GetMedicoDocActivos()
        {
            var activos = await _context.MedicoDoc
                .Where(m => m.Estado == "Activo")
                .ToListAsync();
            return Ok(activos);
        }

        // GET: api/MedicoDocs/Desactivados
        [HttpGet("Desactivados")]
        public async Task<ActionResult<IEnumerable<MedicoDoc>>> GetMedicoDocDesactivados()
        {
            var desactivados = await _context.MedicoDoc
                .Where(m => m.Estado == "Desactivado")
                .ToListAsync();
            return Ok(desactivados);
        }

        // GET: api/MedicoDocs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicoDoc>> GetMedicoDoc(int id)
        {
            var medicoDoc = await _context.MedicoDoc.FindAsync(id);

            if (medicoDoc == null)
            {
                return NotFound();
            }

            return medicoDoc;
        }

        // PUT: api/MedicoDocs/ActualizarPorCodigo/{codigo}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("ActualizarPorCodigo/{codigo}")]
        public async Task<ActionResult<object>> PutMedicoDoc(string codigo, MedicoDoc medicoDoc)
        {
            // Buscar el médico por código
            var medicoExistente = await _context.MedicoDoc.FirstOrDefaultAsync(m => m.CodigoMedico == codigo);
            
            if (medicoExistente == null)
            {
                return NotFound($"Médico con código '{codigo}' no encontrado.");
            }

            // Mostrar datos existentes antes de actualizar
            var datosActuales = new
            {
                Mensaje = "Datos actuales del registro que se va a actualizar",
                DatosAntesDeActualizar = new
                {
                    medicoExistente.MedicoId,
                    medicoExistente.CodigoMedico,
                    medicoExistente.NombreMedico,
                    medicoExistente.Estado
                }
            };

            // Actualizar solo los campos que vienen en el body (ignorar el código del body)
            if (medicoDoc.NombreMedico != null && medicoDoc.NombreMedico != "string")
                medicoExistente.NombreMedico = medicoDoc.NombreMedico;
            
            if (medicoDoc.Estado != null && medicoDoc.Estado != "string")
                medicoExistente.Estado = medicoDoc.Estado;

            // Mantener el código original del parámetro
            medicoExistente.CodigoMedico = codigo;

            _context.Entry(medicoExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicoDocExists(medicoExistente.MedicoId))
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
                    medicoExistente.MedicoId,
                    medicoExistente.CodigoMedico,
                    medicoExistente.NombreMedico,
                    medicoExistente.Estado
                },
                CamposModificados = new
                {
                    NombreMedico = medicoDoc.NombreMedico != null && medicoDoc.NombreMedico != "string" ? medicoDoc.NombreMedico : "No modificado",
                    Estado = medicoDoc.Estado != null && medicoDoc.Estado != "string" ? medicoDoc.Estado : "No modificado"
                }
            });
        }

        // POST: api/MedicoDocs
        [HttpPost]
        public async Task<ActionResult<MedicoDoc>> PostMedicoDoc([FromBody] MedicoDocCreateRequest request)
        {
            var medicoDoc = new MedicoDoc
            {
                CodigoMedico = request.CodigoMedico,
                NombreMedico = request.NombreMedico,
                Estado = request.Estado
            };

            _context.MedicoDoc.Add(medicoDoc);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicoDoc", new { id = medicoDoc.MedicoId }, medicoDoc);
        }

        // DELETE: api/MedicoDocs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicoDoc(int id)
        {
            var medicoDoc = await _context.MedicoDoc.FindAsync(id);
            if (medicoDoc == null)
            {
                return NotFound();
            }

            // Cambiar estado a Desactivado en lugar de eliminar
            medicoDoc.Estado = "Desactivado";
            _context.Entry(medicoDoc).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = "Médico desactivado exitosamente", Codigo = medicoDoc.CodigoMedico, Estado = medicoDoc.Estado });
        }

        // GET: api/MedicoDocs/TotalSolicitudes
        [HttpGet("TotalSolicitudes")]
        public async Task<ActionResult<IEnumerable<object>>> GetTotalSolicitudesPorMedico()
        {
            var estadisticas = await _consultas.TotalSolicitudesPorMedicoAsync();
            return Ok(estadisticas);
        }

        // GET: api/MedicoDocs/ConSolicitudesActivas
        [HttpGet("ConSolicitudesActivas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMedicosConSolicitudesActivas()
        {
            var query = from m in _context.MedicoDoc
                        join sd in _context.SolicitudDocumentacion on m.CodigoMedico equals sd.CodigoMedico into ms
                        from sd in ms.DefaultIfEmpty()
                        where sd.Estado == "Pendiente" || sd == null
                        select new
                        {
                            MedicoId = m.MedicoId,
                            CodigoMedico = m.CodigoMedico,
                            NombreMedico = m.NombreMedico,
                            Estado = m.Estado,
                            SolicitudesPendientes = _context.SolicitudDocumentacion.Count(x => x.CodigoMedico == m.CodigoMedico && x.Estado == "Pendiente"),
                            UltimaSolicitud = _context.SolicitudDocumentacion.Where(x => x.CodigoMedico == m.CodigoMedico).Max(x => (DateTime?)x.FechaSolicitud)
                        };

            return await query.ToListAsync();
        }

        // GET: api/MedicoDocs/Estadisticas/{codigoMedico}
        [HttpGet("Estadisticas/{codigoMedico}")]
        public async Task<ActionResult<object>> GetEstadisticasMedico(string codigoMedico)
        {
            var query = from m in _context.MedicoDoc
                        where m.CodigoMedico == codigoMedico
                        join sd in _context.SolicitudDocumentacion on m.CodigoMedico equals sd.CodigoMedico into ms
                        from sd in ms.DefaultIfEmpty()
                        join ds in _context.DocumentoSolicitado on sd.CodigoSolicitud equals ds.CodigoSolicitud into ds2
                        from ds in ds2.DefaultIfEmpty()
                        select new
                        {
                            Medico = m.NombreMedico,
                            TotalSolicitudes = ms.Count(),
                            SolicitudesCompletadas = ms.Count(x => x.Estado == "Completada"),
                            SolicitudesPendientes = ms.Count(x => x.Estado == "Pendiente"),
                            DocumentosGenerados = ds2.Count(),
                            PromedioRespuesta = ms.Average(x => (DateTime.Now - x.FechaSolicitud).TotalDays)
                        };

            var resultado = await query.FirstOrDefaultAsync();
            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        private bool MedicoDocExists(int id)
        {
            return _context.MedicoDoc.Any(e => e.MedicoId == id);
        }
    }
}
