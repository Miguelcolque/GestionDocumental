using System;
using System.Collections.Generic;
using System.Linq;
using GestionDocumental.Dominio;
using Microsoft.EntityFrameworkCore;

namespace GestionDocumental.Consultas
{
    public class ConsultasGenericas
    {
        private readonly DbContext _context;

        public ConsultasGenericas(DbContext context)
        {
            _context = context;
        }

        // 1. Listado general con JOIN entre al menos 2 tablas
        // Caso de uso: "Consultar documentos solicitados por paciente"
        public async Task<IEnumerable<object>> ConsultarDocumentosPorPacienteAsync(string codigoPaciente)
        {
            var query = from sd in _context.Set<SolicitudDocumentacion>()
                        join p in _context.Set<PacienteDoc>() on sd.CodigoPaciente equals p.CodigoPaciente
                        join ds in _context.Set<DocumentoSolicitado>() on sd.CodigoSolicitud equals ds.CodigoSolicitud
                        join td in _context.Set<TipoDocumento>() on ds.CodigoTipoDoc equals td.CodigoTipoDoc
                        where p.CodigoPaciente == codigoPaciente
                        select new
                        {
                            CodigoSolicitud = sd.CodigoSolicitud,
                            FechaSolicitud = sd.FechaSolicitud,
                            NombrePaciente = p.NombrePaciente,
                            NombreDocumento = td.NombreDocumento,
                            FechaEmision = ds.FechaEmision,
                            ArchivoUrl = ds.ArchivoUrl,
                            EstadoSolicitud = sd.Estado,
                            EstadoDocumento = ds.Estado
                        };

            return await query.ToListAsync();
        }

        // 2. Agrupación con conteo (GROUP BY + COUNT)
        // Caso de uso: "Ver estadísticas de documentos por departamento"
        public async Task<IEnumerable<object>> EstadisticasDocumentosPorDepartamentoAsync()
        {
            var query = from sd in _context.Set<SolicitudDocumentacion>()
                        group sd by sd.DepartamentoSolicitante into g
                        select new
                        {
                            Departamento = g.Key,
                            TotalSolicitudes = g.Count(),
                            SolicitudesPendientes = g.Count(x => x.Estado == "Pendiente"),
                            SolicitudesCompletadas = g.Count(x => x.Estado == "Completada"),
                            PorcentajeCompletadas = g.Count(x => x.Estado == "Completada") * 100.0 / g.Count()
                        };

            return await query.ToListAsync();
        }

        // 3. Agrupación con suma (GROUP BY + SUM)
        // Caso de uso: "Consultar total de solicitudes por médico" (adaptado para mostrar totales)
        public async Task<IEnumerable<object>> TotalSolicitudesPorMedicoAsync()
        {
            var query = from sd in _context.Set<SolicitudDocumentacion>()
                        join m in _context.Set<MedicoDoc>() on sd.CodigoMedico equals m.CodigoMedico
                        group sd by new { m.CodigoMedico, m.NombreMedico } into g
                        select new
                        {
                            CodigoMedico = g.Key.CodigoMedico,
                            NombreMedico = g.Key.NombreMedico,
                            TotalSolicitudes = g.Count(),
                            SolicitudesEsteMes = g.Count(x => x.FechaSolicitud.Month == DateTime.Now.Month && 
                                                             x.FechaSolicitud.Year == DateTime.Now.Year),
                            PromedioDiario = g.Count() / 30.0, // Promedio aproximado
                            UltimaSolicitud = g.Max(x => x.FechaSolicitud)
                        };

            return await query.ToListAsync();
        }

        // 4. Búsqueda filtrada por código o identificador
        // Caso de uso: "Buscar solicitudes por código de paciente"
        public async Task<IEnumerable<object>> BuscarSolicitudesPorCodigoPacienteAsync(string codigoPaciente)
        {
            var query = from sd in _context.Set<SolicitudDocumentacion>()
                        where sd.CodigoPaciente.Contains(codigoPaciente)
                        select new
                        {
                            CodigoSolicitud = sd.CodigoSolicitud,
                            FechaSolicitud = sd.FechaSolicitud,
                            CodigoPaciente = sd.CodigoPaciente,
                            DepartamentoSolicitante = sd.DepartamentoSolicitante,
                            CodigoMedico = sd.CodigoMedico,
                            Estado = sd.Estado
                        };

            return await query.ToListAsync();
        }

        // 5. Consulta de registros que no tienen relación en otra tabla (NOT EXISTS)
        // Caso de uso: "Listar documentos sin formato asignado"
        public async Task<IEnumerable<object>> DocumentosSinFormatoAsignadoAsync()
        {
            var query = from ds in _context.Set<DocumentoSolicitado>()
                        where !(from df in _context.Set<DocumentoFormato>()
                               select df.CodigoDocSolicitado)
                               .Contains(ds.CodigoDocSolicitado)
                        select new
                        {
                            CodigoDocSolicitado = ds.CodigoDocSolicitado,
                            CodigoSolicitud = ds.CodigoSolicitud,
                            CodigoTipoDoc = ds.CodigoTipoDoc,
                            FechaEmision = ds.FechaEmision,
                            ArchivoUrl = ds.ArchivoUrl,
                            Estado = ds.Estado,
                            Motivo = "Sin formato asignado"
                        };

            return await query.ToListAsync();
        }

