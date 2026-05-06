using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDocumental.Dominio
{
    public class EntregaDocumento
    {
        [Key]
        public int EntregaId { get; set; }
        public string CodigoEntrega { get; set; }
        public string CodigoSolicitud { get; set; }
        public string CodigoTipoDoc { get; set; }
        public string CodigoFormato { get; set; }
        public string CodigoMedio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; }

        public int SolicitudId { get; set; }
        public int TipoDocumentoTipoDocId { get; set; }
        public int FormatoEntregaFormatoId { get; set; }
        public int MedioEnvioMedioId { get; set; }
    }
}
