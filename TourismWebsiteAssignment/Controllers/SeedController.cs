using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class SeedController : Controller
    {
        private readonly TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();
        public ActionResult SeedRoles()
        {
            // Prevent duplicate seeding
            if (db.Roles.Any())
            {
                return Content("Roles already exist. Seeding skipped.");
            }

            var roles = new[]
            {
                new Role
                {
                    RoleName = "Admin",
                    Description = "System administrator with full access"
                },
                new Role
                {
                    RoleName = "Agent",
                    Description = "Travel agency user who manages packages and bookings"
                },
                new Role
                {
                    RoleName = "Tourist",
                    Description = "Customer who browses and books travel packages"
                }
            };

            db.Roles.AddRange(roles);
            db.SaveChanges();

            return Content("Roles seeded successfully.");
        }


        // GET: /DbSeed/SeedUsers
        public ActionResult SeedUsers()
        {
            // Must have Roles first
            var adminRole = db.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            var agentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Agent");
            var touristRole = db.Roles.FirstOrDefault(r => r.RoleName == "Tourist");

            if (adminRole == null || agentRole == null || touristRole == null)
            {
                return Content("Roles missing. Run /DbSeed/SeedRoles first.");
            }

            // Create if missing (no duplicates)
            CreateUserIfMissing(
                fullName: "System Admin",
                email: "admin@demo.com",
                username: "admin",
                passwordPlain: "Admin@123",
                roleId: adminRole.RoleId
            );

            CreateUserIfMissing(
                fullName: "Demo Agent",
                email: "agent@demo.com",
                username: "agent",
                passwordPlain: "Agent@123",
                roleId: agentRole.RoleId
            );

            CreateUserIfMissing(
                fullName: "Demo Tourist",
                email: "tourist@demo.com",
                username: "tourist",
                passwordPlain: "Tourist@123",
                roleId: touristRole.RoleId
            );

            db.SaveChanges();
            return Content("Users seeded (created if missing).");
        }

        private void CreateUserIfMissing(string fullName, string email, string username, string passwordPlain, int roleId)
        {
            // Avoid duplicates by Username OR Email
            bool exists = db.Users.Any(u =>
                u.Username.ToLower() == username.ToLower() ||
                u.Email.ToLower() == email.ToLower()
            );

            if (exists) return;

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                Password = passwordPlain, // Replace with hashing if your login expects hashed passwords
                RoleId = roleId
            };

            db.Users.Add(user);
        }

        // GET: /DbSeed/SeedPackages
        public ActionResult SeedPackages()
        {
            // Must have at least 1 agency because AgencyId is required
            var agencies = db.TravelAgencies.ToList();
            if (!agencies.Any())
            {
                return Content("No TravelAgencies found. Seed TravelAgency first, then run SeedPackages.");
            }

            // If packages already exist, skip (or you can change logic to "create if missing")
            if (db.TravelPackages.Any())
            {
                return Content("TravelPackages already exist. Seeding skipped.");
            }

            var now = DateTime.Now;

            // Use available agencies (cycle them)
            int agencyCount = agencies.Count;

            var packages = new[]
            {
                new TravelPackage
                {
                    AgencyId = agencies[0 % agencyCount].AgencyId,
                    PackageTitle = "Sydney City Explorer",
                    PackageDescription = "A guided tour covering Sydney Opera House, Harbour Bridge, and key city highlights.",
                    Destination = "Sydney",
                    PricePerPerson = 299m,
                    GroupMaxSize = 15,
                    Inclusions = "Guide, transport, entry fees (selected attractions)",
                    Exclusions = "Meals, personal expenses",
                    ItineraryDetails = "Morning pickup, city landmarks, harbour area walk, afternoon drop-off.",
                    TermsAndConditions = "Non-refundable within 48 hours of departure. ID required.",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TravelPackage
                {
                    AgencyId = agencies[1 % agencyCount].AgencyId,
                    PackageTitle = "Blue Mountains Day Trip",
                    PackageDescription = "A full-day scenic trip to the Blue Mountains with lookout points and nature walks.",
                    Destination = "Blue Mountains",
                    PricePerPerson = 199m,
                    GroupMaxSize = 20,
                    Inclusions = "Transport, guide",
                    Exclusions = "Meals, optional activities",
                    ItineraryDetails = "Early departure, scenic stops, short hikes, evening return.",
                    TermsAndConditions = "Weather conditions may affect itinerary. No-shows are charged.",
                    CreatedAt = now,
                    UpdatedAt = now
                },
                new TravelPackage
                {
                    AgencyId = agencies[2 % agencyCount].AgencyId,
                    PackageTitle = "Melbourne Culture & Laneways",
                    PackageDescription = "Explore Melbourne’s laneways, cafés, street art, and cultural hotspots.",
                    Destination = "Melbourne",
                    PricePerPerson = 249m,
                    GroupMaxSize = 12,
                    Inclusions = "Guide",
                    Exclusions = "Food/drinks purchases",
                    ItineraryDetails = "City walk, laneway route, cultural stops, free time for cafés.",
                    TermsAndConditions = "Reschedule allowed once with 72 hours notice.",
                    CreatedAt = now,
                    UpdatedAt = now
                }
            };

            db.TravelPackages.AddRange(packages);
            db.SaveChanges();

            return Content("TravelPackages seeded successfully.");
        }

        public ActionResult SeedAgencies()
        {
            var agentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Agent");
            if (agentRole == null)
            {
                return Content("Agent role missing. Run /DbSeed/SeedRoles first.");
            }

            // Ensure we have at least 2 Agent users to attach agencies to
            EnsureAgentUserExists(
                fullName: "Agent One",
                email: "agent1@demo.com",
                username: "agent1",
                passwordPlain: "Agent@123",
                roleId: agentRole.RoleId
            );

            EnsureAgentUserExists(
                fullName: "Agent Two",
                email: "agent2@demo.com",
                username: "agent2",
                passwordPlain: "Agent@123",
                roleId: agentRole.RoleId
            );

            db.SaveChanges();

            var agentUsers = db.Users.Where(u => u.RoleId == agentRole.RoleId).ToList();
            if (!agentUsers.Any())
            {
                return Content("No Agent users found. SeedUsers/agent creation failed.");
            }

            var now = DateTime.Now;
            int created = 0;

            // Create one agency per agent user (if missing)
            foreach (var agent in agentUsers)
            {
                bool agencyExistsForUser = db.TravelAgencies.Any(a => a.UserId == agent.UserId);
                if (agencyExistsForUser) continue;

                // LicenseNumber must be unique for realism; keep it deterministic.
                string license = $"LIC-{agent.UserId:D5}";

                // Guard against any existing license conflict
                if (db.TravelAgencies.Any(a => a.LicenseNumber == license))
                {
                    license = $"LIC-{agent.UserId:D5}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";
                }

                db.TravelAgencies.Add(new TravelAgency
                {
                    UserId = agent.UserId,
                    AgencyName = $"{agent.FullName} Travel",
                    LicenseNumber = license,
                    ContactNumber = "0400000000",
                    AgencyAddress = "Sydney, NSW, Australia",
                    ContactPerson = agent.FullName,
                    PhoneNumber = "0400000000",
                    AgencyDescription = "Demo travel agency seeded for development and testing.",
                    LogoUrl = "/Uploads/Agencies/default-logo.png",
                    CreatedAt = now,
                    UpdatedAt = now
                });

                created++;
            }

            db.SaveChanges();

            return Content($"TravelAgencies seeded successfully. Created: {created}");
        }

        private void EnsureAgentUserExists(string fullName, string email, string username, string passwordPlain, int roleId)
        {
            bool exists = db.Users.Any(u =>
                u.Username.ToLower() == username.ToLower() ||
                u.Email.ToLower() == email.ToLower()
            );

            if (exists) return;

            db.Users.Add(new User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                Password = passwordPlain, // change to hashed if your login expects hashed passwords
                RoleId = roleId
            });
        }

        // GET: /DbSeed/SeedPackageImages
        public ActionResult SeedPackageImages()
        {
            var packages = db.TravelPackages.ToList();
            if (!packages.Any())
            {
                return Content("No TravelPackages found. Run /DbSeed/SeedPackages first.");
            }

            var now = DateTime.Now;
            int created = 0;

            foreach (var p in packages)
            {
                // Use stable, deterministic URLs/paths per package
                // You can replace these with real URLs later.
                var imgs = new[]
                {
                    new { Url = $"/Uploads/Packages/{p.PackageId}/cover.jpg", Caption = $"{p.PackageTitle} - Cover" },
                    new { Url = $"/Uploads/Packages/{p.PackageId}/gallery-1.jpg", Caption = $"{p.Destination} - Gallery 1" },
                    new { Url = $"/Uploads/Packages/{p.PackageId}/gallery-2.jpg", Caption = $"{p.Destination} - Gallery 2" }
                };

                foreach (var img in imgs)
                {
                    bool exists = db.PackageImages.Any(x =>
                        x.PackageId == p.PackageId &&
                        x.ImageURL.ToLower() == img.Url.ToLower()
                    );

                    if (exists) continue;

                    db.PackageImages.Add(new PackageImage
                    {
                        PackageId = p.PackageId,
                        ImageURL = img.Url,
                        Caption = img.Caption,
                        UploadedAt = now
                    });

                    created++;
                }
            }

            db.SaveChanges();
            return Content($"PackageImages seeded successfully. Created: {created}");
        }


        // GET: /DbSeed/SeedTourDates
        public ActionResult SeedTourDates()
        {
            var packages = db.TravelPackages.ToList();
            if (!packages.Any())
            {
                return Content("No TravelPackages found. Run /DbSeed/SeedPackages first.");
            }

            int created = 0;

            // Create 3 future tour dates per package
            foreach (var p in packages)
            {
                // Deterministic schedule per package so repeated runs are stable
                // (but we still protect against duplicates)
                var baseStart = DateTime.Today.AddDays(7 + (p.PackageId % 5)); // starts 7-11 days from now

                var tours = new[]
                {
                    new { Start = baseStart,               End = baseStart.AddDays(1), Slots = 20, Adj = 0m },
                    new { Start = baseStart.AddDays(14),   End = baseStart.AddDays(16), Slots = 15, Adj = 25m },
                    new { Start = baseStart.AddDays(30),   End = baseStart.AddDays(33), Slots = 25, Adj = -10m }
                };

                foreach (var t in tours)
                {
                    // Ensure valid rules
                    int totalSlots = t.Slots;
                    int availableSlots = Math.Max(0, totalSlots - 2); // leave 2 as "taken" for realism

                    bool exists = db.TourDates.Any(td =>
                        td.PackageId == p.PackageId &&
                        td.StartDate == t.Start &&
                        td.EndDate == t.End
                    );

                    if (exists) continue;

                    db.TourDates.Add(new TourDate
                    {
                        PackageId = p.PackageId,
                        StartDate = t.Start,
                        EndDate = t.End,
                        TotalSlots = totalSlots,
                        AvailableSlots = availableSlots,
                        Status = "Open",
                        PriceAdjustment = t.Adj
                    });

                    created++;
                }
            }

            db.SaveChanges();
            return Content($"TourDates seeded successfully. Created: {created}");
        }


        // GET: /DbSeed/SeedTouristProfiles
        public ActionResult SeedTouristProfiles()
        {
            var touristRole = db.Roles.FirstOrDefault(r => r.RoleName == "Tourist");
            if (touristRole == null)
            {
                return Content("Tourist role missing. Run /DbSeed/SeedRoles first.");
            }

            // Ensure a few tourist users exist
            EnsureUserExists("Demo Tourist 1", "tourist1@demo.com", "tourist1", "Tourist@123", touristRole.RoleId);
            EnsureUserExists("Demo Tourist 2", "tourist2@demo.com", "tourist2", "Tourist@123", touristRole.RoleId);
            EnsureUserExists("Demo Tourist 3", "tourist3@demo.com", "tourist3", "Tourist@123", touristRole.RoleId);

            db.SaveChanges();

            var touristUsers = db.Users.Where(u => u.RoleId == touristRole.RoleId).ToList();
            if (!touristUsers.Any())
            {
                return Content("No Tourist users found. Tourist user creation failed.");
            }

            int created = 0;
            var now = DateTime.Now;

            // Create 1 TouristProfile per Tourist user (if missing)
            foreach (var u in touristUsers)
            {
                bool exists = db.TouristProfiles.Any(tp => tp.UserId == u.UserId);
                if (exists) continue;

                // Deterministic but varied demo values
                var profile = new TouristProfile
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Gender = "Other",
                    DateOfBirth = new DateTime(1999, 1, 1).AddDays(u.UserId % 3650),
                    Address = "Sydney, NSW, Australia",
                    Nationality = "Australian",
                    TravelPreferences = "City tours, nature, food experiences",
                    ProfileImageUrl = "/Uploads/Tourists/default-profile.png",
                    CreatedAt = now,
                    UpdatedAt = now
                };

                db.TouristProfiles.Add(profile);
                created++;
            }

            db.SaveChanges();
            return Content($"TouristProfiles seeded successfully. Created: {created}");
        }

        private void EnsureUserExists(string fullName, string email, string username, string passwordPlain, int roleId)
        {
            bool exists = db.Users.Any(u =>
                u.Username.ToLower() == username.ToLower() ||
                u.Email.ToLower() == email.ToLower()
            );

            if (exists) return;

            db.Users.Add(new User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                Password = passwordPlain, // change to hash if your login expects hashed passwords
                RoleId = roleId
            });
        }



        // GET: /DbSeed/SeedBookingStatuses
        public ActionResult SeedBookingStatuses()
        {
            if (db.BookingStatus.Any())
            {
                return Content("BookingStatus records already exist. Seeding skipped.");
            }

            db.BookingStatus.Add(new BookingStatus
            {
                StatusName = "Pending",
                Description = "Booking created but not yet confirmed"
            });

            db.BookingStatus.Add(new BookingStatus
            {
                StatusName = "Confirmed",
                Description = "Booking confirmed and slots reserved"
            });

            db.BookingStatus.Add(new BookingStatus
            {
                StatusName = "Cancelled",
                Description = "Booking cancelled by user or administrator"
            });

            db.SaveChanges();
            return Content("BookingStatus seeded successfully.");
        }
        // GET: /DbSeed/SeedBookings
        public ActionResult SeedBookings()
        {
            // Dependencies
            var tourists = db.TouristProfiles.ToList();
            if (!tourists.Any())
                return Content("No TouristProfiles found. Run /DbSeed/SeedTouristProfiles first.");

            var tourDates = db.TourDates
                .Where(td => td.Status == "Open" && td.AvailableSlots > 0)
                .ToList();

            if (!tourDates.Any())
                return Content("No open TourDates with AvailableSlots > 0 found. Seed TourDates first.");

            if (!db.BookingStatus.Any())
                return Content("No BookingStatuses found. Run /DbSeed/SeedBookingStatuses first.");

            // Prefer Confirmed, else first
            var confirmed = db.BookingStatus.FirstOrDefault(s => s.StatusName == "Confirmed");
            int statusId = (confirmed ?? db.BookingStatus.First()).BookingStatusId;

            int created = 0;
            var rng = new Random();
            var today = DateTime.Today;

            // Create up to N bookings (one per tourist, matched to a tour date)
            int n = Math.Min(tourists.Count, tourDates.Count);

            for (int i = 0; i < n; i++)
            {
                var tourist = tourists[i];
                var tourDate = tourDates[i];

                // Avoid duplicates: one booking per tourist per tourdate
                bool exists = db.Bookings.Any(b =>
                    b.TouristProfileId == tourist.TouristProfileId &&
                    b.TourDateId == tourDate.TourDateId
                );

                if (exists) continue;

                // Guests must be <= AvailableSlots
                int maxGuests = Math.Min(5, tourDate.AvailableSlots);
                int guests = rng.Next(1, maxGuests + 1);

                // Calculate total price:
                // price = (package price + adjustment) * guests
                // Ensure TravelPackage is accessible (FK navigation)
                var td = db.TourDates
                    .Where(x => x.TourDateId == tourDate.TourDateId)
                    .Select(x => new
                    {
                        x.TourDateId,
                        x.AvailableSlots,
                        x.PriceAdjustment,
                        PackagePrice = x.TravelPackage.PricePerPerson
                    })
                    .First();

                decimal unitPrice = td.PackagePrice + td.PriceAdjustment;
                if (unitPrice < 0) unitPrice = 0;
                decimal total = unitPrice * guests;

                db.Bookings.Add(new Booking
                {
                    TouristProfileId = tourist.TouristProfileId,
                    TourDateId = tourDate.TourDateId,
                    BookingStatusId = statusId,
                    BookingDate = today,
                    NumberOfGuests = guests,
                    TotalPrice = total,
                    SpecialStatus = "No special requests."
                });

                // Decrement availability
                tourDate.AvailableSlots -= guests;

                created++;
            }

            db.SaveChanges();
            return Content($"Bookings seeded successfully. Created: {created}");
        }

        public ActionResult SeedFeedback()
        {
            var bookings = db.Bookings.ToList();
            if (!bookings.Any())
                return Content("No Bookings found. Seed Bookings first, then run SeedFeedback.");

            var tourists = db.TouristProfiles.ToList();
            if (!tourists.Any())
                return Content("No TouristProfiles found. Seed TouristProfiles first, then run SeedFeedback.");

            int created = 0;
            var rng = new Random();
            var now = DateTime.Now;

            // Create feedback for up to N bookings
            foreach (var b in bookings)
            {
                // One feedback per booking (typical rule)
                bool exists = db.Feedbacks.Any(f => f.BookingId == b.BookingId);
                if (exists) continue;

                // Prefer: feedback tourist = booking tourist
                int touristId = b.TouristProfileId;

                // Safety: if tourist profile is missing for any reason, fallback
                if (!tourists.Any(t => t.TouristProfileId == touristId))
                {
                    touristId = tourists.First().TouristProfileId;
                }

                int rating = rng.Next(3, 6); // 3..5 for reasonable demo data

                string comment =
                    rating == 5 ? "Excellent experience. Everything was well organised." :
                    rating == 4 ? "Good trip overall. Would recommend with minor improvements." :
                                  "Average experience. Some parts could be improved.";

                db.Feedbacks.Add(new Feedback
                {
                    TouristId = touristId,
                    BookingId = b.BookingId,
                    Rating = rating,
                    Comments = comment,
                    SubmittedAt = now
                });

                created++;
            }

            db.SaveChanges();
            return Content($"Feedback seeded successfully. Created: {created}");
        }

        // GET: /DbSeed/SeedPayments
        public ActionResult SeedPayments()
        {
            var bookings = db.Bookings.ToList();
            if (!bookings.Any())
                return Content("No Bookings found. Seed Bookings first, then run SeedPayments.");

            int created = 0;
            var now = DateTime.Now;
            var rng = new Random();

            string[] methods = { "Card", "PayPal", "BankTransfer" };
            string[] statuses = { "Success", "Pending", "Failed" };

            foreach (var b in bookings)
            {
                // One payment per booking (typical)
                bool exists = db.PaymentTransactions.Any(p => p.BookingId == b.BookingId);
                if (exists) continue;

                string method = methods[rng.Next(methods.Length)];

                // For demo: mostly success
                string status = rng.Next(0, 10) < 8 ? "Success" : statuses[rng.Next(statuses.Length)];

                // Reference should be unique-ish
                string reference = $"TXN-{b.BookingId:D6}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                db.PaymentTransactions.Add(new PaymentTransactions
                {
                    BookingId = b.BookingId,
                    TransactionDate = now,
                    Amount = b.TotalPrice,          // best: match booking amount
                    PaymentMethod = method,
                    TransactionStatus = status,
                    TransactionReference = reference,
                    Currency = "AUD"
                });

                created++;
            }

            db.SaveChanges();
            return Content($"PaymentTransactions seeded successfully. Created: {created}");
        }


        public ActionResult RunAllSeed()
        {
            // Order MATTERS – do not change
            SeedRoles();
            SeedUsers();
            SeedAgencies();
            SeedPackages();
            SeedPackageImages();
            SeedTourDates();
            SeedTouristProfiles();
            SeedBookingStatuses();
            SeedBookings();
            SeedFeedback();
            SeedPayments();

            return Content("All seed operations executed in correct order.");
        }
    }
}