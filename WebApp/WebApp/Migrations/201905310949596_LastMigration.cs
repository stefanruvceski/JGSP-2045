namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Users", newName: "AppUsers");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.AppUsers", newName: "Users");
        }
    }
}
