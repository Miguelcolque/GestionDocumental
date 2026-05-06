namespace GestionDocumental.DTOs.CreateRequests
{
    public class TipoDocumentoCreateRequest
    {
        public string CodigoTipoDoc { get; set; }
        public string NombreDocumento { get; set; }
        public string DepartamentoOrigen { get; set; }
        public string Estado { get; set; }
    }
}
