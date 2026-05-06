using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GestionDocumental.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormatoEntrega",
                columns: table => new
                {
                    FormatoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoFormato = table.Column<string>(type: "text", nullable: false),
                    NombreFormato = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatoEntrega", x => x.FormatoId);
                });

            migrationBuilder.CreateTable(
                name: "MedicoDoc",
                columns: table => new
                {
                    MedicoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoMedico = table.Column<string>(type: "text", nullable: false),
                    NombreMedico = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicoDoc", x => x.MedicoId);
                });

            migrationBuilder.CreateTable(
                name: "MedioEnvio",
                columns: table => new
                {
                    MedioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoMedio = table.Column<string>(type: "text", nullable: false),
                    NombreMedio = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedioEnvio", x => x.MedioId);
                });

            migrationBuilder.CreateTable(
                name: "PacienteDoc",
                columns: table => new
                {
                    PacienteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoPaciente = table.Column<string>(type: "text", nullable: false),
                    NombrePaciente = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacienteDoc", x => x.PacienteId);
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumento",
                columns: table => new
                {
                    TipoDocId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoTipoDoc = table.Column<string>(type: "text", nullable: false),
                    NombreDocumento = table.Column<string>(type: "text", nullable: false),
                    DepartamentoOrigen = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumento", x => x.TipoDocId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoFormato",
                columns: table => new
                {
                    DocFormatoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoDocFormato = table.Column<string>(type: "text", nullable: false),
                    CodigoDocSolicitado = table.Column<string>(type: "text", nullable: false),
                    CodigoFormato = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    DocumentoSolicitadoDocSolicitadoId = table.Column<int>(type: "integer", nullable: false),
                    FormatoEntregaFormatoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoFormato", x => x.DocFormatoId);
                    table.ForeignKey(
                        name: "FK_DocumentoFormato_FormatoEntrega_FormatoEntregaFormatoId",
                        column: x => x.FormatoEntregaFormatoId,
                        principalTable: "FormatoEntrega",
                        principalColumn: "FormatoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoMedio",
                columns: table => new
                {
                    DocMedioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoDocMedio = table.Column<string>(type: "text", nullable: false),
                    CodigoDocSolicitado = table.Column<string>(type: "text", nullable: false),
                    CodigoMedio = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    DocumentoSolicitadoDocSolicitadoId = table.Column<int>(type: "integer", nullable: false),
                    MedioEnvioMedioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoMedio", x => x.DocMedioId);
                    table.ForeignKey(
                        name: "FK_DocumentoMedio_MedioEnvio_MedioEnvioMedioId",
                        column: x => x.MedioEnvioMedioId,
                        principalTable: "MedioEnvio",
                        principalColumn: "MedioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudDocumentacion",
                columns: table => new
                {
                    SolicitudId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoSolicitud = table.Column<string>(type: "text", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodigoPaciente = table.Column<string>(type: "text", nullable: false),
                    DepartamentoSolicitante = table.Column<string>(type: "text", nullable: false),
                    CodigoMedico = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    MedicoDocMedicoId = table.Column<int>(type: "integer", nullable: true),
                    PacienteDocPacienteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudDocumentacion", x => x.SolicitudId);
                    table.ForeignKey(
                        name: "FK_SolicitudDocumentacion_MedicoDoc_MedicoDocMedicoId",
                        column: x => x.MedicoDocMedicoId,
                        principalTable: "MedicoDoc",
                        principalColumn: "MedicoId");
                    table.ForeignKey(
                        name: "FK_SolicitudDocumentacion_PacienteDoc_PacienteDocPacienteId",
                        column: x => x.PacienteDocPacienteId,
                        principalTable: "PacienteDoc",
                        principalColumn: "PacienteId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentoSolicitado",
                columns: table => new
                {
                    DocSolicitadoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoDocSolicitado = table.Column<string>(type: "text", nullable: false),
                    CodigoSolicitud = table.Column<string>(type: "text", nullable: false),
                    CodigoTipoDoc = table.Column<string>(type: "text", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArchivoUrl = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    SolicitudId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumentoTipoDocId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoSolicitado", x => x.DocSolicitadoId);
                    table.ForeignKey(
                        name: "FK_DocumentoSolicitado_TipoDocumento_TipoDocumentoTipoDocId",
                        column: x => x.TipoDocumentoTipoDocId,
                        principalTable: "TipoDocumento",
                        principalColumn: "TipoDocId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntregaDocumento",
                columns: table => new
                {
                    EntregaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoEntrega = table.Column<string>(type: "text", nullable: false),
                    CodigoSolicitud = table.Column<string>(type: "text", nullable: false),
                    CodigoTipoDoc = table.Column<string>(type: "text", nullable: false),
                    CodigoFormato = table.Column<string>(type: "text", nullable: false),
                    CodigoMedio = table.Column<string>(type: "text", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    SolicitudId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumentoTipoDocId = table.Column<int>(type: "integer", nullable: false),
                    FormatoEntregaFormatoId = table.Column<int>(type: "integer", nullable: false),
                    MedioEnvioMedioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregaDocumento", x => x.EntregaId);
                    table.ForeignKey(
                        name: "FK_EntregaDocumento_FormatoEntrega_FormatoEntregaFormatoId",
                        column: x => x.FormatoEntregaFormatoId,
                        principalTable: "FormatoEntrega",
                        principalColumn: "FormatoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntregaDocumento_MedioEnvio_MedioEnvioMedioId",
                        column: x => x.MedioEnvioMedioId,
                        principalTable: "MedioEnvio",
                        principalColumn: "MedioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntregaDocumento_TipoDocumento_TipoDocumentoTipoDocId",
                        column: x => x.TipoDocumentoTipoDocId,
                        principalTable: "TipoDocumento",
                        principalColumn: "TipoDocId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoFormato_FormatoEntregaFormatoId",
                table: "DocumentoFormato",
                column: "FormatoEntregaFormatoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoMedio_MedioEnvioMedioId",
                table: "DocumentoMedio",
                column: "MedioEnvioMedioId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoSolicitado_TipoDocumentoTipoDocId",
                table: "DocumentoSolicitado",
                column: "TipoDocumentoTipoDocId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregaDocumento_FormatoEntregaFormatoId",
                table: "EntregaDocumento",
                column: "FormatoEntregaFormatoId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregaDocumento_MedioEnvioMedioId",
                table: "EntregaDocumento",
                column: "MedioEnvioMedioId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregaDocumento_TipoDocumentoTipoDocId",
                table: "EntregaDocumento",
                column: "TipoDocumentoTipoDocId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDocumentacion_MedicoDocMedicoId",
                table: "SolicitudDocumentacion",
                column: "MedicoDocMedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDocumentacion_PacienteDocPacienteId",
                table: "SolicitudDocumentacion",
                column: "PacienteDocPacienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentoFormato");

            migrationBuilder.DropTable(
                name: "DocumentoMedio");

            migrationBuilder.DropTable(
                name: "DocumentoSolicitado");

            migrationBuilder.DropTable(
                name: "EntregaDocumento");

            migrationBuilder.DropTable(
                name: "SolicitudDocumentacion");

            migrationBuilder.DropTable(
                name: "FormatoEntrega");

            migrationBuilder.DropTable(
                name: "MedioEnvio");

            migrationBuilder.DropTable(
                name: "TipoDocumento");

            migrationBuilder.DropTable(
                name: "MedicoDoc");

            migrationBuilder.DropTable(
                name: "PacienteDoc");
        }
    }
}
