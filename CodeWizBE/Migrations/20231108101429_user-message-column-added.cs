using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeWizBE.Migrations
{
    /// <inheritdoc />
    public partial class usermessagecolumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserMessage",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserMessage",
                table: "Messages");
        }
    }
}
