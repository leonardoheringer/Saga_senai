using System.ComponentModel.DataAnnotations;

namespace SistemaPetrobras.Models
{
    public enum StatusFeedback
    {
        Pendente,
        Aprovado,
        Rejeitado
    }
    
    public class Feedback
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mensagem é obrigatória")]
        public string Mensagem { get; set; } = string.Empty;
        
        public DateTime DataEnvio { get; set; } = DateTime.Now;
        
        public StatusFeedback Status { get; set; } = StatusFeedback.Pendente;
        
        public string? Resposta { get; set; }
        
        public DateTime? DataResposta { get; set; }
        
        public string? MoradorId { get; set; }
        
        public string? AdminId { get; set; }
    }
}

