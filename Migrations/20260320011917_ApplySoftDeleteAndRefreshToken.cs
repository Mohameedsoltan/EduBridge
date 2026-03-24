using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduBridge.Migrations
{
    /// <inheritdoc />
    public partial class ApplySoftDeleteAndRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorRequests_Doctors_DoctorId",
                table: "DoctorRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TARequests_TeachingAssistants_RespondedByTAId",
                table: "TARequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TARequests_TeachingAssistants_TAId",
                table: "TARequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TARequests_Teams_TeamId",
                table: "TARequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TARequests",
                table: "TARequests");

            migrationBuilder.RenameTable(
                name: "TARequests",
                newName: "TaRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TARequests_TeamId",
                table: "TaRequests",
                newName: "IX_TaRequests_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TARequests_TAId",
                table: "TaRequests",
                newName: "IX_TaRequests_TAId");

            migrationBuilder.RenameIndex(
                name: "IX_TARequests_RespondedByTAId",
                table: "TaRequests",
                newName: "IX_TaRequests_RespondedByTAId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaRequests",
                table: "TaRequests",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdeaTags_Name_CategoryId",
                table: "IdeaTags",
                columns: new[] { "Name", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdeaCategories_Name",
                table: "IdeaCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorRequests_Doctors_DoctorId",
                table: "DoctorRequests",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaRequests_TeachingAssistants_RespondedByTAId",
                table: "TaRequests",
                column: "RespondedByTAId",
                principalTable: "TeachingAssistants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaRequests_TeachingAssistants_TAId",
                table: "TaRequests",
                column: "TAId",
                principalTable: "TeachingAssistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaRequests_Teams_TeamId",
                table: "TaRequests",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorRequests_Doctors_DoctorId",
                table: "DoctorRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaRequests_TeachingAssistants_RespondedByTAId",
                table: "TaRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaRequests_TeachingAssistants_TAId",
                table: "TaRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TaRequests_Teams_TeamId",
                table: "TaRequests");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaRequests",
                table: "TaRequests");

            migrationBuilder.DropIndex(
                name: "IX_Skills_Name",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_IdeaTags_Name_CategoryId",
                table: "IdeaTags");

            migrationBuilder.DropIndex(
                name: "IX_IdeaCategories_Name",
                table: "IdeaCategories");

            migrationBuilder.RenameTable(
                name: "TaRequests",
                newName: "TARequests");

            migrationBuilder.RenameIndex(
                name: "IX_TaRequests_TeamId",
                table: "TARequests",
                newName: "IX_TARequests_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TaRequests_TAId",
                table: "TARequests",
                newName: "IX_TARequests_TAId");

            migrationBuilder.RenameIndex(
                name: "IX_TaRequests_RespondedByTAId",
                table: "TARequests",
                newName: "IX_TARequests_RespondedByTAId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TARequests",
                table: "TARequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorRequests_Doctors_DoctorId",
                table: "DoctorRequests",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TARequests_TeachingAssistants_RespondedByTAId",
                table: "TARequests",
                column: "RespondedByTAId",
                principalTable: "TeachingAssistants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TARequests_TeachingAssistants_TAId",
                table: "TARequests",
                column: "TAId",
                principalTable: "TeachingAssistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TARequests_Teams_TeamId",
                table: "TARequests",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
