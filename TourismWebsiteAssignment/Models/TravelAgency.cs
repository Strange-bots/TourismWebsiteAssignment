using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("TravelAgency")]
    public class TravelAgency
    {
        [Key]
        public int AgencyId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Agency Name")]
        public string AgencyName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Contact Number")]
        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Agency Address")]
        public string AgencyAddress { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Agency Description")]
        [DataType(DataType.MultilineText)]
        public string AgencyDescription { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        [StringLength(255)]
        [Display(Name = "Logo URL")]
        public string LogoUrl { get; set; }
    }
}