using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SysPlanner.Infrastructure.Contexts;
using SysPlanner.Infrastructure.Persistance;
using SysPlanner.Services.Interfaces;
using System.Diagnostics;

namespace SysPlanner.Services
{
    public class LembreteService : ILembreteService
    {
        private readonly SysPlannerDbContext _context;
        private readonly ILogger<LembreteService> _logger;

        public LembreteService(SysPlannerDbContext context, ILogger<LembreteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<Lembrete> Lembretes, int TotalItems)> ListarTodosAsync(int page, int pageSize)
        {
            using var activity = new Activity(nameof(ListarTodosAsync)).Start();
            _logger.LogInformation("Listando lembretes, página {Page}, tamanho {PageSize}", page, pageSize);

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Lembretes.AsNoTracking().OrderBy(l => l.Data);

            int total = await query.CountAsync();

            var lembretes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.Usuario)
                .ToListAsync();

            _logger.LogInformation("Foram retornados {Count} lembretes de {Total}", lembretes.Count, total);

            return (lembretes, total);
        }

        public async Task<Lembrete> CriarAsync(Lembrete lembrete)
        {
            using var activity = new Activity(nameof(CriarAsync)).Start();
            _logger.LogInformation("Criando lembrete para o usuário {UsuarioId}: {Titulo}", lembrete.UsuarioId, lembrete.Titulo);

            lembrete.Titulo = lembrete.Titulo.Trim();
            if (lembrete.Data.Date < DateTime.Today)
                throw new Exception("A data do lembrete não pode estar no passado.");

            bool tituloDuplicado = await _context.Lembretes
                .CountAsync(l => l.UsuarioId == lembrete.UsuarioId && l.Titulo.ToLower() == lembrete.Titulo.ToLower()) > 0;

            if (tituloDuplicado)
                throw new Exception("O usuário já possui um lembrete com esse título.");

            _context.Lembretes.Add(lembrete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Lembrete criado com sucesso, Id: {Id}", lembrete.Id);

            return lembrete;
        }

        public async Task<Lembrete?> ObterPorIdAsync(Guid id)
        {
            using var activity = new Activity(nameof(ObterPorIdAsync)).Start();
            _logger.LogInformation("Buscando lembrete pelo Id: {Id}", id);

            return await _context.Lembretes
                .Include(l => l.Usuario)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lembrete> AtualizarAsync(Guid id, Lembrete lembreteAtualizado)
        {
            using var activity = new Activity(nameof(AtualizarAsync)).Start();
            _logger.LogInformation("Atualizando lembrete Id: {Id}", id);

            var lembreteDB = await ObterPorIdAsync(id)
                ?? throw new Exception("Lembrete não encontrado.");

            lembreteAtualizado.Titulo = lembreteAtualizado.Titulo.Trim();

            bool tituloDuplicado = await _context.Lembretes
                .CountAsync(l => l.UsuarioId == lembreteDB.UsuarioId && l.Id != id && l.Titulo.ToLower() == lembreteAtualizado.Titulo.ToLower()) > 0;

            if (tituloDuplicado)
                throw new Exception("Já existe outro lembrete com esse título para o usuário.");

            if (lembreteAtualizado.Data.Date < DateTime.Today)
                throw new Exception("Data inválida.");

            lembreteDB.Titulo = lembreteAtualizado.Titulo;
            lembreteDB.Descricao = lembreteAtualizado.Descricao;
            lembreteDB.Data = lembreteAtualizado.Data;
            lembreteDB.Prioridade = lembreteAtualizado.Prioridade;
            lembreteDB.Categoria = lembreteAtualizado.Categoria;
            lembreteDB.Concluido = lembreteAtualizado.Concluido;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Lembrete Id {Id} atualizado com sucesso", id);

            return lembreteDB;
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            using var activity = new Activity(nameof(DeletarAsync)).Start();
            _logger.LogInformation("Deletando lembrete Id: {Id}", id);

            var lembrete = await _context.Lembretes.FirstOrDefaultAsync(l => l.Id == id);
            if (lembrete == null)
                return false;

            _context.Lembretes.Remove(lembrete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Lembrete Id {Id} deletado com sucesso", id);

            return true;
        }

        public async Task<IEnumerable<Lembrete>> ListarPorUsuarioAsync(Guid usuarioId)
        {
            using var activity = new Activity(nameof(ListarPorUsuarioAsync)).Start();
            _logger.LogInformation("Listando lembretes do usuário {UsuarioId}", usuarioId);

            return await _context.Lembretes
                .Where(l => l.UsuarioId == usuarioId)
                .OrderBy(l => l.Data)
                .ToListAsync();
        }
    }
}

