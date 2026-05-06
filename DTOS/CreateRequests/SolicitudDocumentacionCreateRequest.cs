using System;

namespace GestionDocumental.DTOs.CreateRequests
{
    public class SolicitudDocumentacionCreateRequest
    {
        public string CodigoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string CodigoPaciente { get; set; }
        public string DepartamentoSolicitante { get; set; }
        public string CodigoMedico { get; set; }
        public string Estado { get; set; }
    }
}
