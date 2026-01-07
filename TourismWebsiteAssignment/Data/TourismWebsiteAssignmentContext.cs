using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TourismWebsiteAssignment.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace TourismWebsiteAssignment.Data
{
    public class TourismWebsiteAssignmentContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public TourismWebsiteAssignmentContext() : base("TourismWebsiteAssignmentContext")
        {
        }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.BookingStatus> BookingStatus { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.Booking> Bookings { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.TourDate> TourDates { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.TouristProfile> TouristProfiles { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.Feedback> Feedbacks { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.PackageImage> PackageImages { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.TravelPackage> TravelPackages { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.PaymentTransactions> PaymentTransactions { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.Role> Roles { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<TourismWebsiteAssignment.Models.TravelAgency> TravelAgencies { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Disable cascade delete for TouristProfile -> User
            modelBuilder.Entity<TouristProfile>()
                .HasRequired(tp => tp.User)
                .WithMany()
                .HasForeignKey(tp => tp.UserId)
                .WillCascadeOnDelete(false);

            // Disable cascade delete for Feedback -> TouristProfile
            modelBuilder.Entity<Feedback>()
                .HasRequired(f => f.Tourist)
                .WithMany() // no navigation property in TouristProfile
                .HasForeignKey(f => f.TouristId)
                .WillCascadeOnDelete(false);

            // Optionally, disable cascade delete for Feedback -> Booking if needed
            modelBuilder.Entity<Feedback>()
                .HasRequired(f => f.Booking)
                .WithMany()
                .HasForeignKey(f => f.BookingId)
                .WillCascadeOnDelete(false);
        }

    }
}
    