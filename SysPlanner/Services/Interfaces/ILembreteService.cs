using SysPlanner.Infrastructure.Persistance;

namespace SysPlanner.Services.Interfaces
{
    public interface ILembreteService
    {
        Task<(IEnumerable<Lembrete> Lembretes, int TotalItems)> ListarTodosAsync(int page, int pageSize);
        Task<Lembrete> CriarAsync(Lembrete lembrete);
        Task<Lembrete> AtualizarAsync(Guid id, Lembrete lembreteAtualizado);
        Task<bool> DeletarAsync(Guid id);
        Task<Lembrete?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Lembrete>> ListarPorUsuarioAsync(Guid usuarioId);
    }
}
