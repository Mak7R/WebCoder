using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCoder.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAlternateKeyForRepositories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProjectRepositories_OwnerUserName_Title",
                table: "ProjectRepositories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProjectRepositories_OwnerUserName_Title",
                table: "ProjectRepositories",
                columns: new[] { "OwnerUserName", "Title" });
        }
    }
}
