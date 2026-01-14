namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageIdAuto : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourDate", "PackageId", "dbo.TravelPackage");
            DropForeignKey("dbo.PackageImage", "PackageId", "dbo.TravelPackage");
            DropIndex("dbo.TravelPackage", new[] { "PackageId" });
            DropPrimaryKey("dbo.TravelPackage");
            AlterColumn("dbo.TravelPackage", "PackageId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TravelPackage", "PackageId");
            CreateIndex("dbo.TravelPackage", "PackageId");
            AddForeignKey("dbo.TourDate", "PackageId", "dbo.TravelPackage", "PackageId", cascadeDelete: true);
            AddForeignKey("dbo.PackageImage", "PackageId", "dbo.TravelPackage", "PackageId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PackageImage", "PackageId", "dbo.TravelPackage");
            DropForeignKey("dbo.TourDate", "PackageId", "dbo.TravelPackage");
            DropIndex("dbo.TravelPackage", new[] { "PackageId" });
            DropPrimaryKey("dbo.TravelPackage");
            AlterColumn("dbo.TravelPackage", "PackageId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TravelPackage", "PackageId");
            CreateIndex("dbo.TravelPackage", "PackageId");
            AddForeignKey("dbo.PackageImage", "PackageId", "dbo.TravelPackage", "PackageId", cascadeDelete: true);
            AddForeignKey("dbo.TourDate", "PackageId", "dbo.TravelPackage", "PackageId", cascadeDelete: true);
        }
    }
}
