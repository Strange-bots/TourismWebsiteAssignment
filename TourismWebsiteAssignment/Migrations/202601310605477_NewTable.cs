namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Feedback", "TouristProfileId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Feedback", "TouristProfileId");
        }
    }
}
