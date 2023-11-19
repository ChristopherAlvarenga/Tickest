using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickest.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTicketStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Título",
                table: "Tickets",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "Descrição",
                table: "Tickets",
                newName: "Descricao");

            migrationBuilder.RenameColumn(
                name: "Data_Limite",
                table: "Tickets",
                newName: "DataLimite");

            migrationBuilder.RenameColumn(
                name: "Data_Criação",
                table: "Tickets",
                newName: "DataCriacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "Tickets",
                newName: "Título");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "Tickets",
                newName: "Descrição");

            migrationBuilder.RenameColumn(
                name: "DataLimite",
                table: "Tickets",
                newName: "Data_Limite");

            migrationBuilder.RenameColumn(
                name: "DataCriacao",
                table: "Tickets",
                newName: "Data_Criação");
        }
    }
}
