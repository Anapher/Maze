using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Maze.Server.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(maxLength: 64, nullable: false),
                    Password = table.Column<string>(type: "char(60)", nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    TokenValidityPeriod = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: false),
                    OperatingSystem = table.Column<string>(nullable: true),
                    MacAddress = table.Column<string>(nullable: true),
                    SystemLanguage = table.Column<string>(nullable: true),
                    HardwareId = table.Column<string>(type: "char(64)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "ClientSession",
                columns: table => new
                {
                    ClientSessionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(nullable: false),
                    IsAdministrator = table.Column<bool>(nullable: false),
                    ClientVersion = table.Column<string>(nullable: true),
                    ClientPath = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSession", x => x.ClientSessionId);
                    table.ForeignKey(
                        name: "FK_ClientSession_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_HardwareId",
                table: "Clients",
                column: "HardwareId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientSession_ClientId",
                table: "ClientSession",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ClientSession");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
