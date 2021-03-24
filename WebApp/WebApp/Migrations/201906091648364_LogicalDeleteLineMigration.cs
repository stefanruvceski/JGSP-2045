namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogicalDeleteLineMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "IsActive");
        }
    }
}
