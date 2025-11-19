using Microsoft.AspNetCore.Mvc;
using SysPlanner.DTO.Request;
using SysPlanner.DTO.Response;
using SysPlanner.Infrastructure.Persistance;
using SysPlanner.Services.Interfaces;
using System.Diagnostics;

namespace SysPlanner.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class LembreteController : ControllerBase
    {
        private readonly ILembreteService _lembreteService;
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<LembreteController> _logger;
        private static readonly ActivitySource Activity = new ActivitySource("SysPlanner.Lembretes");

        public LembreteController(
            ILembreteService lembreteService,
            IUsuarioService usuarioService,
            ILogger<LembreteController> logger)
        {
            _lembreteService = lembreteService;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // ===================================================================
        // GET ALL PAGINADO
        // ===================================================================
        [HttpGet]
        public async Task<ActionResult<PagedResponse<LembreteResponse>>> GetAll(
            Guid usuarioId,
            int page = 1,
            int pageSize = 10)
        {
            using var activity = Activity.StartActivity("GetAllLembretes");
            _logger.LogInformation("Listando lembretes do usuário {UsuarioId}, página {Page}, tamanho {PageSize}", usuarioId, page, pageSize);
            activity?.SetTag("usuario.id", usuarioId);

            var lembretes = await _lembreteService.ListarPorUsuarioAsync(usuarioId);

            var total = lembretes.Count();
            var paginated = lembretes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = paginated.Select(l => new LembreteResponse
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Descricao = l.Descricao,
                Data = l.Data,
                Prioridade = l.Prioridade,
                Categoria = l.Categoria,
                Concluido = l.Concluido,
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Lembrete", new { id = l.Id })!, Rel = "self", Method = "GET" },
                    new Link { Href = Url.Action(nameof(Update), "Lembrete", new { id = l.Id })!, Rel = "update", Method = "PUT" },
                    new Link { Href = Url.Action(nameof(Delete), "Lembrete", new { id = l.Id })!, Rel = "delete", Method = "DELETE" }
                }
            }).ToList();

            var response = new PagedResponse<LembreteResponse>(dtoList, page, pageSize, total);
            activity?.SetTag("lembretes.total", total);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return Ok(response);
        }

        // ===================================================================
        // GET POR ID (DETALHADO)
        // ===================================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<LembreteDetalhadoResponse>> GetById(Guid id)
        {
            using var activity = Activity.StartActivity("GetLembreteById");
            _logger.LogInformation("Buscando lembrete {LembreteId}", id);
            activity?.SetTag("lembrete.id", id);

            var lembrete = await _lembreteService.ObterPorIdAsync(id);
            if (lembrete == null)
            {
                _logger.LogWarning("Lembrete {LembreteId} não encontrado", id);
                activity?.SetStatus(ActivityStatusCode.Error, "Lembrete não encontrado");
                return NotFound();
            }

            var usuario = await _usuarioService.ObterPorIdAsync(lembrete.UsuarioId);

            var dto = new LembreteDetalhadoResponse
            {
                Id = lembrete.Id,
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                Data = lembrete.Data,
                Prioridade = lembrete.Prioridade,
                Categoria = lembrete.Categoria,
                Concluido = lembrete.Concluido,
                Usuario = new UsuarioResponse
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Cpf = usuario.Cpf,
                    Links = new List<Link>
                    {
                        new Link { Href = Url.Action("GetById", "Usuario", new { id = usuario.Id })!, Rel = "self", Method = "GET" }
                    }
                },
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Lembrete", new { id = lembrete.Id })!, Rel = "self", Method = "GET" },
                    new Link { Href = Url.Action(nameof(Update), "Lembrete", new { id = lembrete.Id })!, Rel = "update", Method = "PUT" },
                    new Link { Href = Url.Action(nameof(Delete), "Lembrete", new { id = lembrete.Id })!, Rel = "delete", Method = "DELETE" }
                }
            };

            activity?.SetStatus(ActivityStatusCode.Ok);
            return Ok(dto);
        }

        // ===================================================================
        // POST
        // ===================================================================
        [HttpPost]
        public async Task<ActionResult<LembreteResponse>> Create(CreateLembreteRequest dto)
        {
            using var activity = Activity.StartActivity("CreateLembrete");
            _logger.LogInformation("Criando lembrete para usuário {UsuarioId} com título {Titulo}", dto.UsuarioId, dto.Titulo);
            activity?.SetTag("usuario.id", dto.UsuarioId);

            var usuario = await _usuarioService.ObterPorIdAsync(dto.UsuarioId);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário {UsuarioId} não encontrado para criar lembrete", dto.UsuarioId);
                activity?.SetStatus(ActivityStatusCode.Error, "Usuário não encontrado");
                return BadRequest("Usuário não encontrado.");
            }

            var lembrete = new Lembrete
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Data = dto.Data,
                Prioridade = dto.Prioridade,
                Categoria = dto.Categoria,
                UsuarioId = dto.UsuarioId
            };

            lembrete = await _lembreteService.CriarAsync(lembrete);

            var response = new LembreteResponse
            {
                Id = lembrete.Id,
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                Data = lembrete.Data,
                Prioridade = lembrete.Prioridade,
                Categoria = lembrete.Categoria,
                Concluido = lembrete.Concluido,
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Lembrete", new { id = lembrete.Id })!, Rel = "self", Method = "GET" }
                }
            };

            activity?.SetTag("lembrete.id", lembrete.Id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return CreatedAtAction(nameof(GetById), new { id = lembrete.Id }, response);
        }

        // ===================================================================
        // PUT
        // ===================================================================
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, UpdateLembreteRequest dto)
        {
            using var activity = Activity.StartActivity("UpdateLembrete");
            _logger.LogInformation("Atualizando lembrete {LembreteId} com título {Titulo}", id, dto.Titulo);
            activity?.SetTag("lembrete.id", id);

            var lembreteAtualizado = new Lembrete
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Data = dto.Data,
                Prioridade = dto.Prioridade,
                Categoria = dto.Categoria,
                Concluido = dto.Concluido
            };

            var ok = await _lembreteService.AtualizarAsync(id, lembreteAtualizado);

            activity?.SetStatus(ActivityStatusCode.Ok);
            return NoContent();
        }

        // ===================================================================
        // DELETE
        // ===================================================================
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            using var activity = Activity.StartActivity("DeleteLembrete");
            _logger.LogInformation("Deletando lembrete {LembreteId}", id);
            activity?.SetTag("lembrete.id", id);

            var deleted = await _lembreteService.DeletarAsync(id);

            activity?.SetStatus(deleted ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
            return deleted ? NoContent() : NotFound();
        }
    }
}
