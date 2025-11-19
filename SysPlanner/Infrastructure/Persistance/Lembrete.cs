using SysPlanner.Infrastructure.Persistance.Enums;
using System.ComponentModel.DataAnnotations;
using System.IO;
using SysPlanner.Infrastructure.Persistance;

namespace SysPlanner.Infrastructure.Persistance
{
    public class Lembrete
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A prioridade é obrigatória")]
        public Prioridade Prioridade { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public Categoria Categoria { get; set; }

        public string Concluido { get; set; } = "N";

        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        public Usuario Usuario { get; set; } = null!;
    }
}
