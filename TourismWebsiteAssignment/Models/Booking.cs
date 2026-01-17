using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("Booking")]
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int TouristProfileId { get; set; }

        [Required]
        public int TourDateId { get; set; }

        [Required]
        public int BookingStatusId { get; set; }

        [ForeignKey("TouristProfileId")]
        public virtual TouristProfile Tourist { get; set; }

        [ForeignKey("TourDateId")]
        public virtual TourDate TourDate { get; set; }

        [ForeignKey("BookingStatusId")]
        public virtual BookingStatus BookingStatus { get; set; }

        [Required]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        [Display(Name = "Number of Guests")]
        [Range(1, 100)]
        public int NumberOfGuests { get; set; }

        [Required]
        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Special Requests")]
        [DataType(DataType.MultilineText)]
        public string SpecialStatus { get; set; }
    }
}