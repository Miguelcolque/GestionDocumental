namespace GestionDocumental.DTOs.CreateRequests
{
    public class PacienteDocCreateRequest
    {
        public string CodigoPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string Estado { get; set; }
        
        // Campo para saber si ya fue atendido
        public bool FueAtendido { get; set; }
    }
}
