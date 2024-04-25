using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCoder.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlternateKeyToRepositoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProjectRepositories_OwnerUserName_Title",
                table: "ProjectRepositories",
                columns: new[] { "OwnerUserName", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProjectRepositories_OwnerUserName_Title",
                table: "ProjectRepositories");
        }
    }
}
