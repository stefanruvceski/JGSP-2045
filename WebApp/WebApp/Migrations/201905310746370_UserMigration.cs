namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "UserId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "UserId");
            AddForeignKey("dbo.AspNetUsers", "UserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "UserId", "dbo.Users");
            DropIndex("dbo.AspNetUsers", new[] { "UserId" });
            DropColumn("dbo.AspNetUsers", "UserId");
        }
    }
}
