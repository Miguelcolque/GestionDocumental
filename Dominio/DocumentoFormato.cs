using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDocumental.Dominio
{
    public class DocumentoFormato
    {
        [Key]
        public int DocFormatoId { get; set; }
        public string CodigoDocFormato { get; set; }
        public string CodigoDocSolicitado { get; set; }
        public string CodigoFormato { get; set; }
        public string Estado { get; set; }

        public int DocumentoSolicitadoDocSolicitadoId { get; set; }
        public int FormatoEntregaFormatoId { get; set; }
    }
}
