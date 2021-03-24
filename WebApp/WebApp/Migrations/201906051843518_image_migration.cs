namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class image_migration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AppUsers", "Document");
            AddColumn("dbo.AppUsers", "Document",c=>c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "Document");
            AddColumn("dbo.AppUsers", "Document", c => c.String());
        }
    }
}
