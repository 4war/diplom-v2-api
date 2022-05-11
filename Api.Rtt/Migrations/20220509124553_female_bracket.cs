using System;
using System.IO;
using Api.Rtt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using MySql.EntityFrameworkCore.Metadata;

namespace Api.Rtt.Migrations
{
    public partial class female_bracket : Migration, IDesignTimeDbContextFactory<ApiContext>
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "city",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(767)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_city", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "player",
                columns: table => new
                {
                    Rni = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    Point = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.Rni);
                });

            migrationBuilder.CreateTable(
                name: "tennis_center",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tennis_center", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "court",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surface = table.Column<string>(type: "text", nullable: false),
                    Opened = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TennisCenterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_court", x => x.Id);
                    table.ForeignKey(
                        name: "FK_court_tennis_center_TennisCenterId",
                        column: x => x.TennisCenterId,
                        principalTable: "tennis_center",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tournament_factory",
                columns: table => new
                {
                    FirstTournamentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Ages = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    HasQualification = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NumberOfQualificationWinners = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateRequest = table.Column<DateTime>(type: "datetime", nullable: false),
                    NetRange = table.Column<int>(type: "int", nullable: false),
                    TennisCenterId = table.Column<int>(type: "int", nullable: false),
                    Genders = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournament_factory", x => x.FirstTournamentId);
                    table.ForeignKey(
                        name: "FK_tournament_factory_tennis_center_TennisCenterId",
                        column: x => x.TennisCenterId,
                        principalTable: "tennis_center",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schedule",
                columns: table => new
                {
                    Day = table.Column<DateTime>(type: "datetime", nullable: false),
                    FactoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule", x => new { x.FactoryId, x.Day });
                    table.ForeignKey(
                        name: "FK_schedule_tournament_factory_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "tournament_factory",
                        principalColumn: "FirstTournamentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateRequest = table.Column<DateTime>(type: "datetime", nullable: false),
                    NetRange = table.Column<int>(type: "int", nullable: false),
                    QualificationId = table.Column<int>(type: "int", nullable: true),
                    NumberOfQualificationWinners = table.Column<int>(type: "int", nullable: false),
                    FactoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournament", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tournament_tournament_factory_FactoryId",
                        column: x => x.FactoryId,
                        principalTable: "tournament_factory",
                        principalColumn: "FirstTournamentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tournament_tournament_QualificationId",
                        column: x => x.QualificationId,
                        principalTable: "tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bracket",
                columns: table => new
                {
                    TournamentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bracket", x => x.TournamentId);
                    table.ForeignKey(
                        name: "FK_bracket_tournament_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTournament",
                columns: table => new
                {
                    PlayersRni = table.Column<int>(type: "int", nullable: false),
                    TournamentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTournament", x => new { x.PlayersRni, x.TournamentsId });
                    table.ForeignKey(
                        name: "FK_PlayerTournament_player_PlayersRni",
                        column: x => x.PlayersRni,
                        principalTable: "player",
                        principalColumn: "Rni",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerTournament_tournament_TournamentsId",
                        column: x => x.TournamentsId,
                        principalTable: "tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    BracketId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rounds_bracket_BracketId",
                        column: x => x.BracketId,
                        principalTable: "bracket",
                        principalColumn: "TournamentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Player1Rni = table.Column<int>(type: "int", nullable: true),
                    Player2Rni = table.Column<int>(type: "int", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime", nullable: true),
                    End = table.Column<DateTime>(type: "datetime", nullable: true),
                    WinnerRni = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<string>(type: "text", nullable: true),
                    PlaceInRound = table.Column<int>(type: "int", nullable: false),
                    RoundId = table.Column<int>(type: "int", nullable: false),
                    CourtId = table.Column<int>(type: "int", nullable: true),
                    OrderInSchedule = table.Column<int>(type: "int", nullable: true),
                    ScheduleDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    ScheduleFactoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_match_court_CourtId",
                        column: x => x.CourtId,
                        principalTable: "court",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_match_player_Player1Rni",
                        column: x => x.Player1Rni,
                        principalTable: "player",
                        principalColumn: "Rni",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_match_player_Player2Rni",
                        column: x => x.Player2Rni,
                        principalTable: "player",
                        principalColumn: "Rni",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_match_player_WinnerRni",
                        column: x => x.WinnerRni,
                        principalTable: "player",
                        principalColumn: "Rni",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_match_rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_schedule_ScheduleFactoryId_ScheduleDay",
                        columns: x => new { x.ScheduleFactoryId, x.ScheduleDay },
                        principalTable: "schedule",
                        principalColumns: new[] { "FactoryId", "Day" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_court_TennisCenterId",
                table: "court",
                column: "TennisCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_match_CourtId",
                table: "match",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_match_Player1Rni",
                table: "match",
                column: "Player1Rni");

            migrationBuilder.CreateIndex(
                name: "IX_match_Player2Rni",
                table: "match",
                column: "Player2Rni");

            migrationBuilder.CreateIndex(
                name: "IX_match_RoundId",
                table: "match",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_match_ScheduleFactoryId_ScheduleDay",
                table: "match",
                columns: new[] { "ScheduleFactoryId", "ScheduleDay" });

            migrationBuilder.CreateIndex(
                name: "IX_match_WinnerRni",
                table: "match",
                column: "WinnerRni");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTournament_TournamentsId",
                table: "PlayerTournament",
                column: "TournamentsId");

            migrationBuilder.CreateIndex(
                name: "IX_rounds_BracketId",
                table: "rounds",
                column: "BracketId");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_FactoryId",
                table: "tournament",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_QualificationId",
                table: "tournament",
                column: "QualificationId");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_factory_TennisCenterId",
                table: "tournament_factory",
                column: "TennisCenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "city");

            migrationBuilder.DropTable(
                name: "match");

            migrationBuilder.DropTable(
                name: "PlayerTournament");

            migrationBuilder.DropTable(
                name: "court");

            migrationBuilder.DropTable(
                name: "rounds");

            migrationBuilder.DropTable(
                name: "schedule");

            migrationBuilder.DropTable(
                name: "player");

            migrationBuilder.DropTable(
                name: "bracket");

            migrationBuilder.DropTable(
                name: "tournament");

            migrationBuilder.DropTable(
                name: "tournament_factory");

            migrationBuilder.DropTable(
                name: "tennis_center");
        }

        public ApiContext CreateDbContext(string[] args)
        {
          var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

          var builder = new DbContextOptionsBuilder<ApiContext>();
          var connectionString = configuration.GetConnectionString("Data:ConnectionString");

          builder.UseMySQL(connectionString);

          return new ApiContext(builder.Options);
        }
    }
}
