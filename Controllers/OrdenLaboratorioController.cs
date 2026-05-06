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
    public class OrdenLaboratorioController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://investigacionclinica-production.up.railway.app/api/OrdenLaboratorio";

        public OrdenLaboratorioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene la lista de exámenes de laboratorio desde el API externa
        /// </summary>
        /// <returns>Lista de exámenes de laboratorio disponibles</returns>
        [HttpGet("ListarExamenes")]
        public async Task<ActionResult<IEnumerable<OrdenLaboratorioDto>>> ListarExamenes()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/ListarExamenes");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var responseWrapper = JsonSerializer.Deserialize<OrdenLaboratorioResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var examenes = responseWrapper?.Examenes;

                return Ok(examenes);
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
        /// Obtiene un examen de laboratorio específico por su código
        /// </summary>
        /// <param name="examenCodigo">Código del examen a buscar</param>
        /// <returns>Información del examen de laboratorio</returns>
        [HttpGet("Obtener-Por-Codigo/{examenCodigo}")]
        public async Task<ActionResult<OrdenLaboratorioDto>> ObtenerExamenPorCodigo(string examenCodigo)
        {
            try
            {
                // Primero obtenemos todos los exámenes
                var response = await _httpClient.GetAsync($"{_baseUrl}/ListarExamenes");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var responseWrapper = JsonSerializer.Deserialize<OrdenLaboratorioResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var examenes = responseWrapper?.Examenes;

                // Buscamos el examen específico por código
                var examenEncontrado = examenes?.Find(e => e.ExamenCodigo == examenCodigo);

                if (examenEncontrado == null)
                {
                    return NotFound($"Examen de laboratorio con código '{examenCodigo}' no encontrado.");
                }

                return Ok(examenEncontrado);
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
        /// Obtiene exámenes de laboratorio que requieren ayuno
        /// </summary>
        /// <returns>Lista de exámenes que requieren ayuno</returns>
        [HttpGet("Obtener-Requieren-Ayuno")]
        public async Task<ActionResult<IEnumerable<OrdenLaboratorioDto>>> ObtenerExamenesRequierenAyuno()
        {
            try
            {
                // Primero obtenemos todos los exámenes
                var response = await _httpClient.GetAsync($"{_baseUrl}/ListarExamenes");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var responseWrapper = JsonSerializer.Deserialize<OrdenLaboratorioResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var examenes = responseWrapper?.Examenes;

                // Filtramos los exámenes que requieren ayuno
                var examenesConAyuno = examenes?.FindAll(e => e.RequiereAyuno == true);

                if (examenesConAyuno == null || examenesConAyuno.Count == 0)
                {
                    return NotFound("No se encontraron exámenes que requieran ayuno.");
                }

                return Ok(examenesConAyuno);
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
        /// Obtiene exámenes de laboratorio que NO requieren ayuno
        /// </summary>
        /// <returns>Lista de exámenes que no requieren ayuno</returns>
        [HttpGet("Obtener-Sin-Ayuno")]
        public async Task<ActionResult<IEnumerable<OrdenLaboratorioDto>>> ObtenerExamenesSinAyuno()
        {
            try
            {
                // Primero obtenemos todos los exámenes
                var response = await _httpClient.GetAsync($"{_baseUrl}/ListarExamenes");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var responseWrapper = JsonSerializer.Deserialize<OrdenLaboratorioResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var examenes = responseWrapper?.Examenes;

                // Filtramos los exámenes que NO requieren ayuno
                var examenesSinAyuno = examenes?.FindAll(e => e.RequiereAyuno == false);

                if (examenesSinAyuno == null || examenesSinAyuno.Count == 0)
                {
                    return NotFound("No se encontraron exámenes que no requieran ayuno.");
                }

                return Ok(examenesSinAyuno);
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
        /// Obtiene exámenes de laboratorio por tiempo de procesamiento máximo
        /// </summary>
        /// <param name="tiempoMaximo">Tiempo máximo de procesamiento en minutos</param>
        /// <returns>Lista de exámenes dentro del tiempo especificado</returns>
        [HttpGet("Obtener-Por-Tiempo/{tiempoMaximo}")]
        public async Task<ActionResult<IEnumerable<OrdenLaboratorioDto>>> ObtenerExamenesPorTiempo(int tiempoMaximo)
        {
            try
            {
                // Primero obtenemos todos los exámenes
                var response = await _httpClient.GetAsync($"{_baseUrl}/ListarExamenes");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var responseWrapper = JsonSerializer.Deserialize<OrdenLaboratorioResponseDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var examenes = responseWrapper?.Examenes;

                // Filtramos los exámenes por tiempo de procesamiento
                var examenesPorTiempo = examenes?.FindAll(e => e.TiempoProcesamiento <= tiempoMaximo);

                if (examenesPorTiempo == null || examenesPorTiempo.Count == 0)
                {
                    return NotFound($"No se encontraron exámenes con tiempo de procesamiento menor o igual a {tiempoMaximo} minutos.");
                }

                return Ok(examenesPorTiempo);
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
