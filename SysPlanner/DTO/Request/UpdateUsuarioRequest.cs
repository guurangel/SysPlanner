using System.ComponentModel.DataAnnotations;

namespace SysPlanner.DTO.Request
{
    public class UpdateUsuarioRequest
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        public EnderecoRequest? Endereco { get; set; }
    }
}
