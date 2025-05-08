using Microsoft.EntityFrameworkCore.Migrations;

namespace MinhaCarteira.API.Migrations;

public partial class UpdateUserIdToGuid : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    // Primeiro, adicionamos a nova coluna IdGuid
    migrationBuilder.AddColumn<Guid>(
        name: "IdGuid",
        table: "Users",
        type: "uuid",
        nullable: false,
        defaultValue: Guid.NewGuid());

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Users",
        table: "Users");

    // Removemos a coluna Id antiga
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Users");

    // Renomeamos a coluna IdGuid para Id
    migrationBuilder.RenameColumn(
        name: "IdGuid",
        table: "Users",
        newName: "Id");

    // Adicionamos a nova chave primária
    migrationBuilder.AddPrimaryKey(
        name: "PK_Users",
        table: "Users",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets");

    migrationBuilder.AlterColumn<Guid>(
        name: "UserId",
        table: "Wallets",
        type: "uuid",
        nullable: false);

    migrationBuilder.AddForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    // Primeiro, adicionamos a coluna Id antiga
    migrationBuilder.AddColumn<int>(
        name: "Id",
        table: "Users",
        type: "integer",
        nullable: false)
        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Users",
        table: "Users");

    // Removemos a coluna IdGuid
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Users");

    // Adicionamos a chave primária antiga
    migrationBuilder.AddPrimaryKey(
        name: "PK_Users",
        table: "Users",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets");

    migrationBuilder.AlterColumn<int>(
        name: "UserId",
        table: "Wallets",
        type: "integer",
        nullable: false);

    migrationBuilder.AddForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
  }
}