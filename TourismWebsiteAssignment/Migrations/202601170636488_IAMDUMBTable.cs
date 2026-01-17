namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IAMDUMBTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TravelPackage", "PackageId", "dbo.Booking");
            DropIndex("dbo.TravelPackage", new[] { "PackageId" });
            DropColumn("dbo.Booking", "PackageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Booking", "PackageId", c => c.Int(nullable: false));
            CreateIndex("dbo.TravelPackage", "PackageId");
            AddForeignKey("dbo.TravelPackage", "PackageId", "dbo.Booking", "BookingId");
        }
    }
}
