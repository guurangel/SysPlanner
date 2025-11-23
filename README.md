# 🚀 SysPlanner

**SysPlanner** é uma aplicação desenvolvida em **ASP.NET Core Web API** para gerenciar lembretes criados pelos nossos usuários, oferecendo filtros personalizados, paginação e ordenação.

## 📌 Índice

- [🧾 Sobre o Projeto](#-sobre-o-projeto)
- [⚙️ Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [🧪 Como Executar](#-como-executar)
- [🧪 Testes](#-testes)
- [📌 Endpoints da API](#-endpoints-da-api)
- [✅ Funcionalidades](#-funcionalidades)
- [🗃️ Modelo de Dados](#-modelo-de-dados)
- [📚 Enums das Entidades](#-enums-das-entidades)
- [👨‍💻 Nossa equipe](#-nossa-equipe)

---

## 🧾 Sobre o Projeto

O objetivo do SysPlanner é fornecer uma **API RESTful robusta** para cadastro, listagem, e manutenção de lembretes de usuários, com:

- Validações e regras de negócio bem definidas.
- Versionamento de API.
- Health checks, Loggings e Tracing para monitoramento de serviços.

A aplicação segue boas práticas do **ASP.NET Core Web API**, utilizando **DTOs**, **Controllers**, **Services** e **Swagger** para documentação interativa.

---

## ⚙️ Tecnologias Utilizadas

- C#  
- .NET 6 ou superior  
- ASP.NET Core Web API  
- Entity Framework Core (EF Core)  
- Oracle Database  
- Oracle.EntityFrameworkCore  
- Swagger/OpenAPI  
- xUnit (para testes unitários e de integração)  

---

## 🧪 Como Executar

### Ambiente

- .NET SDK 7.0 ou superior  
- Oracle Database  
- Visual Studio 2022+ ou Visual Studio Code  
- dotnet ef  
- Postman ou outro programa de testes de API  

### Pacotes NuGet importantes

- Microsoft.EntityFrameworkCore  
- Microsoft.EntityFrameworkCore.Tools  
- Microsoft.EntityFrameworkCore.Design  
- Oracle.EntityFrameworkCore  
- Microsoft.AspNetCore.Mvc.Testing  
- xUnit  

### Passos

```bash
# Clone o repositório
git clone https://github.com/guurangel/SysPlanner.git

# Acesse a pasta do projeto
cd SysPlanner

# Configure a string de conexão Oracle dentro de appsettings.json
"ConnectionStrings": {
  "Oracle": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:PORTA/SEU_SERVICE_NAME"
}

# Gerar e aplicar as migrations
dotnet ef migrations add CreateDatabase --context SysPlannerDbContext
dotnet ef database update --context SysPlannerDbContext

# Executar a aplicação
dotnet run


```

A API estará disponível em:  
📍 `http://localhost:5033`

Acesse o Swagger para testar os endpoints:  
📘 `http://localhost:5033/swagger/index.html`

---

## 🧪 Testes

### Testes unitários

- Localizados no projeto `SysPlanner.Tests`.  
- Cobrem a lógica principal, como regras de negócios.
- Executar pelo Visual Studio Test Explorer ou linha de comando:

```bash
# Acesse a pasta do projeto de testes
cd SysPlanner.Tests

# Executar os testes
dotnet test
```

Testes de integração

Realizam chamadas HTTP para endpoints reais da API.

Utilizam WebApplicationFactory<Program> para simular o host da aplicação.

Endpoints testados: /api/Usuario

Observação: Não é necessário ter a API rodando separadamente; o WebApplicationFactory inicializa a aplicação internamente para testes.

---

## 📌 Endpoints da API

---

### 👤 Usuário

- `GET /api/v1/usuario` — Lista todos os usuários (paginação)
- `POST /api/v1/usuario` — Cadastra um novo usuário
- `GET /api/v1/usuario/{id}` — Busca por ID
- `PUT /api/v1/usuario/{id}` — Atualiza dados
- `DELETE /api/v1/usuario/{id}` — Remove um usuário

---

### 👤 Lembrete

- `POST /api/v1/lembrete` — Cadastra um novo lembrete
- `GET /api/v1/lembrete/{id}` — Busca por ID
- `PUT /api/v1/lembrete/{id}` — Atualiza dados
- `DELETE /api/v1/lembrete/{id}` — Remove um lembrete

---

**Paginação:**

- `pageNumber` — número da página (default: 1)
- `pageSize` — quantidade de registros por página (default: 10)

---

### 👤 Health Check

- `GET api/v1/health` — Verifica a saúde do serviço e conexão com o banco de dados.

---

### 🔹 Versionamento da API

Default: v1.

API versioning habilitado e relatado nos headers da resposta.

---

## ✅ Funcionalidades

- 🧱 Organização em camadas (Controllers, DTO, Infrastructure, Services).
- :file_cabinet: Utilizaçao de Migrations para criação da estrutura do banco de dados.
- 📖 Validações detalhadas com mensagens amigáveis.
- 📊 Documentação interativa via Swagger.
- 🧪 Testes unitários e de integração com xUnit.
- 📦 Paginação e ordenação nos endpoints.
- ⚡ Health checks para monitoramento da API.

---

## 🗃️ Modelo de Dados

---

### Usuario

```
Id: Guid
Nome: String
Email: String
Senha: String
Cpf: String
Endereco: Endereco
Lembrete: List
```

### Endereco

```
Rua: String
Numero: String
Complemento: String
Bairro: String
Cidade: String
Estado: Estado
Cep: String
```

### Lembrete

```
Id: Guid
Titulo: String
Descricao: String
Data: DateTime
Prioridade: Prioridade
Categoria: Categoria
Concluido: String ("S" ou "N")
UsuarioId: Guid
Usuario: Usuario
```

## 📚 Enums das Entidades

### `Estado`
Enum que representa os estados brasileiros.

```
AC, AL, AP, AM, BA, CE, DF, ES, GO, MA,
MT, MS, MG, PA, PB, PR, PE, PI, RJ, RN,
RS, RO, RR, SC, SP, SE, TO
```

### `Categoria`
Enum que representa as categorias de lembretes.

```
SAUDE, LAZER, FAMILIA, OUTROS
```

### `Prioridade`
Enum que classifica o nível de prioridade do lembrete.

```
ALTA, MODERADA, BAIXA
```

---

## 👨‍💻 Nossa equipe

**Gustavo Rangel**  
💼 Estudante de Análise e Desenvolvimento de Sistemas na FIAP  
🔗 [linkedin.com/in/gustavoorangel](https://www.linkedin.com/in/gustavoorangel)

**David Rapeckman**  
💼 Estudante de Análise e Desenvolvimento de Sistemas na FIAP  
🔗 [linkedin.com/in/davidrapeckman](https://www.linkedin.com/in/davidrapeckman)

**Luis Felippe Morais**  
💼 Estudante de Análise e Desenvolvimento de Sistemas na FIAP  
🔗 [linkedin.com/in/luis-felippe-morais-das-neves-16219b2b9](https://www.linkedin.com/in/luis-felippe-morais-das-neves-16219b2b9)
