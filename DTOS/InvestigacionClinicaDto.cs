using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class InvestigacionClinicaDto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("tipoEstudio")]
        public string TipoEstudio { get; set; }

        [JsonPropertyName("fase")]
        public string Fase { get; set; }

        [JsonPropertyName("fechaInicio")]
        public string FechaInicio { get; set; }

        [JsonPropertyName("fechaFin")]
        public string FechaFin { get; set; }
    }
}
