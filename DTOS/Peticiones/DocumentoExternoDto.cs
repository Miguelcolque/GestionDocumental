using System.ComponentModel.DataAnnotations;

namespace GestionDocumental.Dtos.Peticiones
{
    /// <summary>
    /// DTO para recibir documentos de otros departamentos mediante POST.
    /// No es una entidad de base de datos, solo transporte.
    /// </summary>
    public class DocumentoExternoUploadDto
    {
        [Key]
        [Required(ErrorMessage = "El archivo es obligatorio")]
        public IFormFile Archivo { get; set; }

        [Required(ErrorMessage = "El código de solicitud es obligatorio")]
        [StringLength(50)]
        public string CodigoSolicitud { get; set; }

        [Required(ErrorMessage = "El código de tipo de documento es obligatorio")]
        [StringLength(50)]
        public string CodigoTipoDoc { get; set; }

        [Required(ErrorMessage = "El departamento origen es obligatorio")]
        [StringLength(100)]
        public string DepartamentoOrigen { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        [StringLength(50)]
        public string? CodigoPaciente { get; set; }

        [StringLength(50)]
        public string? CodigoEquipo { get; set; }

        [StringLength(500)]
        public string? Comentario { get; set; }
    }
}