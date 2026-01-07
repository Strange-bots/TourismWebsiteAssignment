using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("TouristProfile")]
    public class TouristProfile
    {
        [Key]
        public int TouristProfileId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Nationality { get; set; }

        [StringLength(500)]
        [Display(Name = "Travel Preferences")]
        public string TravelPreferences { get; set; }

        [StringLength(255)]
        [Display(Name = "Profile Image")]
        public string ProfileImageUrl { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}