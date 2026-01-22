using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourismWebsiteAssignment.Data;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Controllers
{
    public class PaymentTransactionsController : Controller
    {
        private TourismWebsiteAssignmentContext db = new TourismWebsiteAssignmentContext();

        // GET: PaymentTransactions
        public async Task<ActionResult> Index()
        {
            var paymentTransactions = db.PaymentTransactions.Include(p => p.Booking);
            return View(await paymentTransactions.ToListAsync());
        }
        
        // GET: PaymentTransactions
        public async Task<ActionResult> OnlyView()
        {
            var paymentTransactions = db.PaymentTransactions.Include(p => p.Booking);
            return View(await paymentTransactions.ToListAsync());
        }

        // GET: PaymentTransactions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }
            return View(paymentTransactions);
        }

        // GET: PaymentTransactions/Create
        public ActionResult Create(int bookingId)
        {
            var booking = db.Bookings.Find(bookingId);
            if (booking == null) return HttpNotFound();

            // Dropdown
            ViewBag.PaymentMethod = new SelectList(new[]
            {
                "Visa", "MasterCard", "Amex", "PayPal", "Bank Transfer"
            });

            // Display only
            ViewBag.Amount = booking.TotalPrice;
            ViewBag.Currency = "AUD";

            // Model contains BookingId for HiddenFor
            return View(new PaymentTransactions { BookingId = bookingId });
        }

        // POST: PaymentTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "BookingId,PaymentMethod")] PaymentTransactions payment)
        {
            // Ignore validation for fields user doesn't submit
            ModelState.Remove("TransactionDate");
            ModelState.Remove("Amount");
            ModelState.Remove("TransactionStatus");
            ModelState.Remove("TransactionReference");
            ModelState.Remove("Currency");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .Select(kvp => kvp.Key + ": " + string.Join(", ", kvp.Value.Errors.Select(e => e.ErrorMessage)))
                    .ToList();

                ModelState.AddModelError("", "ModelState invalid: " + string.Join(" | ", errors));

                ViewBag.PaymentMethod = new SelectList(new[] { "Visa", "MasterCard", "Amex", "PayPal", "Bank Transfer" }, payment.PaymentMethod);
                return View(payment);
            }


            var booking = db.Bookings.Find(payment.BookingId);
            if (booking == null) return HttpNotFound();

            payment.TransactionDate = DateTime.Now;
            payment.Amount = booking.TotalPrice;
            payment.Currency = "AUD";
            payment.TransactionStatus = "Paid";
            payment.TransactionReference = GenerateReference();

            db.PaymentTransactions.Add(payment);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "PaymentTransactions");
        }
        private string GenerateReference()
        {
            return $"PAY-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        // GET: PaymentTransactions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
            return View(paymentTransactions);
        }

        // POST: PaymentTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TransactionId,BookingId,TransactionDate,Amount,PaymentMethod,TransactionStatus,TransactionReference,Currency")] PaymentTransactions paymentTransactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentTransactions).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
            return View(paymentTransactions);
        }

        // GET: PaymentTransactions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            if (paymentTransactions == null)
            {
                return HttpNotFound();
            }
            return View(paymentTransactions);
        }

        // POST: PaymentTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PaymentTransactions paymentTransactions = await db.PaymentTransactions.FindAsync(id);
            db.PaymentTransactions.Remove(paymentTransactions);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        //Viewing payments that are made for this agent packages
        public async Task<ActionResult> AgentViewPayment()
        {

            if (Session["UserId"] == null)
                return RedirectToAction("Index", "LoginRegistration");

            int userId = (int)Session["UserId"];

            var payments = db.PaymentTransactions
                .Include(p => p.Booking)
                .Include(p => p.Booking.TourDate)
                .Include(p => p.Booking.TourDate.TravelPackage)
                .Include(p => p.Booking.TourDate.TravelPackage.TravelAgency)
                .Where(p => p.Booking.TourDate.TravelPackage.TravelAgency.UserId == userId)
                .OrderByDescending(p => p.TransactionDate)
                .ToList();

            if (!payments.Any())
            {
                // either show empty page, or redirect somewhere sensible
                return View(payments); // empty list -> show "no payments yet"
                                       // OR: return RedirectToAction("AgentDashboard", "Agent");
            }

            return View(payments);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
