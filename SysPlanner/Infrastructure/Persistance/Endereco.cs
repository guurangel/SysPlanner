using System.ComponentModel.DataAnnotations;
using SysPlanner.Infrastructure.Persistance.Enums;

namespace SysPlanner.Infrastructure.Persistance
{
    public class Endereco
    {
        [Required(ErrorMessage = "A rua é obrigatória")]
        [StringLength(150)]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório")]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(60)]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O bairro é obrigatório")]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória")]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório")]
        public Estado Estado { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve ter 8 dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "O CEP deve conter apenas números")]
        public string Cep { get; set; } = string.Empty;
    }
}