        // Consultas adicionales para completar los 10 casos de uso

        // 6. Listar documentos por tipo y formato disponible
        public async Task<IEnumerable<object>> ListarDocumentosPorTipoYFormatoAsync()
        {
            var query = from td in _context.Set<TipoDocumento>()
                        join ds in _context.Set<DocumentoSolicitado>() on td.CodigoTipoDoc equals ds.CodigoTipoDoc
                        join df in _context.Set<DocumentoFormato>() on ds.CodigoDocSolicitado equals df.CodigoDocSolicitado
                        join fe in _context.Set<FormatoEntrega>() on df.CodigoFormato equals fe.CodigoFormato
                        where ds.Estado == "Disponible"
                        group new { td, fe } by new { td.CodigoTipoDoc, td.NombreDocumento, td.DepartamentoOrigen, fe.NombreFormato, fe.CodigoFormato } into g
                        select new
                        {
                            TipoDocumento = g.Key.NombreDocumento,
                            CodigoTipoDoc = g.Key.CodigoTipoDoc,
                            FormatoDisponible = g.Key.NombreFormato,
                            CodigoFormato = g.Key.CodigoFormato,
                            DepartamentoOrigen = g.Key.DepartamentoOrigen,
                            TotalDisponibles = g.Count()
                        };

            return await query.ToListAsync();
        }

        // 7. Consultar entregas pendientes por medio de envío
        public async Task<IEnumerable<object>> EntregasPendientesPorMedioEnvioAsync()
        {
            var query = from ed in _context.Set<EntregaDocumento>()
                        join me in _context.Set<MedioEnvio>() on ed.CodigoMedio equals me.CodigoMedio
                        where ed.Estado == "Pendiente"
                        select new
                        {
                            CodigoEntrega = ed.CodigoEntrega,
                            CodigoSolicitud = ed.CodigoSolicitud,
                            MedioEnvio = me.NombreMedio,
                            CodigoMedio = me.CodigoMedio,
                            FechaEntrega = ed.FechaEntrega,
                            DiasPendiente = (DateTime.UtcNow - ed.FechaEntrega).Days,
                            Estado = ed.Estado
                        };

            return await query.ToListAsync();
        }

        // 8. Generar reporte de entregas completadas por fecha
        public async Task<IEnumerable<object>> ReporteEntregasCompletadasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var fechaInicioUtc = DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc);
            var fechaFinUtc = DateTime.SpecifyKind(fechaFin, DateTimeKind.Utc);
            
            var query = from ed in _context.Set<EntregaDocumento>()
                        join sd in _context.Set<SolicitudDocumentacion>() on ed.CodigoSolicitud equals sd.CodigoSolicitud
                        join td in _context.Set<TipoDocumento>() on ed.CodigoTipoDoc equals td.CodigoTipoDoc
                        where ed.Estado == "Completada" && 
                              ed.FechaEntrega >= fechaInicioUtc && 
                              ed.FechaEntrega <= fechaFinUtc
                        group ed by new { ed.FechaEntrega.Date, td.NombreDocumento } into g
                        select new
                        {
                            Fecha = g.Key.Date,
                            TipoDocumento = g.Key.NombreDocumento,
                            TotalEntregas = g.Count(),
                            EntregasPorFormato = g.GroupBy(x => x.CodigoFormato)
                                                 .Select(x => new { Formato = x.Key, Cantidad = x.Count() })
                        };

            return await query.ToListAsync();
        }

        // 9. Consultar tipos de documentos más solicitados
        public async Task<IEnumerable<object>> TiposDocumentosMasSolicitadosAsync()
        {
            var query = from ds in _context.Set<DocumentoSolicitado>()
                        join td in _context.Set<TipoDocumento>() on ds.CodigoTipoDoc equals td.CodigoTipoDoc
                        group ds by new { td.CodigoTipoDoc, td.NombreDocumento, td.DepartamentoOrigen } into g
                        orderby g.Count() descending
                        select new
                        {
                            CodigoTipoDoc = g.Key.CodigoTipoDoc,
                            NombreDocumento = g.Key.NombreDocumento,
                            DepartamentoOrigen = g.Key.DepartamentoOrigen,
                            TotalSolicitudes = g.Count(),
                            PorcentajeTotal = g.Count() * 100.0 / _context.Set<DocumentoSolicitado>().Count()
                        };

            return await query.ToListAsync();
        }

        // 10. Ver documentos que no tienen medio de envío registrado
        public async Task<IEnumerable<object>> DocumentosSinMedioEnvioAsync()
        {
            var query = from ds in _context.Set<DocumentoSolicitado>()
                        where !(from dm in _context.Set<DocumentoMedio>()
                               select dm.CodigoDocSolicitado)
                               .Contains(ds.CodigoDocSolicitado)
                        select new
                        {
                            CodigoDocSolicitado = ds.CodigoDocSolicitado,
                            CodigoSolicitud = ds.CodigoSolicitud,
                            CodigoTipoDoc = ds.CodigoTipoDoc,
                            FechaEmision = ds.FechaEmision,
                            ArchivoUrl = ds.ArchivoUrl,
                            Estado = ds.Estado,
                            Motivo = "Sin medio de envío registrado"
                        };

            return await query.ToListAsync();
        }
    }
}
