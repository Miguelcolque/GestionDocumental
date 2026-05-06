using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class InvestigacionClinicaResponseDTO
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

        [JsonPropertyName("nombrePaciente")]
        public string NombrePaciente { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }
    }
}
