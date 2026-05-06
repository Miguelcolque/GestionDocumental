using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestionDocumental.Dominio;

namespace GestionDocumental.Data
{
    public class GestionDocumentalContext : DbContext
    {
        public GestionDocumentalContext (DbContextOptions<GestionDocumentalContext> options)
            : base(options)
        {
        }

        public DbSet<GestionDocumental.Dominio.SolicitudDocumentacion> SolicitudDocumentacion { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.DocumentoSolicitado> DocumentoSolicitado { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.EntregaDocumento> EntregaDocumento { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.DocumentoFormato> DocumentoFormato { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.DocumentoMedio> DocumentoMedio { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.FormatoEntrega> FormatoEntrega { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.MedicoDoc> MedicoDoc { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.MedioEnvio> MedioEnvio { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.PacienteDoc> PacienteDoc { get; set; } = default!;
        public DbSet<GestionDocumental.Dominio.TipoDocumento> TipoDocumento { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
