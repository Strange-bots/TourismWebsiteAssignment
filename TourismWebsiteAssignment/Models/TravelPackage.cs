using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("TravelPackage")]
    public class TravelPackage
    {
        [Key]
        public int PackageId { get; set; }

        [Required]
        public int AgencyId { get; set; }

        [ForeignKey("AgencyId")]
        public virtual TravelAgency TravelAgency { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Package Title")]
        public string PackageTitle { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Package Description")]
        [DataType(DataType.MultilineText)]
        public string PackageDescription { get; set; }

        [Required]
        [StringLength(100)]
        public string Destination { get; set; }

        [Required]
        [Display(Name = "Price Per Person")]
        [Range(0.01, 999999.99)]
        [DataType(DataType.Currency)]
        public decimal PricePerPerson { get; set; }

        [Required]
        [Display(Name = "Group Max Size")]
        [Range(1, 1000)]
        public int GroupMaxSize { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Inclusions { get; set; }

        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Exclusions { get; set; }

        [StringLength(2000)]
        [Display(Name = "Itinerary Details")]
        [DataType(DataType.MultilineText)]
        public string ItineraryDetails { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "Terms and Conditions")]
        [DataType(DataType.MultilineText)]
        public string TermsAndConditions { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }
    }
}