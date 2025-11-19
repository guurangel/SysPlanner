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
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;
        private static readonly ActivitySource Activity = new ActivitySource("SysPlanner.Usuarios");

        public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // ================================================================
        // GET ALL PAGINADO
        // ================================================================
        [HttpGet]
        public async Task<ActionResult<PagedResponse<UsuarioResponse>>> GetAll(int page = 1, int pageSize = 10)
        {
            using var activity = Activity.StartActivity("GetAllUsuarios");
            _logger.LogInformation("Listando usuários página {Page} com tamanho {PageSize}", page, pageSize);

            var (usuarios, total) = await _usuarioService.ListarTodosAsync(page, pageSize);

            var dtoList = usuarios.Select(u => new UsuarioResponse
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Cpf = u.Cpf,
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Usuario", new { id = u.Id })!, Rel = "self", Method = "GET" },
                    new Link { Href = Url.Action(nameof(Update), "Usuario", new { id = u.Id })!, Rel = "update", Method = "PUT" },
                    new Link { Href = Url.Action(nameof(Delete), "Usuario", new { id = u.Id })!, Rel = "delete", Method = "DELETE" }
                }
            }).ToList();

            var response = new PagedResponse<UsuarioResponse>(dtoList, page, pageSize, total);

            activity?.SetTag("usuarios.total", total);
            return Ok(response);
        }

        // ================================================================
        // GET POR ID (DETALHADO)
        // ================================================================
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UsuarioDetalhadoResponse>> GetById(Guid id)
        {
            using var activity = Activity.StartActivity("GetUsuarioById");
            activity?.SetTag("usuario.id", id);
            _logger.LogInformation("Buscando usuário com Id {UsuarioId}", id);

            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário {UsuarioId} não encontrado", id);
                activity?.SetStatus(ActivityStatusCode.Error, "Usuário não encontrado");
                return NotFound();
            }

            var dto = new UsuarioDetalhadoResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Cpf = usuario.Cpf,
                Endereco = usuario.Endereco == null ? null! : new EnderecoResponse
                {
                    Rua = usuario.Endereco.Rua,
                    Numero = usuario.Endereco.Numero,
                    Complemento = usuario.Endereco.Complemento,
                    Bairro = usuario.Endereco.Bairro,
                    Cidade = usuario.Endereco.Cidade,
                    Estado = usuario.Endereco.Estado,
                    Cep = usuario.Endereco.Cep
                },
                Lembretes = usuario.Lembretes.Select(l => new LembreteResponse
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
                        new Link { Href = Url.Action("GetById", "Lembrete", new { id = l.Id })!, Rel = "self", Method = "GET" }
                    }
                }).ToList(),
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Usuario", new { id })!, Rel = "self", Method = "GET" },
                    new Link { Href = Url.Action(nameof(Update), "Usuario", new { id })!, Rel = "update", Method = "PUT" },
                    new Link { Href = Url.Action(nameof(Delete), "Usuario", new { id })!, Rel = "delete", Method = "DELETE" }
                }
            };

            activity?.SetStatus(ActivityStatusCode.Ok);
            return Ok(dto);
        }

        // ================================================================
        // POST
        // ================================================================
        [HttpPost]
        public async Task<ActionResult<UsuarioResponse>> Create(CreateUsuarioRequest dto)
        {
            using var activity = Activity.StartActivity("CreateUsuario");
            _logger.LogInformation("Criando usuário {Nome}", dto.Nome);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Cpf = dto.Cpf,
                Endereco = new Endereco
                {
                    Rua = dto.Endereco.Rua,
                    Numero = dto.Endereco.Numero,
                    Complemento = dto.Endereco.Complemento,
                    Bairro = dto.Endereco.Bairro,
                    Cidade = dto.Endereco.Cidade,
                    Estado = dto.Endereco.Estado,
                    Cep = dto.Endereco.Cep
                }
            };

            usuario = await _usuarioService.CriarAsync(usuario);

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Cpf = usuario.Cpf,
                Links = new List<Link>
                {
                    new Link { Href = Url.Action(nameof(GetById), "Usuario", new { id = usuario.Id })!, Rel = "self", Method = "GET" }
                }
            };

            activity?.SetTag("usuario.id", usuario.Id);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, response);
        }

        // ================================================================
        // PUT
        // ================================================================
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, UpdateUsuarioRequest dto)
        {
            using var activity = Activity.StartActivity("UpdateUsuario");
            _logger.LogInformation("Atualizando usuário {UsuarioId} com nome {Nome}", id, dto.Nome);
            activity?.SetTag("usuario.id", id);

            var usuarioAtualizado = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Endereco = dto.Endereco == null ? null! : new Endereco
                {
                    Rua = dto.Endereco.Rua,
                    Numero = dto.Endereco.Numero,
                    Complemento = dto.Endereco.Complemento,
                    Bairro = dto.Endereco.Bairro,
                    Cidade = dto.Endereco.Cidade,
                    Estado = dto.Endereco.Estado,
                    Cep = dto.Endereco.Cep
                }
            };

            var sucesso = await _usuarioService.AtualizarAsync(id, usuarioAtualizado);
            activity?.SetStatus(ActivityStatusCode.Ok);
            return NoContent();
        }

        // ================================================================
        // DELETE
        // ================================================================
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            using var activity = Activity.StartActivity("DeleteUsuario");
            _logger.LogInformation("Deletando usuário {UsuarioId}", id);
            activity?.SetTag("usuario.id", id);

            var deleted = await _usuarioService.DeletarAsync(id);
            activity?.SetStatus(deleted ? ActivityStatusCode.Ok : ActivityStatusCode.Error);
            return deleted ? NoContent() : NotFound();
        }
    }
}
