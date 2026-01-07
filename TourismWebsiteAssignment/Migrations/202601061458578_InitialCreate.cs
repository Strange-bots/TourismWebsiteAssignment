namespace TourismWebsiteAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Booking",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        TouristProfileId = c.Int(nullable: false),
                        TourDateId = c.Int(nullable: false),
                        BookingStatusId = c.Int(nullable: false),
                        BookingDate = c.DateTime(nullable: false),
                        NumberOfGuests = c.Int(nullable: false),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialStatus = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.BookingStatus", t => t.BookingStatusId, cascadeDelete: true)
                .ForeignKey("dbo.TourDate", t => t.TourDateId, cascadeDelete: true)
                .ForeignKey("dbo.TouristProfile", t => t.TouristProfileId, cascadeDelete: true)
                .Index(t => t.TouristProfileId)
                .Index(t => t.TourDateId)
                .Index(t => t.BookingStatusId);
            
            CreateTable(
                "dbo.BookingStatus",
                c => new
                    {
                        BookingStatusId = c.Int(nullable: false, identity: true),
                        StatusName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.BookingStatusId);
            
            CreateTable(
                "dbo.TourDate",
                c => new
                    {
                        TourDateId = c.Int(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AvailableSlots = c.Int(nullable: false),
                        TotalSlots = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 50),
                        PriceAdjustment = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TourDateId)
                .ForeignKey("dbo.TravelPackage", t => t.PackageId, cascadeDelete: true)
                .Index(t => t.PackageId);
            
            CreateTable(
                "dbo.TravelPackage",
                c => new
                    {
                        PackageId = c.Int(nullable: false, identity: true),
                        AgencyId = c.Int(nullable: false),
                        PackageTitle = c.String(nullable: false, maxLength: 200),
                        PackageDescription = c.String(nullable: false, maxLength: 2000),
                        Destination = c.String(nullable: false, maxLength: 100),
                        PricePerPerson = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GroupMaxSize = c.Int(nullable: false),
                        Inclusions = c.String(maxLength: 1000),
                        Exclusions = c.String(maxLength: 1000),
                        ItineraryDetails = c.String(maxLength: 2000),
                        TermsAndConditions = c.String(nullable: false, maxLength: 2000),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PackageId)
                .ForeignKey("dbo.TravelAgency", t => t.AgencyId, cascadeDelete: true)
                .Index(t => t.AgencyId);
            
            CreateTable(
                "dbo.TravelAgency",
                c => new
                    {
                        AgencyId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        AgencyName = c.String(nullable: false, maxLength: 100),
                        LicenseNumber = c.String(nullable: false, maxLength: 50),
                        ContactNumber = c.String(nullable: false, maxLength: 20),
                        AgencyAddress = c.String(nullable: false, maxLength: 255),
                        ContactPerson = c.String(nullable: false, maxLength: 100),
                        PhoneNumber = c.String(nullable: false, maxLength: 20),
                        AgencyDescription = c.String(nullable: false, maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        LogoUrl = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.AgencyId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 100),
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 255),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.TouristProfile",
                c => new
                    {
                        TouristProfileId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Gender = c.String(nullable: false, maxLength: 10),
                        DateOfBirth = c.DateTime(nullable: false),
                        Address = c.String(nullable: false, maxLength: 255),
                        Nationality = c.String(nullable: false, maxLength: 50),
                        TravelPreferences = c.String(maxLength: 500),
                        ProfileImageUrl = c.String(maxLength: 255),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.TouristProfileId)
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.User", t => t.User_UserId)
                .Index(t => t.UserId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Feedback",
                c => new
                    {
                        FeedbackId = c.Int(nullable: false, identity: true),
                        TouristId = c.Int(nullable: false),
                        BookingId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comments = c.String(nullable: false, maxLength: 1000),
                        SubmittedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FeedbackId)
                .ForeignKey("dbo.Booking", t => t.BookingId)
                .ForeignKey("dbo.TouristProfile", t => t.TouristId)
                .Index(t => t.TouristId)
                .Index(t => t.BookingId);
            
            CreateTable(
                "dbo.PackageImage",
                c => new
                    {
                        ImageId = c.Int(nullable: false, identity: true),
                        PackageId = c.Int(nullable: false),
                        ImageURL = c.String(nullable: false, maxLength: 255),
                        UploadedAt = c.DateTime(nullable: false),
                        Caption = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ImageId)
                .ForeignKey("dbo.TravelPackage", t => t.PackageId, cascadeDelete: true)
                .Index(t => t.PackageId);
            
            CreateTable(
                "dbo.PaymentTransactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        BookingId = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(nullable: false, maxLength: 50),
                        TransactionStatus = c.String(nullable: false, maxLength: 50),
                        TransactionReference = c.String(nullable: false, maxLength: 100),
                        Currency = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Booking", t => t.BookingId, cascadeDelete: true)
                .Index(t => t.BookingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaymentTransactions", "BookingId", "dbo.Booking");
            DropForeignKey("dbo.PackageImage", "PackageId", "dbo.TravelPackage");
            DropForeignKey("dbo.Feedback", "TouristId", "dbo.TouristProfile");
            DropForeignKey("dbo.Feedback", "BookingId", "dbo.Booking");
            DropForeignKey("dbo.Booking", "TouristProfileId", "dbo.TouristProfile");
            DropForeignKey("dbo.Booking", "TourDateId", "dbo.TourDate");
            DropForeignKey("dbo.TourDate", "PackageId", "dbo.TravelPackage");
            DropForeignKey("dbo.TravelPackage", "AgencyId", "dbo.TravelAgency");
            DropForeignKey("dbo.TravelAgency", "UserId", "dbo.User");
            DropForeignKey("dbo.TouristProfile", "User_UserId", "dbo.User");
            DropForeignKey("dbo.TouristProfile", "UserId", "dbo.User");
            DropForeignKey("dbo.User", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Booking", "BookingStatusId", "dbo.BookingStatus");
            DropIndex("dbo.PaymentTransactions", new[] { "BookingId" });
            DropIndex("dbo.PackageImage", new[] { "PackageId" });
            DropIndex("dbo.Feedback", new[] { "BookingId" });
            DropIndex("dbo.Feedback", new[] { "TouristId" });
            DropIndex("dbo.TouristProfile", new[] { "User_UserId" });
            DropIndex("dbo.TouristProfile", new[] { "UserId" });
            DropIndex("dbo.User", new[] { "RoleId" });
            DropIndex("dbo.TravelAgency", new[] { "UserId" });
            DropIndex("dbo.TravelPackage", new[] { "AgencyId" });
            DropIndex("dbo.TourDate", new[] { "PackageId" });
            DropIndex("dbo.Booking", new[] { "BookingStatusId" });
            DropIndex("dbo.Booking", new[] { "TourDateId" });
            DropIndex("dbo.Booking", new[] { "TouristProfileId" });
            DropTable("dbo.PaymentTransactions");
            DropTable("dbo.PackageImage");
            DropTable("dbo.Feedback");
            DropTable("dbo.TouristProfile");
            DropTable("dbo.Role");
            DropTable("dbo.User");
            DropTable("dbo.TravelAgency");
            DropTable("dbo.TravelPackage");
            DropTable("dbo.TourDate");
            DropTable("dbo.BookingStatus");
            DropTable("dbo.Booking");
        }
    }
}
