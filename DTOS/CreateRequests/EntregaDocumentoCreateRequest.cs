using System;

namespace GestionDocumental.DTOs.CreateRequests
{
    public class EntregaDocumentoCreateRequest
    {
        public string CodigoEntrega { get; set; }
        public string CodigoSolicitud { get; set; }
        public string CodigoTipoDoc { get; set; }
        public string CodigoFormato { get; set; }
        public string CodigoMedio { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; }
    }
}
