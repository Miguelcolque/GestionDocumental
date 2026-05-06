using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GestionDocumental.DTOs;

namespace GestionDocumental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestigacionClinicaController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://investigacionclinica-production.up.railway.app/api/Investigaciones";

        public InvestigacionClinicaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene la lista de investigaciones clínicas desde el API externa
        /// </summary>
        /// <returns>Lista de investigaciones clínicas disponibles</returns>
        [HttpGet("Lista")]
        public async Task<ActionResult<IEnumerable<InvestigacionClinicaDto>>> ListarInvestigaciones()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Lista");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var investigaciones = JsonSerializer.Deserialize<List<InvestigacionClinicaDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(investigaciones);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión con el API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error al procesar la respuesta del API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene una investigación clínica específica por su código
        /// </summary>
        /// <param name="codigo">Código de la investigación a buscar</param>
        /// <returns>Información de la investigación clínica</returns>
        [HttpGet("Obtener-Por-Codigo/{codigo}")]
        public async Task<ActionResult<InvestigacionClinicaDto>> ObtenerInvestigacionPorCodigo(string codigo)
        {
            try
            {
                // Primero obtenemos todas las investigaciones
                var response = await _httpClient.GetAsync($"{_baseUrl}/Lista");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var investigaciones = JsonSerializer.Deserialize<List<InvestigacionClinicaDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Buscamos la investigación específica por código
                var investigacionEncontrada = investigaciones?.Find(i => i.Codigo == codigo);

                if (investigacionEncontrada == null)
                {
                    return NotFound($"Investigación clínica con código '{codigo}' no encontrada.");
                }

                return Ok(investigacionEncontrada);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión con el API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error al procesar la respuesta del API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene investigaciones clínicas filtradas por fase
        /// </summary>
        /// <param name="fase">Fase de la investigación para filtrar</param>
        /// <returns>Lista de investigaciones por fase</returns>
        [HttpGet("Obtener-Por-Fase/{fase}")]
        public async Task<ActionResult<IEnumerable<InvestigacionClinicaDto>>> ObtenerInvestigacionesPorFase(string fase)
        {
            try
            {
                // Primero obtenemos todas las investigaciones
                var response = await _httpClient.GetAsync($"{_baseUrl}/Lista");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var investigaciones = JsonSerializer.Deserialize<List<InvestigacionClinicaDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Filtramos las investigaciones por fase
                var investigacionesPorFase = investigaciones?.FindAll(i => i.Fase.ToLower() == fase.ToLower());

                if (investigacionesPorFase == null || investigacionesPorFase.Count == 0)
                {
                    return NotFound($"No se encontraron investigaciones en fase '{fase}'.");
                }

                return Ok(investigacionesPorFase);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión con el API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error al procesar la respuesta del API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene investigaciones clínicas filtradas por tipo de estudio
        /// </summary>
        /// <param name="tipoEstudio">Tipo de estudio para filtrar</param>
        /// <returns>Lista de investigaciones por tipo de estudio</returns>
        [HttpGet("Obtener-Por-TipoEstudio/{tipoEstudio}")]
        public async Task<ActionResult<IEnumerable<InvestigacionClinicaDto>>> ObtenerInvestigacionesPorTipoEstudio(string tipoEstudio)
        {
            try
            {
                // Primero obtenemos todas las investigaciones
                var response = await _httpClient.GetAsync($"{_baseUrl}/Lista");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var investigaciones = JsonSerializer.Deserialize<List<InvestigacionClinicaDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Filtramos las investigaciones por tipo de estudio
                var investigacionesPorTipo = investigaciones?.FindAll(i => i.TipoEstudio.ToLower().Contains(tipoEstudio.ToLower()));

                if (investigacionesPorTipo == null || investigacionesPorTipo.Count == 0)
                {
                    return NotFound($"No se encontraron investigaciones con tipo de estudio '{tipoEstudio}'.");
                }

                return Ok(investigacionesPorTipo);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de conexión con el API externa: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Error al procesar la respuesta del API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error inesperado: {ex.Message}");
            }
        }
    }
}
