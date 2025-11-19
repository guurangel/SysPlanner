namespace SysPlanner.DTO.Response
{
    public class UsuarioDetalhadoResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;

        public EnderecoResponse Endereco { get; set; } = null!;

        public List<LembreteResponse> Lembretes { get; set; } = new();
        public List<Link> Links { get; set; } = new();
    }
}
