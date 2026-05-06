using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDocumental.Dominio
{
    public class SolicitudDocumentacion
    {
        [Key]
        public int SolicitudId { get; set; }
        public string CodigoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string CodigoPaciente { get; set; }
        public string DepartamentoSolicitante { get; set; }
        public string CodigoMedico { get; set; }
        public string Estado { get; set; }

        public int? MedicoDocMedicoId { get; set; }
        public int? PacienteDocPacienteId { get; set; }
    }
}
