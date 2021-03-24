namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinesMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lines", "Description");
        }
    }
}
