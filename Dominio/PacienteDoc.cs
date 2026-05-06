using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestionDocumental.Dominio
{
    public class PacienteDoc
    {
        [Key]
        public int PacienteId { get; set; }
        public string CodigoPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string Estado { get; set; }

        public virtual ICollection<SolicitudDocumentacion> Solicitudes { get; set; } = new List<SolicitudDocumentacion>();
    }
}
