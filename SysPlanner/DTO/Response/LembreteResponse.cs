using SysPlanner.Infrastructure.Persistance.Enums;

namespace SysPlanner.DTO.Response
{
    public class LembreteResponse
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }
        public Prioridade Prioridade { get; set; }
        public Categoria Categoria { get; set; }
        public string Concluido { get; set; }
        public List<Link> Links { get; set; } = new();
    }
}
