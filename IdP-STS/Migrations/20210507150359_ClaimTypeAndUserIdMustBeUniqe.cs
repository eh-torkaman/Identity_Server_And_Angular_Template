using Microsoft.EntityFrameworkCore.Migrations;

namespace STS.Migrations
{
    public partial class ClaimTypeAndUserIdMustBeUniqe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE dbo.[AspNetUserClaims] ALTER COLUMN ClaimType nvarchar(150)   ;");
            migrationBuilder.Sql("ALTER TABLE dbo.[AspNetUserClaims] ALTER COLUMN  ClaimValue  nvarchar(150) ;");

            migrationBuilder.CreateIndex("Ix_Uniqe_UserID_ClaimType_TableUserClaims", "AspNetUserClaims", new[] { "UserId", "ClaimType" }, "dbo", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("Ix_Uniqe_UserID_ClaimType_TableUserClaims");

            migrationBuilder.Sql("ALTER TABLE dbo.[AspNetUserClaims] ALTER COLUMN ClaimType nvarchar(max)   ;");
            migrationBuilder.Sql("ALTER TABLE dbo.[AspNetUserClaims] ALTER COLUMN  ClaimValue  nvarchar(max) ;");
        }
    }
}
