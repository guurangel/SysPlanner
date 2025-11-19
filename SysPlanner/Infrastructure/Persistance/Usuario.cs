using SysPlanner.Infrastructure.Persistance;
using SysPlanner.Infrastructure.Persistance.Enums;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SysPlanner.Infrastructure.Persistance
{
    public class Usuario
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public Endereco Endereco { get; set; }

        public ICollection<Lembrete> Lembretes { get; set; } = new List<Lembrete>();

    }
}