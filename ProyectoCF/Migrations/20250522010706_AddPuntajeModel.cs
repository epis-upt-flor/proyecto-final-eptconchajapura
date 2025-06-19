using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCF.Migrations
{
    /// <inheritdoc />
    public partial class AddPuntajeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Puntajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Valor = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puntajes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RespuestasEstudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    EvaluacionId = table.Column<int>(type: "int", nullable: false),
                    PreguntaId = table.Column<int>(type: "int", nullable: false),
                    RespuestaId = table.Column<int>(type: "int", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasEstudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasEstudiantes_Evaluaciones_EvaluacionId",
                        column: x => x.EvaluacionId,
                        principalTable: "Evaluaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasEstudiantes_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasEstudiantes_Respuestas_RespuestaId",
                        column: x => x.RespuestaId,
                        principalTable: "Respuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasEstudiantes_Usuarios_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Puntajes_UsuarioId",
                table: "Puntajes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEstudiantes_EstudianteId",
                table: "RespuestasEstudiantes",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEstudiantes_EvaluacionId",
                table: "RespuestasEstudiantes",
                column: "EvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEstudiantes_PreguntaId",
                table: "RespuestasEstudiantes",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasEstudiantes_RespuestaId",
                table: "RespuestasEstudiantes",
                column: "RespuestaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puntajes");

            migrationBuilder.DropTable(
                name: "RespuestasEstudiantes");
        }
    }
}
