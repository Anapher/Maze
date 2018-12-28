using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Maze.Server.Data.Migrations
{
    public partial class AddGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    ClientGroupId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.ClientGroupId);
                });

            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    ClientConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientGroupId = table.Column<int>(nullable: true),
                    Content = table.Column<string>(nullable: false),
                    ContentHash = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfiguration", x => x.ClientConfigurationId);
                    table.ForeignKey(
                        name: "FK_ClientConfiguration_Groups_ClientGroupId",
                        column: x => x.ClientGroupId,
                        principalTable: "Groups",
                        principalColumn: "ClientGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientGroupMembership",
                columns: table => new
                {
                    ClientId = table.Column<int>(nullable: false),
                    ClientGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGroupMembership", x => new { x.ClientId, x.ClientGroupId });
                    table.ForeignKey(
                        name: "FK_ClientGroupMembership_Groups_ClientGroupId",
                        column: x => x.ClientGroupId,
                        principalTable: "Groups",
                        principalColumn: "ClientGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientGroupMembership_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientConfiguration_ClientGroupId",
                table: "ClientConfiguration",
                column: "ClientGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientGroupMembership_ClientGroupId",
                table: "ClientGroupMembership",
                column: "ClientGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientConfiguration");

            migrationBuilder.DropTable(
                name: "ClientGroupMembership");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
