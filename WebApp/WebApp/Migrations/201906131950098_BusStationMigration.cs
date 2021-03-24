namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BusStationMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Buses", "NextStationX", c => c.Double(nullable: false));
            AddColumn("dbo.Buses", "NextStationY", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Buses", "NextStationY");
            DropColumn("dbo.Buses", "NextStationX");
        }
    }
}
