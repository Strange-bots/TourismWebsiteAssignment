using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("PaymentTransactions")]
    public class PaymentTransactions
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Transaction Status")]
        public string TransactionStatus { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Transaction Reference")]
        public string TransactionReference { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; }
    }
}
