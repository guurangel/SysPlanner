using SysPlanner.Infrastructure.Persistance.Enums;
using System.ComponentModel.DataAnnotations;

namespace SysPlanner.DTO.Request
{
    public class EnderecoRequest
    {
        [Required]
        [StringLength(150)]
        public string Rua { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(60)]
        public string? Complemento { get; set; }

        [Required]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        public Estado Estado { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Cep { get; set; } = string.Empty;
    }
}
