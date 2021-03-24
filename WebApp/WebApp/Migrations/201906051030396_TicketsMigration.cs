namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketsMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "ExpirationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "ExpirationDate");
        }
    }
}
