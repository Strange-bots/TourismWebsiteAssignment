namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFeedbackTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Feedback", "TouristProfileId");
            RenameColumn(table: "dbo.Feedback", name: "TouristId", newName: "TouristProfileId");
            RenameIndex(table: "dbo.Feedback", name: "IX_TouristId", newName: "IX_TouristProfileId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Feedback", name: "IX_TouristProfileId", newName: "IX_TouristId");
            RenameColumn(table: "dbo.Feedback", name: "TouristProfileId", newName: "TouristId");
            AddColumn("dbo.Feedback", "TouristProfileId", c => c.Int(nullable: false));
        }
    }
}
