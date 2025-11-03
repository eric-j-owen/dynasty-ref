using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Db.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:data_source", "keep_trade_cut,fantasy_calc,sleeper,dynasty_process,reddit")
                .Annotation("Npgsql:Enum:included_position", "qb,wr,rb,te")
                .Annotation("Npgsql:Enum:team_abbr", "buf,mia,ne,nyj,dal,nyg,phi,was,bal,cin,cle,pit,chi,det,gb,min,hou,ind,jax,ten,atl,car,no,tb,den,kc,lv,lac,ari,lar,sf,sea,null_team");

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    positions = table.Column<int[]>(type: "integer[]", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "external_id_player_lookup",
                columns: table => new
                {
                    data_source = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<string>(type: "text", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_id_player_lookup", x => new { x.data_source, x.source_id });
                    table.ForeignKey(
                        name: "FK_external_id_player_lookup_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_source = table.Column<int>(type: "integer", nullable: false),
                    is_super_flex = table.Column<bool>(type: "boolean", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    player_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_values_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_external_id_player_lookup_player_id",
                table: "external_id_player_lookup",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_values_player_id",
                table: "player_values",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_id_player_lookup");

            migrationBuilder.DropTable(
                name: "player_values");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
