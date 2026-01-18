using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("Feedback")]
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int TouristId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("TouristId")]
        public virtual TouristProfile Tourist { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Required]
        [Display(Name = "Submitted At")]
        public DateTime SubmittedAt { get; set; }
    }
}
