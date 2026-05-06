using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDocumental.Dominio
{
    public class DocumentoMedio
    {
        [Key]
        public int DocMedioId { get; set; }
        public string CodigoDocMedio { get; set; }
        public string CodigoDocSolicitado { get; set; }
        public string CodigoMedio { get; set; }
        public string Estado { get; set; }

        public int DocumentoSolicitadoDocSolicitadoId { get; set; }
        public int MedioEnvioMedioId { get; set; }
    }
}
