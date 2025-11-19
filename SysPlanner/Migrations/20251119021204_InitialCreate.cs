using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SysPlanner.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: false),
                    Endereco_Rua = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Endereco_Numero = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Endereco_Complemento = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: true),
                    Endereco_Bairro = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Endereco_Estado = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    Endereco_Cep = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lembretes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Prioridade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Categoria = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Concluido = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, defaultValue: "N"),
                    UsuarioId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lembretes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lembretes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lembretes_UsuarioId",
                table: "Lembretes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cpf",
                table: "Usuarios",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lembretes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
