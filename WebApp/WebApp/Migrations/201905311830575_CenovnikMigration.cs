namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CenovnikMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceList_TicketType", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PriceList_TicketType", "Price");
        }
    }
}
