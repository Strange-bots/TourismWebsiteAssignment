using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("PackageImage")]
    public class PackageImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int PackageId { get; set; }

        [ForeignKey("PackageId")]
        public virtual TravelPackage TravelPackage { get; set; }
        public virtual ICollection<PackageImage> PackageImages { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Image URL")]
        public string ImageURL { get; set; }

        [Required]
        [Display(Name = "Uploaded At")]
        public DateTime UploadedAt { get; set; }

        [StringLength(255)]
        public string Caption { get; set; }
    }
}