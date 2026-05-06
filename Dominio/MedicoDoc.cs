using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestionDocumental.Dominio
{
    public class MedicoDoc
    {
        [Key]
        public int MedicoId { get; set; }
        public string CodigoMedico { get; set; }
        public string NombreMedico { get; set; }
        public string Estado { get; set; }

        public virtual ICollection<SolicitudDocumentacion> Solicitudes { get; set; } = new List<SolicitudDocumentacion>();
    }
}
