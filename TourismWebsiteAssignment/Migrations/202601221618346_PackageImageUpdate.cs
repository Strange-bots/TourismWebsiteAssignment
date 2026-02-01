namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PackageImageUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PackageImage", "PackageImage_ImageId", c => c.Int());
            CreateIndex("dbo.PackageImage", "PackageImage_ImageId");
            AddForeignKey("dbo.PackageImage", "PackageImage_ImageId", "dbo.PackageImage", "ImageId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PackageImage", "PackageImage_ImageId", "dbo.PackageImage");
            DropIndex("dbo.PackageImage", new[] { "PackageImage_ImageId" });
            DropColumn("dbo.PackageImage", "PackageImage_ImageId");
        }
    }
}
