using System.ComponentModel.DataAnnotations;

namespace SistemaPetrobras.Models
{
    public class Morador
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(14)]
        public string CPF { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(15)]
        public string? Telefone { get; set; }
        
        [Required(ErrorMessage = "Região é obrigatória")]
        [StringLength(50)]
        public string Regiao { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Escolaridade é obrigatória")]
        [StringLength(50)]
        public string Escolaridade { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Endereco { get; set; }
        
        public decimal? RendaFamiliar { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public string? UserId { get; set; }
    }
}

