using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class OrdenLaboratorioDto
    {
        [JsonPropertyName("examenCodigo")]
        public string ExamenCodigo { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("tiempoProcesamiento")]
        public int TiempoProcesamiento { get; set; }

        [JsonPropertyName("requiereAyuno")]
        public bool RequiereAyuno { get; set; }
    }
}
