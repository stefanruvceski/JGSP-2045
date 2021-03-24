namespace WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BigChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AgeGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        Coefficient = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Buses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineId = c.String(maxLength: 128),
                        XCooridinate = c.Double(nullable: false),
                        YCoordinate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lines", t => t.LineId)
                .Index(t => t.LineId);
            
            CreateTable(
                "dbo.Lines",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        LineType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationName = c.String(),
                        Address = c.String(),
                        XCooridinate = c.Double(nullable: false),
                        YCoordinate = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        Address = c.String(),
                        Type = c.Int(nullable: false),
                        Document = c.String(),
                        AgeGroupId = c.Int(),
                        VerificationStatus = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AgeGroups", t => t.AgeGroupId, cascadeDelete: true)
                .Index(t => t.AgeGroupId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketTypeId = c.Int(nullable: false),
                        PassengerId = c.Int(nullable: false),
                        IssuingDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.PassengerId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId, cascadeDelete: true)
                .Index(t => t.TicketTypeId)
                .Index(t => t.PassengerId);
            
            CreateTable(
                "dbo.TicketTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PriceList_TicketType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriceListId = c.Int(nullable: false),
                        TicketTypeId = c.Int(nullable: false),
                        AgeGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AgeGroups", t => t.AgeGroupId, cascadeDelete: true)
                .ForeignKey("dbo.PriceLists", t => t.PriceListId, cascadeDelete: true)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId, cascadeDelete: true)
                .Index(t => t.PriceListId)
                .Index(t => t.TicketTypeId)
                .Index(t => t.AgeGroupId);
            
            CreateTable(
                "dbo.PriceLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IssueDate = c.DateTime(nullable: false),
                        ExpireDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StationLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationId = c.Int(nullable: false),
                        LineId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lines", t => t.LineId)
                .ForeignKey("dbo.Stations", t => t.StationId, cascadeDelete: true)
                .Index(t => t.StationId)
                .Index(t => t.LineId);
            
            CreateTable(
                "dbo.TimeTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        Schedule = c.String(),
                        LineId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lines", t => t.LineId)
                .Index(t => t.LineId);
            
            CreateTable(
                "dbo.LineStations",
                c => new
                    {
                        Line_Id = c.String(nullable: false, maxLength: 128),
                        Station_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Line_Id, t.Station_Id })
                .ForeignKey("dbo.Lines", t => t.Line_Id, cascadeDelete: true)
                .ForeignKey("dbo.Stations", t => t.Station_Id, cascadeDelete: true)
                .Index(t => t.Line_Id)
                .Index(t => t.Station_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeTables", "LineId", "dbo.Lines");
            DropForeignKey("dbo.StationLines", "StationId", "dbo.Stations");
            DropForeignKey("dbo.StationLines", "LineId", "dbo.Lines");
            DropForeignKey("dbo.PriceList_TicketType", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.PriceList_TicketType", "PriceListId", "dbo.PriceLists");
            DropForeignKey("dbo.PriceList_TicketType", "AgeGroupId", "dbo.AgeGroups");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropForeignKey("dbo.Tickets", "PassengerId", "dbo.Users");
            DropForeignKey("dbo.Users", "AgeGroupId", "dbo.AgeGroups");
            DropForeignKey("dbo.Buses", "LineId", "dbo.Lines");
            DropForeignKey("dbo.LineStations", "Station_Id", "dbo.Stations");
            DropForeignKey("dbo.LineStations", "Line_Id", "dbo.Lines");
            DropIndex("dbo.LineStations", new[] { "Station_Id" });
            DropIndex("dbo.LineStations", new[] { "Line_Id" });
            DropIndex("dbo.TimeTables", new[] { "LineId" });
            DropIndex("dbo.StationLines", new[] { "LineId" });
            DropIndex("dbo.StationLines", new[] { "StationId" });
            DropIndex("dbo.PriceList_TicketType", new[] { "AgeGroupId" });
            DropIndex("dbo.PriceList_TicketType", new[] { "TicketTypeId" });
            DropIndex("dbo.PriceList_TicketType", new[] { "PriceListId" });
            DropIndex("dbo.Tickets", new[] { "PassengerId" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Users", new[] { "AgeGroupId" });
            DropIndex("dbo.Buses", new[] { "LineId" });
            DropTable("dbo.LineStations");
            DropTable("dbo.TimeTables");
            DropTable("dbo.StationLines");
            DropTable("dbo.PriceLists");
            DropTable("dbo.PriceList_TicketType");
            DropTable("dbo.TicketTypes");
            DropTable("dbo.Tickets");
            DropTable("dbo.Users");
            DropTable("dbo.Stations");
            DropTable("dbo.Lines");
            DropTable("dbo.Buses");
            DropTable("dbo.AgeGroups");
        }
    }
}
