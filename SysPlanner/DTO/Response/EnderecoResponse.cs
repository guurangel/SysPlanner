using SysPlanner.Infrastructure.Persistance.Enums;

namespace SysPlanner.DTO.Response
{
    public class EnderecoResponse
    {
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public Estado Estado { get; set; }
        public string Cep { get; set; } = string.Empty;
    }
}
