using SysPlanner.Infrastructure.Persistance.Enums;
using System.ComponentModel.DataAnnotations;

namespace SysPlanner.DTO.Request
{
    public class UpdateLembreteRequest
    {
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        public DateTime Data { get; set; }

        [Required]
        public Prioridade Prioridade { get; set; }

        [Required]
        public Categoria Categoria { get; set; }

        public string Concluido { get; set; }
    }
}
