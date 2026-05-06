using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GestionDocumental.Dominio
{
    public class FormatoEntrega
    {
        [Key]
        public int FormatoId { get; set; }
        public string CodigoFormato { get; set; }
        public string NombreFormato { get; set; }
        public string Estado { get; set; }

        public virtual ICollection<DocumentoFormato> DocumentosFormato { get; set; } = new List<DocumentoFormato>();
        public virtual ICollection<EntregaDocumento> Entregas { get; set; } = new List<EntregaDocumento>();
    }
}
