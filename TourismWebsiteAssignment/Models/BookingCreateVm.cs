using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismWebsiteAssignment.Models
{
    public class BookingCreateVm
    {
        [Required]
        public int TourDateId { get; set; }

        [Required]
        public int BookingStatusId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required, Range(1, 100)]
        public int NumberOfGuests { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required, StringLength(500)]
        public string SpecialStatus { get; set; }
    }

}