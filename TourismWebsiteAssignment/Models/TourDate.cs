using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("TourDate")]
    public class TourDate
    {
        [Key]
        public int TourDateId { get; set; }

        [Required]
        public int PackageId { get; set; }

        [ForeignKey("PackageId")]
        public virtual TravelPackage TravelPackage { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Available Slots")]
        [Range(0, 10000)]
        public int AvailableSlots { get; set; }

        [Required]
        [Display(Name = "Total Slots")]
        [Range(1, 10000)]
        public int TotalSlots { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Price Adjustment")]
        public decimal PriceAdjustment { get; set; }
    }
}