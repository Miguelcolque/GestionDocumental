using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class InvestigacionClinicaRequestDTO
    {
        [JsonPropertyName("codigoInvestigacion")]
        public string CodigoInvestigacion { get; set; }

        [JsonPropertyName("codigoPaciente")]
        public string CodigoPaciente { get; set; }

        [JsonPropertyName("tipoPrueba")]
        public string TipoPrueba { get; set; }

        [JsonPropertyName("valorObtenido")]
        public string ValorObtenido { get; set; }

        [JsonPropertyName("fechaInvestigacion")]
        public DateTime? FechaInvestigacion { get; set; }

        [JsonPropertyName("nombreMedico")]
        public string NombreMedico { get; set; }

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }
    }
}
