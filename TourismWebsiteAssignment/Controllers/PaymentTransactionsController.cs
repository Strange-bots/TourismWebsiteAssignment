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
        public ActionResult Create()
        {
            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus");
            return View();
        }

        // POST: PaymentTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TransactionId,BookingId,TransactionDate,Amount,PaymentMethod,TransactionStatus,TransactionReference,Currency")] PaymentTransactions paymentTransactions)
        {
            if (ModelState.IsValid)
            {
                db.PaymentTransactions.Add(paymentTransactions);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.BookingId = new SelectList(db.Bookings, "BookingId", "SpecialStatus", paymentTransactions.BookingId);
            return View(paymentTransactions);
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
