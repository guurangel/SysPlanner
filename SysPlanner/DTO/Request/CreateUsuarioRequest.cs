using System.ComponentModel.DataAnnotations;

namespace SysPlanner.DTO.Request
{
    public class CreateUsuarioRequest
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        public EnderecoRequest Endereco { get; set; } = null!;
    }
}
