using Microsoft.EntityFrameworkCore.Migrations;

namespace MinhaCarteira.API.Migrations;

public partial class UpdateWalletIdToGuid : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    // Primeiro, adicionamos a nova coluna IdGuid
    migrationBuilder.AddColumn<Guid>(
        name: "IdGuid",
        table: "Wallets",
        type: "uuid",
        nullable: false,
        defaultValue: Guid.NewGuid());

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Wallets",
        table: "Wallets");

    // Removemos a coluna Id antiga
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Wallets");

    // Renomeamos a coluna IdGuid para Id
    migrationBuilder.RenameColumn(
        name: "IdGuid",
        table: "Wallets",
        newName: "Id");

    // Adicionamos a nova chave primária
    migrationBuilder.AddPrimaryKey(
        name: "PK_Wallets",
        table: "Wallets",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions");

    migrationBuilder.AddForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);

    migrationBuilder.AddForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions",
        column: "DestinationWalletId",
        principalTable: "Wallets",
        principalColumn: "Id");

    migrationBuilder.AddForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions",
        column: "WalletId",
        principalTable: "Wallets",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    // Primeiro, adicionamos a coluna Id antiga
    migrationBuilder.AddColumn<int>(
        name: "Id",
        table: "Wallets",
        type: "integer",
        nullable: false)
        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Wallets",
        table: "Wallets");

    // Removemos a coluna IdGuid
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Wallets");

    // Adicionamos a chave primária antiga
    migrationBuilder.AddPrimaryKey(
        name: "PK_Wallets",
        table: "Wallets",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions");

    migrationBuilder.AddForeignKey(
        name: "FK_Wallets_Users_UserId",
        table: "Wallets",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);

    migrationBuilder.AddForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions",
        column: "DestinationWalletId",
        principalTable: "Wallets",
        principalColumn: "Id");

    migrationBuilder.AddForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions",
        column: "WalletId",
        principalTable: "Wallets",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
  }
}