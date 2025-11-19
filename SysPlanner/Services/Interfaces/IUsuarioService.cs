using SysPlanner.Infrastructure.Persistance;

namespace SysPlanner.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<(IEnumerable<Usuario> Usuarios, int TotalItems)> ListarTodosAsync(int page, int pageSize);
        Task<Usuario> CriarAsync(Usuario usuario);
        Task<Usuario> AtualizarAsync(Guid id, Usuario usuarioAtualizado);
        Task<bool> DeletarAsync(Guid id);
        Task<Usuario?> ObterPorIdAsync(Guid id);
    }
}
