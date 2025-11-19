using SysPlanner.DTO.Request;
using SysPlanner.DTO.Response;
using SysPlanner.Infrastructure.Persistance.Enums;
using System.Net.Http.Json;
using Xunit;

public class UsuarioControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsuarioControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarUsuario_DeveRetornar201()
    {
        var request = new CreateUsuarioRequest
        {
            Nome = "Teste",
            Email = "teste@email.com",
            Senha = "123456",
            Cpf = "12345678900",
            Endereco = new SysPlanner.DTO.Request.EnderecoRequest
            {
                Rua = "Rua Teste",
                Numero = "123",
                Bairro = "Bairro Teste",
                Cidade = "Cidade Teste",
                Estado = Estado.SP,
                Cep = "12345678",
                Complemento = ""
            }
        };

        var response = await _client.PostAsJsonAsync("/api/v1/usuario", request);

        response.EnsureSuccessStatusCode(); // Status 201
        var usuarioResponse = await response.Content.ReadFromJsonAsync<UsuarioResponse>();

        Assert.NotNull(usuarioResponse);
        Assert.Equal("Teste", usuarioResponse!.Nome);
    }
}
