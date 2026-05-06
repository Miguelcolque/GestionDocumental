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
    public class PsicologoTestController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://departamento-psicologia-hospital-tercer.onrender.com/api/Psicologo_Test";

        public PsicologoTestController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Obtiene la lista de pruebas psicológicas desde el API externa
        /// </summary>
        /// <returns>Lista de pruebas psicológicas disponibles</returns>
        [HttpGet("Listar-PsicologoTest")]
        public async Task<ActionResult<IEnumerable<PsicologoTestDto>>> ListarPsicologoTest()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Listar-PsicologoTest");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var psicologoTests = JsonSerializer.Deserialize<List<PsicologoTestDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(psicologoTests);
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
        /// Obtiene una prueba psicológica específica por su código
        /// </summary>
        /// <param name="codigoTest">Código de la prueba a buscar</param>
        /// <returns>Información de la prueba psicológica</returns>
        [HttpGet("Obtener-Por-Codigo/{codigoTest}")]
        public async Task<ActionResult<PsicologoTestDto>> ObtenerPsicologoTestPorCodigo(string codigoTest)
        {
            try
            {
                // Primero obtenemos todas las pruebas
                var response = await _httpClient.GetAsync($"{_baseUrl}/Listar-PsicologoTest");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var psicologoTests = JsonSerializer.Deserialize<List<PsicologoTestDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Buscamos la prueba específica por código
                var testEncontrado = psicologoTests?.Find(t => t.CodigoTest == codigoTest);

                if (testEncontrado == null)
                {
                    return NotFound($"Prueba psicológica con código '{codigoTest}' no encontrada.");
                }

                return Ok(testEncontrado);
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
        /// Obtiene pruebas psicológicas filtradas por psicólogo
        /// </summary>
        /// <param name="codigoPsicologo">Código del psicólogo para filtrar</param>
        /// <returns>Lista de pruebas asignadas al psicólogo</returns>
        [HttpGet("Obtener-Por-Psicologo/{codigoPsicologo}")]
        public async Task<ActionResult<IEnumerable<PsicologoTestDto>>> ObtenerPsicologoTestPorPsicologo(string codigoPsicologo)
        {
            try
            {
                // Primero obtenemos todas las pruebas
                var response = await _httpClient.GetAsync($"{_baseUrl}/Listar-PsicologoTest");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, 
                        $"Error al consumir el API externa. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var psicologoTests = JsonSerializer.Deserialize<List<PsicologoTestDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Filtramos las pruebas por psicólogo
                var testsDelPsicologo = psicologoTests?.FindAll(t => t.CodigoPsicologo == codigoPsicologo);

                if (testsDelPsicologo == null || testsDelPsicologo.Count == 0)
                {
                    return NotFound($"No se encontraron pruebas para el psicólogo con código '{codigoPsicologo}'.");
                }

                return Ok(testsDelPsicologo);
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
