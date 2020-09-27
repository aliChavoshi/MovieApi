using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesApi.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //نقش ادمین را اضافه کن
            migrationBuilder.Sql(@"insert into AspNetRoles (Id,[Name],[NormalizedName])
            values ('8c2dfcec-c7b8-4f3e-a6ea-7b69f68488f1','Admin','Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //اگر مقدار بالا داخلش بود برایم پاک کن
            migrationBuilder.Sql(@"delete aspnetRoles where id='8c2dfcec-c7b8-4f3e-a6ea-7b69f68488f1'");
        }
    }
}
