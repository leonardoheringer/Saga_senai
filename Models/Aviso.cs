using System.ComponentModel.DataAnnotations;

namespace SistemaPetrobras.Models
{
    public class Aviso
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Conteúdo é obrigatório")]
        public string Conteudo { get; set; } = string.Empty;
        
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        
        public bool Ativo { get; set; } = true;
        
        public string? AutorId { get; set; }
    }
}

