using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestionDocumental.Dominio
{
    public class TipoDocumento
    {
        [Key]
        public int TipoDocId { get; set; }
        public string CodigoTipoDoc { get; set; }
        public string NombreDocumento { get; set; }
        public string DepartamentoOrigen { get; set; }
        public string Estado { get; set; }

        public virtual ICollection<DocumentoSolicitado> DocumentosSolicitados { get; set; } = new List<DocumentoSolicitado>();
        public virtual ICollection<EntregaDocumento> Entregas { get; set; } = new List<EntregaDocumento>();
    }
}
