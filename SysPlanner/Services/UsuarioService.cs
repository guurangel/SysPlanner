using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SysPlanner.Infrastructure.Contexts;
using SysPlanner.Infrastructure.Persistance;
using SysPlanner.Services.Interfaces;
using System.Diagnostics;

namespace SysPlanner.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly SysPlannerDbContext _context;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(SysPlannerDbContext context, ILogger<UsuarioService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<Usuario> Usuarios, int TotalItems)> ListarTodosAsync(int page, int pageSize)
        {
            using var activity = new Activity(nameof(ListarTodosAsync)).Start();
            _logger.LogInformation("Listando usuários, página {Page}, tamanho {PageSize}", page, pageSize);

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Usuarios.AsNoTracking().OrderBy(u => u.Nome);

            int total = await query.CountAsync();

            var usuarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.Lembretes)
                .ToListAsync();

            _logger.LogInformation("Foram retornados {Count} usuários de {Total}", usuarios.Count, total);

            return (usuarios, total);
        }

        public async Task<Usuario> CriarAsync(Usuario usuario)
        {
            using var activity = new Activity(nameof(CriarAsync)).Start();
            _logger.LogInformation("Criando usuário {Email}", usuario.Email);

            usuario.Nome = usuario.Nome.Trim();
            usuario.Email = usuario.Email.Trim().ToLower();
            usuario.Cpf = usuario.Cpf.Trim();

            var emailUsado = await _context.Usuarios.CountAsync(u => u.Email == usuario.Email) > 0;
            if (emailUsado)
                throw new Exception("Email já está em uso.");

            var cpfUsado = await _context.Usuarios.CountAsync(u => u.Cpf == usuario.Cpf) > 0;
            if (cpfUsado)
                throw new Exception("CPF já está registrado.");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário criado com sucesso, Id: {Id}", usuario.Id);

            return usuario;
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            using var activity = new Activity(nameof(ObterPorIdAsync)).Start();
            _logger.LogInformation("Buscando usuário pelo Id: {Id}", id);

            return await _context.Usuarios
                .Include(u => u.Lembretes)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> AtualizarAsync(Guid id, Usuario usuarioAtualizado)
        {
            using var activity = new Activity(nameof(AtualizarAsync)).Start();
            _logger.LogInformation("Atualizando usuário Id: {Id}", id);

            var usuarioDB = await ObterPorIdAsync(id)
                ?? throw new Exception("Usuário não encontrado.");

            usuarioAtualizado.Nome = usuarioAtualizado.Nome.Trim();
            usuarioAtualizado.Email = usuarioAtualizado.Email.Trim().ToLower();

            var emailUsado = await _context.Usuarios
                .CountAsync(u => u.Email == usuarioAtualizado.Email && u.Id != id) > 0;
            if (emailUsado)
                throw new Exception("O novo email já está em uso por outro usuário.");

            usuarioDB.Nome = usuarioAtualizado.Nome;
            usuarioDB.Email = usuarioAtualizado.Email;
            usuarioDB.Endereco = usuarioAtualizado.Endereco;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário Id {Id} atualizado com sucesso", id);

            return usuarioDB;
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            using var activity = new Activity(nameof(DeletarAsync)).Start();
            _logger.LogInformation("Deletando usuário Id: {Id}", id);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
                return false;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuário Id {Id} deletado com sucesso", id);

            return true;
        }
    }
}

