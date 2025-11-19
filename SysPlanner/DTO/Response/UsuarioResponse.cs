namespace SysPlanner.DTO.Response
{
    public class UsuarioResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public List<Link> Links { get; set; } = new();
    }
}
