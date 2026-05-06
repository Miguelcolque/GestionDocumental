using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class PsicologoTestDto
    {
        [JsonPropertyName("nombreTest")]
        public string NombreTest { get; set; }

        [JsonPropertyName("tipoTest")]
        public string TipoTest { get; set; }

        [JsonPropertyName("codigoTest")]
        public string CodigoTest { get; set; }

        [JsonPropertyName("codigoPsicologo")]
        public string CodigoPsicologo { get; set; }
    }
}
