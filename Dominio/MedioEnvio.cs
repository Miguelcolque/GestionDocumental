using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestionDocumental.Dominio
{
    public class MedioEnvio
    {
        [Key]
        public int MedioId { get; set; }
        public string CodigoMedio { get; set; }
        public string NombreMedio { get; set; }
        public string Estado { get; set; }

        public virtual ICollection<DocumentoMedio> DocumentosMedio { get; set; } = new List<DocumentoMedio>();
        public virtual ICollection<EntregaDocumento> Entregas { get; set; } = new List<EntregaDocumento>();
    }
}
