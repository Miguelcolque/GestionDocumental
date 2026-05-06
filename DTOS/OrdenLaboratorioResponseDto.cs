using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GestionDocumental.DTOs
{
    public class OrdenLaboratorioResponseDto
    {
        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }

        [JsonPropertyName("examenes")]
        public List<OrdenLaboratorioDto> Examenes { get; set; }
    }
}
