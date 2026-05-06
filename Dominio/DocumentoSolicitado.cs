using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDocumental.Dominio
{
    public class DocumentoSolicitado
    {
        [Key]
        public int DocSolicitadoId { get; set; }
        public string CodigoDocSolicitado { get; set; }
        public string CodigoSolicitud { get; set; }
        public string CodigoTipoDoc { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ArchivoUrl { get; set; }
        public string Estado { get; set; }

        public int SolicitudId { get; set; }
        public int TipoDocumentoTipoDocId { get; set; }
    }
}
