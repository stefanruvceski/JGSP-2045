namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LineStationsMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lines", "Color", c => c.String());
            AddColumn("dbo.Stations", "IsStation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stations", "IsStation");
            DropColumn("dbo.Lines", "Color");
        }
    }
}
