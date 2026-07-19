using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mail.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "login_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eposta = table.Column<string>(maxLength: 50, nullable: false),
                    sifre = table.Column<string>(maxLength: 16, nullable: false),
                    IPV4 = table.Column<string>(maxLength: 40, nullable: false),
                    pc_user_name = table.Column<string>(maxLength: 40, nullable: false),
                    tarih = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mail_get_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    yollayan_kisi = table.Column<string>(maxLength: 100, nullable: false),
                    mail_alma_tarhi = table.Column<DateTimeOffset>(nullable: false),
                    alınan_mail_konu = table.Column<string>(maxLength: 1000, nullable: true),
                    alınan_mail_icerik = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_get_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_get_user_login_user_kisi_no",
                        column: x => x.kisi_no,
                        principalTable: "login_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mail_send_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    mail_yollama_tarhi = table.Column<DateTimeOffset>(nullable: false),
                    gonderilen_mail_konu = table.Column<string>(maxLength: 1000, nullable: true),
                    gonderilen_mail_icerik = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_send_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_send_user_login_user_kisi_no",
                        column: x => x.kisi_no,
                        principalTable: "login_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trash_get_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    yollayan_kisi = table.Column<string>(maxLength: 100, nullable: false),
                    mail_alma_tarhi = table.Column<DateTimeOffset>(nullable: false),
                    alınan_mail_konu = table.Column<string>(maxLength: 1000, nullable: true),
                    alınan_mail_icerik = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trash_get_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_trash_get_user_login_user_kisi_no",
                        column: x => x.kisi_no,
                        principalTable: "login_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mail_get_user_bodyfile",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    alınan_mail_no = table.Column<int>(nullable: false),
                    alınan_mail_bodyfile = table.Column<byte[]>(nullable: true),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_get_user_bodyfile", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_get_user_bodyfile_mail_get_user_alınan_mail_no",
                        column: x => x.alınan_mail_no,
                        principalTable: "mail_get_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mail_get_user_dosyalar",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    alınan_mail_no = table.Column<int>(nullable: false),
                    alınan_mail_dosyalar = table.Column<byte[]>(nullable: true),
                    attachment_name = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_get_user_dosyalar", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_get_user_dosyalar_mail_get_user_alınan_mail_no",
                        column: x => x.alınan_mail_no,
                        principalTable: "mail_get_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mail_send_user_bodyfile",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    gonderilen_mail_no = table.Column<int>(nullable: false),
                    gonderilen_mail_bodyfile = table.Column<byte[]>(nullable: true),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_send_user_bodyfile", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_send_user_bodyfile_mail_send_user_gonderilen_mail_no",
                        column: x => x.gonderilen_mail_no,
                        principalTable: "mail_send_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mail_send_user_dosyalar",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    gonderilen_mail_no = table.Column<int>(nullable: false),
                    gonderilen_mail_dosyalar = table.Column<byte[]>(nullable: true),
                    attachment_name = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mail_send_user_dosyalar", x => x.id);
                    table.ForeignKey(
                        name: "FK_mail_send_user_dosyalar_mail_send_user_gonderilen_mail_no",
                        column: x => x.gonderilen_mail_no,
                        principalTable: "mail_send_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trash_get_user_bodyfile",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    alınan_mail_no = table.Column<int>(nullable: false),
                    alınan_mail_bodyfile = table.Column<byte[]>(nullable: true),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trash_get_user_bodyfile", x => x.id);
                    table.ForeignKey(
                        name: "FK_trash_get_user_bodyfile_trash_get_user_alınan_mail_no",
                        column: x => x.alınan_mail_no,
                        principalTable: "trash_get_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trash_get_user_dosyalar",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kisi_no = table.Column<int>(nullable: false),
                    alınan_mail_no = table.Column<int>(nullable: false),
                    alınan_mail_dosyalar = table.Column<byte[]>(nullable: true),
                    attachment_name = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trash_get_user_dosyalar", x => x.id);
                    table.ForeignKey(
                        name: "FK_trash_get_user_dosyalar_trash_get_user_alınan_mail_no",
                        column: x => x.alınan_mail_no,
                        principalTable: "trash_get_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mail_get_user_kisi_no",
                table: "mail_get_user",
                column: "kisi_no");

            migrationBuilder.CreateIndex(
                name: "IX_mail_get_user_bodyfile_alınan_mail_no",
                table: "mail_get_user_bodyfile",
                column: "alınan_mail_no");

            migrationBuilder.CreateIndex(
                name: "IX_mail_get_user_dosyalar_alınan_mail_no",
                table: "mail_get_user_dosyalar",
                column: "alınan_mail_no");

            migrationBuilder.CreateIndex(
                name: "IX_mail_send_user_kisi_no",
                table: "mail_send_user",
                column: "kisi_no");

            migrationBuilder.CreateIndex(
                name: "IX_mail_send_user_bodyfile_gonderilen_mail_no",
                table: "mail_send_user_bodyfile",
                column: "gonderilen_mail_no");

            migrationBuilder.CreateIndex(
                name: "IX_mail_send_user_dosyalar_gonderilen_mail_no",
                table: "mail_send_user_dosyalar",
                column: "gonderilen_mail_no");

            migrationBuilder.CreateIndex(
                name: "IX_trash_get_user_kisi_no",
                table: "trash_get_user",
                column: "kisi_no");

            migrationBuilder.CreateIndex(
                name: "IX_trash_get_user_bodyfile_alınan_mail_no",
                table: "trash_get_user_bodyfile",
                column: "alınan_mail_no");

            migrationBuilder.CreateIndex(
                name: "IX_trash_get_user_dosyalar_alınan_mail_no",
                table: "trash_get_user_dosyalar",
                column: "alınan_mail_no");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mail_get_user_bodyfile");

            migrationBuilder.DropTable(
                name: "mail_get_user_dosyalar");

            migrationBuilder.DropTable(
                name: "mail_send_user_bodyfile");

            migrationBuilder.DropTable(
                name: "mail_send_user_dosyalar");

            migrationBuilder.DropTable(
                name: "trash_get_user_bodyfile");

            migrationBuilder.DropTable(
                name: "trash_get_user_dosyalar");

            migrationBuilder.DropTable(
                name: "mail_get_user");

            migrationBuilder.DropTable(
                name: "mail_send_user");

            migrationBuilder.DropTable(
                name: "trash_get_user");

            migrationBuilder.DropTable(
                name: "login_user");
        }
    }
}
