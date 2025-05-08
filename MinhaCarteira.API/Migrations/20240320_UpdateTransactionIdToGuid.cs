using Microsoft.EntityFrameworkCore.Migrations;

namespace MinhaCarteira.API.Migrations;

public partial class UpdateTransactionIdToGuid : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    // Primeiro, adicionamos a nova coluna IdGuid
    migrationBuilder.AddColumn<Guid>(
        name: "IdGuid",
        table: "Transactions",
        type: "uuid",
        nullable: false,
        defaultValue: Guid.NewGuid());

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Transactions",
        table: "Transactions");

    // Removemos a coluna Id antiga
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Transactions");

    // Renomeamos a coluna IdGuid para Id
    migrationBuilder.RenameColumn(
        name: "IdGuid",
        table: "Transactions",
        newName: "Id");

    // Adicionamos a nova chave primária
    migrationBuilder.AddPrimaryKey(
        name: "PK_Transactions",
        table: "Transactions",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions");

    migrationBuilder.AlterColumn<Guid>(
        name: "DestinationWalletId",
        table: "Transactions",
        type: "uuid",
        nullable: true);

    migrationBuilder.AlterColumn<Guid>(
        name: "WalletId",
        table: "Transactions",
        type: "uuid",
        nullable: false);

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
        table: "Transactions",
        type: "integer",
        nullable: false)
        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

    // Removemos a chave primária atual
    migrationBuilder.DropPrimaryKey(
        name: "PK_Transactions",
        table: "Transactions");

    // Removemos a coluna IdGuid
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Transactions");

    // Adicionamos a chave primária antiga
    migrationBuilder.AddPrimaryKey(
        name: "PK_Transactions",
        table: "Transactions",
        column: "Id");

    // Atualizamos as chaves estrangeiras
    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_DestinationWalletId",
        table: "Transactions");

    migrationBuilder.DropForeignKey(
        name: "FK_Transactions_Wallets_WalletId",
        table: "Transactions");

    migrationBuilder.AlterColumn<int>(
        name: "DestinationWalletId",
        table: "Transactions",
        type: "integer",
        nullable: true);

    migrationBuilder.AlterColumn<int>(
        name: "WalletId",
        table: "Transactions",
        type: "integer",
        nullable: false);

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