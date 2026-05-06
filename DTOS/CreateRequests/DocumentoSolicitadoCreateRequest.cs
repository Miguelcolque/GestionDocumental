namespace GestionDocumental.DTOs.CreateRequests
{
    public class DocumentoSolicitadoCreateRequest
    {
        public string CodigoDocSolicitado { get; set; }
        public string CodigoSolicitud { get; set; }
        public string CodigoTipoDoc { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ArchivoUrl { get; set; }
        public string Estado { get; set; }
    }
}
