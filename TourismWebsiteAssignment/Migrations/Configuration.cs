using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Migrations
{
    internal sealed class Configuration
        : DbMigrationsConfiguration<TourismWebsiteAssignmentContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TourismWebsiteAssignmentContext context)
        {
            /* =========================
               1) ROLES
               ========================= */

            EnsureRole(context, "Admin", "System administrator");
            EnsureRole(context, "Agent", "Travel agency user");
            EnsureRole(context, "Tourist", "Tourist user");

            context.SaveChanges();

            var adminRoleId = context.Roles.First(r => r.RoleName == "Admin").RoleId;
            var agentRoleId = context.Roles.First(r => r.RoleName == "Agent").RoleId;
            var touristRoleId = context.Roles.First(r => r.RoleName == "Tourist").RoleId;

            /* =========================
               2) USERS
               ========================= */

            UpsertUser(context, "System Admin", "admin@demo.com", "admin", "Admin@123", adminRoleId);
            UpsertUser(context, "Demo Agent", "agent@demo.com", "agent", "Agent@123", agentRoleId);
            UpsertUser(context, "Demo Tourist", "tourist@demo.com", "tourist", "Tourist@123", touristRoleId);
            context.SaveChanges();
            /* =========================
   4) TRAVEL AGENCY
   ========================= */

            var agentUser = context.Users
                .FirstOrDefault(u => u.Username == "agent");

            if (agentUser != null)
            {
                EnsureTravelAgency(
                    context,
                    agentUser.UserId,
                    "Demo Travel Agency",
                    "LIC-AGENT-001",
                    "+61 400 000 000",
                    "123 George Street, Sydney NSW",
                    "Demo Agent",
                    "+61 400 000 001",
                    "This is a demo travel agency created for system demonstration purposes.",
                    "/Content/Images/demo-agency-logo.png"
                );
            }

            context.SaveChanges();
            /* =========================
   5) TRAVEL PACKAGES
   ========================= */

            var demoAgency = context.TravelAgencies
                .FirstOrDefault(a => a.AgencyName == "Demo Travel Agency");

            if (demoAgency != null)
            {
                EnsureTravelPackage(
                    context,
                    demoAgency.AgencyId,
                    "Sydney City Highlights Day Tour",
                    "A guided day tour covering Sydney’s key landmarks with local insights and photo stops.",
                    "Sydney, Australia",
                    149.00m,
                    20,
                    "Hotel pickup (selected locations)\nLocal guide\nAttraction entry (as specified)",
                    "Meals\nPersonal expenses\nTravel insurance",
                    "09:00 Meet & greet\n10:00 Opera House & Circular Quay\n12:00 Lunch break\n14:00 Bondi Beach\n16:30 Return",
                    "Bookings are subject to availability. Cancellations within 24 hours are non-refundable."
                );

                EnsureTravelPackage(
                    context,
                    demoAgency.AgencyId,
                    "Blue Mountains Scenic Adventure",
                    "A full-day trip to the Blue Mountains including viewpoints and short walks. Ideal for first-time visitors.",
                    "Blue Mountains, Australia",
                    199.00m,
                    18,
                    "Return transport\nGuide\nScenic stops",
                    "Meals\nOptional attractions",
                    "08:00 Departure\n10:30 Scenic World (optional)\n12:30 Echo Point\n15:00 Short walk\n17:30 Return",
                    "Bring comfortable shoes. Weather conditions may alter the itinerary."
                );

                EnsureTravelPackage(
                    context,
                    demoAgency.AgencyId,
                    "Hunter Valley Winery Experience",
                    "Taste local wines and enjoy a relaxed day in Hunter Valley with guided cellar-door visits.",
                    "Hunter Valley, Australia",
                    229.00m,
                    16,
                    "Transport\nWine tastings\nGuide",
                    "Lunch\nPurchases at venues",
                    "08:30 Depart\n11:00 Winery 1\n13:00 Lunch stop\n14:30 Winery 2\n17:30 Return",
                    "Participants must be 18+ to consume alcohol. ID may be required."
                );
            }

            context.SaveChanges();
            /* =========================
   6) TOUR DATES
   ========================= */

            var sydneyPackage = context.TravelPackages
                .FirstOrDefault(p => p.PackageTitle == "Sydney City Highlights Day Tour");

            if (sydneyPackage != null)
            {
                EnsureTourDate(
                    context,
                    sydneyPackage.PackageId,
                    new DateTime(2026, 03, 10),
                    new DateTime(2026, 03, 10),
                    totalSlots: 20,
                    availableSlots: 20,
                    status: "Open",
                    priceAdjustment: 0m
                );

                EnsureTourDate(
                    context,
                    sydneyPackage.PackageId,
                    new DateTime(2026, 03, 20),
                    new DateTime(2026, 03, 20),
                    totalSlots: 20,
                    availableSlots: 15,
                    status: "Open",
                    priceAdjustment: 10m
                );
            }

            var blueMountainsPackage = context.TravelPackages
                .FirstOrDefault(p => p.PackageTitle == "Blue Mountains Scenic Adventure");

            if (blueMountainsPackage != null)
            {
                EnsureTourDate(
                    context,
                    blueMountainsPackage.PackageId,
                    new DateTime(2026, 04, 05),
                    new DateTime(2026, 04, 05),
                    totalSlots: 18,
                    availableSlots: 18,
                    status: "Open",
                    priceAdjustment: 0m
                );
            }

            context.SaveChanges();
            /* =========================
   7) PACKAGE IMAGES
   ========================= */

            var sydneysPackage = context.TravelPackages
                .FirstOrDefault(p => p.PackageTitle == "Sydney City Highlights Day Tour");

            if (sydneysPackage != null)
            {
                EnsurePackageImage(
                    context,
                    sydneysPackage.PackageId,
                    "https://images.unsplash.com/photo-1506973035872-a4f23f4a6d7c",
                    "Sydney Opera House and Harbour",
                    DateTime.UtcNow
                );

                EnsurePackageImage(
                    context,
                    sydneysPackage.PackageId,
                    "https://images.unsplash.com/photo-1524293581917-878a6d017c71",
                    "Bondi Beach coastline",
                    DateTime.UtcNow
                );
            }

            var blueMountainsPackages = context.TravelPackages
                .FirstOrDefault(p => p.PackageTitle == "Blue Mountains Scenic Adventure");

            if (blueMountainsPackages != null)
            {
                EnsurePackageImage(
                    context,
                    blueMountainsPackages.PackageId,
                    "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee",
                    "Blue Mountains cliffs and valleys",
                    DateTime.UtcNow
                );
            }

            var hunterValleyPackage = context.TravelPackages
                .FirstOrDefault(p => p.PackageTitle == "Hunter Valley Winery Experience");

            if (hunterValleyPackage != null)
            {
                EnsurePackageImage(
                    context,
                    hunterValleyPackage.PackageId,
                    "https://images.unsplash.com/photo-1504754524776-8f4f37790ca0",
                    "Hunter Valley vineyards",
                    DateTime.UtcNow
                );
            }

            context.SaveChanges();
            /* =========================
   8) TOURIST PROFILE
   ========================= */

            var touristUser = context.Users
                .FirstOrDefault(u => u.Username == "tourist");

            if (touristUser != null)
            {
                EnsureTouristProfile(
                    context,
                    touristUser.UserId,
                    "Demo Tourist",
                    "Male",
                    new DateTime(1998, 6, 15),
                    "45 Elizabeth Street, Melbourne VIC",
                    "Australian",
                    "City tours, nature walks, local food experiences",
                    "https://images.unsplash.com/photo-1500648767791-00dcc994a43e"
                );
            }

            context.SaveChanges();



            /* =========================
               3) BOOKING STATUSES
               ========================= */

            EnsureBookingStatus(context, "Pending",
                "Booking created but not yet confirmed");

            EnsureBookingStatus(context, "Confirmed",
                "Booking confirmed and slots reserved");

            EnsureBookingStatus(context, "Cancelled",
                "Booking cancelled by user or administrator");

            context.SaveChanges();

            /* =========================
   9) BOOKINGS
   ========================= */

            var demoTouristProfile = context.TouristProfiles
                .FirstOrDefault(tp => tp.FullName == "Demo Tourist");

            var pendingStatusId = context.BookingStatus
                .FirstOrDefault(s => s.StatusName == "Pending")?.BookingStatusId;

            var tourDate = context.TourDates
                .OrderBy(td => td.StartDate)
                .FirstOrDefault(); // simplest: take first seeded tour date

            if (demoTouristProfile != null && pendingStatusId != null && tourDate != null)
            {
                // Pricing logic: package price + price adjustment (per person) * guests
                var package = context.TravelPackages.FirstOrDefault(p => p.PackageId == tourDate.PackageId);
                var guests = 2;

                decimal perPerson = (package?.PricePerPerson ?? 0m) + tourDate.PriceAdjustment;
                decimal total = perPerson * guests;

                EnsureBooking(
                    context,
                    demoTouristProfile.TouristProfileId,
                    tourDate.TourDateId,
                    pendingStatusId.Value,
                    bookingDate: new DateTime(2026, 02, 01),
                    numberOfGuests: guests,
                    totalPrice: total,
                    specialStatus: "Window seat preferred (if applicable)."
                );
            }

            context.SaveChanges();
            /* =========================
   10) PAYMENT TRANSACTIONS
   ========================= */

            var demoBooking = context.Bookings
                .OrderByDescending(b => b.BookingId)
                .FirstOrDefault();

            if (demoBooking != null)
            {
                EnsurePaymentTransaction(
                    context,
                    demoBooking.BookingId,
                    transactionDate: new DateTime(2026, 02, 01, 10, 30, 0),
                    amount: demoBooking.TotalPrice,
                    paymentMethod: "Card",
                    transactionStatus: "Paid",
                    transactionReference: "DEMO-TXN-0001",
                    currency: "AUD"
                );
            }

            context.SaveChanges();

        }



        /* =========================
           HELPERS (LOCAL)
           ========================= */

        private static void EnsureRole(
            TourismWebsiteAssignmentContext context,
            string roleName,
            string description)
        {
            if (!context.Roles.Any(r => r.RoleName == roleName))
            {
                context.Roles.Add(new Role
                {
                    RoleName = roleName,
                    Description = description
                });
            }
        }

        private static void UpsertUser(
            TourismWebsiteAssignmentContext context,
            string fullName,
            string email,
            string username,
            string plainPassword,
            int roleId)
        {
            var user = context.Users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            string hash = HashMd5Aligned(plainPassword); // EXACT login match

            if (user == null)
            {
                context.Users.Add(new User
                {
                    FullName = fullName,
                    Email = email,
                    Username = username,
                    Password = hash,
                    RoleId = roleId
                });
            }
            else
            {
                user.FullName = fullName;
                user.Email = email;
                user.Username = username;
                user.Password = hash;   // overwrite legacy/plaintext
                user.RoleId = roleId;
            }
        }

        private static void EnsureBookingStatus(
            TourismWebsiteAssignmentContext context,
            string statusName,
            string description)
        {
            var status = context.BookingStatus
                .FirstOrDefault(s => s.StatusName == statusName);

            if (status == null)
            {
                context.BookingStatus.Add(new BookingStatus
                {
                    StatusName = statusName,
                    Description = description
                });
            }
            else
            {
                status.Description = description;
            }
        }
        private static void EnsureTravelAgency(
    TourismWebsiteAssignmentContext context,
    int userId,
    string agencyName,
    string licenseNumber,
    string contactNumber,
    string agencyAddress,
    string contactPerson,
    string phoneNumber,
    string agencyDescription,
    string logoUrl)
        {
            var agency = context.TravelAgencies
                .FirstOrDefault(a => a.UserId == userId);

            if (agency == null)
            {
                context.TravelAgencies.Add(new TravelAgency
                {
                    UserId = userId,
                    AgencyName = agencyName,
                    LicenseNumber = licenseNumber,
                    ContactNumber = contactNumber,
                    AgencyAddress = agencyAddress,
                    ContactPerson = contactPerson,
                    PhoneNumber = phoneNumber,
                    AgencyDescription = agencyDescription,
                    LogoUrl = logoUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // keep data consistent if Seed runs again
                agency.AgencyName = agencyName;
                agency.LicenseNumber = licenseNumber;
                agency.ContactNumber = contactNumber;
                agency.AgencyAddress = agencyAddress;
                agency.ContactPerson = contactPerson;
                agency.PhoneNumber = phoneNumber;
                agency.AgencyDescription = agencyDescription;
                agency.LogoUrl = logoUrl;
            }
        }
        private static void EnsureTravelPackage(
    TourismWebsiteAssignmentContext context,
    int agencyId,
    string packageTitle,
    string packageDescription,
    string destination,
    decimal pricePerPerson,
    int groupMaxSize,
    string inclusions,
    string exclusions,
    string itineraryDetails,
    string termsAndConditions)
        {
            // Uniqueness strategy: Agency + PackageTitle
            var pkg = context.TravelPackages.FirstOrDefault(p =>
                p.AgencyId == agencyId && p.PackageTitle == packageTitle);

            if (pkg == null)
            {
                context.TravelPackages.Add(new TravelPackage
                {
                    AgencyId = agencyId,
                    PackageTitle = packageTitle,
                    PackageDescription = packageDescription,
                    Destination = destination,
                    PricePerPerson = pricePerPerson,
                    GroupMaxSize = groupMaxSize,
                    Inclusions = inclusions,
                    Exclusions = exclusions,
                    ItineraryDetails = itineraryDetails,
                    TermsAndConditions = termsAndConditions,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Keep seed deterministic if rerun
                pkg.PackageDescription = packageDescription;
                pkg.Destination = destination;
                pkg.PricePerPerson = pricePerPerson;
                pkg.GroupMaxSize = groupMaxSize;
                pkg.Inclusions = inclusions;
                pkg.Exclusions = exclusions;
                pkg.ItineraryDetails = itineraryDetails;
                pkg.TermsAndConditions = termsAndConditions;
            }
        }
        private static void EnsureTourDate(
    TourismWebsiteAssignmentContext context,
    int packageId,
    DateTime startDate,
    DateTime endDate,
    int totalSlots,
    int availableSlots,
    string status,
    decimal priceAdjustment)
        {
            // Uniqueness strategy: Package + StartDate
            var tourDate = context.TourDates.FirstOrDefault(td =>
                td.PackageId == packageId &&
                td.StartDate == startDate);

            if (tourDate == null)
            {
                context.TourDates.Add(new TourDate
                {
                    PackageId = packageId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalSlots = totalSlots,
                    AvailableSlots = availableSlots,
                    Status = status,
                    PriceAdjustment = priceAdjustment
                });
            }
            else
            {
                // keep deterministic on re-run
                tourDate.EndDate = endDate;
                tourDate.TotalSlots = totalSlots;
                tourDate.AvailableSlots = availableSlots;
                tourDate.Status = status;
                tourDate.PriceAdjustment = priceAdjustment;
            }
        }
        private static void EnsurePackageImage(
    TourismWebsiteAssignmentContext context,
    int packageId,
    string imageUrl,
    string caption,
    DateTime uploadedAt)
        {
            // Uniqueness strategy: Package + ImageURL
            var image = context.PackageImages.FirstOrDefault(i =>
                i.PackageId == packageId && i.ImageURL == imageUrl);

            if (image == null)
            {
                context.PackageImages.Add(new PackageImage
                {
                    PackageId = packageId,
                    ImageURL = imageUrl,
                    Caption = caption,
                    UploadedAt = uploadedAt
                });
            }
            else
            {
                image.Caption = caption;
                image.UploadedAt = uploadedAt;
            }
        }
        private static void EnsureTouristProfile(
    TourismWebsiteAssignmentContext context,
    int userId,
    string fullName,
    string gender,
    DateTime dateOfBirth,
    string address,
    string nationality,
    string travelPreferences,
    string profileImageUrl)
        {
            var profile = context.TouristProfiles
                .FirstOrDefault(tp => tp.UserId == userId);

            if (profile == null)
            {
                context.TouristProfiles.Add(new TouristProfile
                {
                    UserId = userId,
                    FullName = fullName,
                    Gender = gender,
                    DateOfBirth = dateOfBirth,
                    Address = address,
                    Nationality = nationality,
                    TravelPreferences = travelPreferences,
                    ProfileImageUrl = profileImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                // deterministic if Seed runs again
                profile.FullName = fullName;
                profile.Gender = gender;
                profile.DateOfBirth = dateOfBirth;
                profile.Address = address;
                profile.Nationality = nationality;
                profile.TravelPreferences = travelPreferences;
                profile.ProfileImageUrl = profileImageUrl;
            }
        }

        private static void EnsureBooking(
    TourismWebsiteAssignmentContext context,
    int touristProfileId,
    int tourDateId,
    int bookingStatusId,
    DateTime bookingDate,
    int numberOfGuests,
    decimal totalPrice,
    string specialStatus)
        {
            // Uniqueness strategy: TouristProfile + TourDate
            var booking = context.Bookings.FirstOrDefault(b =>
                b.TouristProfileId == touristProfileId &&
                b.TourDateId == tourDateId);

            if (booking == null)
            {
                context.Bookings.Add(new Booking
                {
                    TouristProfileId = touristProfileId,
                    TourDateId = tourDateId,
                    BookingStatusId = bookingStatusId,
                    BookingDate = bookingDate,
                    NumberOfGuests = numberOfGuests,
                    TotalPrice = totalPrice,
                    SpecialStatus = specialStatus
                });
            }
            else
            {
                // deterministic if Seed runs again
                booking.BookingStatusId = bookingStatusId;
                booking.BookingDate = bookingDate;
                booking.NumberOfGuests = numberOfGuests;
                booking.TotalPrice = totalPrice;
                booking.SpecialStatus = specialStatus;
            }
        }
        private static void EnsurePaymentTransaction(
    TourismWebsiteAssignmentContext context,
    int bookingId,
    DateTime transactionDate,
    decimal amount,
    string paymentMethod,
    string transactionStatus,
    string transactionReference,
    string currency)
        {
            // Uniqueness strategy: BookingId (assume 1 payment per booking for demo)
            var txn = context.PaymentTransactions
                .FirstOrDefault(t => t.BookingId == bookingId);

            if (txn == null)
            {
                context.PaymentTransactions.Add(new PaymentTransactions
                {
                    BookingId = bookingId,
                    TransactionDate = transactionDate,
                    Amount = amount,
                    PaymentMethod = paymentMethod,
                    TransactionStatus = transactionStatus,
                    TransactionReference = transactionReference,
                    Currency = currency
                });
            }
            else
            {
                // deterministic if Seed runs again
                txn.TransactionDate = transactionDate;
                txn.Amount = amount;
                txn.PaymentMethod = paymentMethod;
                txn.TransactionStatus = transactionStatus;
                txn.TransactionReference = transactionReference;
                txn.Currency = currency;
            }
        }


        /* =========================
           HASH — SAME AS CONTROLLER
           ========================= */

        private static string HashMd5Aligned(string value)
        {
            if (value == null) value = "";
            value = value.Trim();

            using (var md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                byte[] hash = md5.ComputeHash(bytes);

                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2")); // lowercase hex
                return sb.ToString();
            }
        }
    }
}
