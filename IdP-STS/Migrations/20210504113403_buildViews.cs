using Microsoft.EntityFrameworkCore.Migrations;

namespace STS.Migrations
{
    public partial class buildViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    create view dbo.VUserClaims
                    as
                    (

                    SELECT UserName
                          ,LockoutEnd
                          ,LockoutEnabled
                          ,AccessFailedCount
	                      ,	  ISNULL( ClaimType,'') as ClaimType
	                      ,isnull(ClaimValue,'') as ClaimValue
                      FROM [IdentityServer.Authentication].[dbo].[AspNetUsers] as users
                     left join [IdentityServer.Authentication].[dbo].[AspNetUserClaims] as claims on users.Id=claims.UserId
                     )"
                    );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop view dbo.VUserClaims");
        }
    }
}
