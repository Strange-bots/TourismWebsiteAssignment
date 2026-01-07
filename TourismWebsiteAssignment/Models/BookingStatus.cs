using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismWebsiteAssignment.Models
{
    [Table("BookingStatus")]
    public class BookingStatus
    {
        [Key]
        public int BookingStatusId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Status Name")]
        public string StatusName { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }
    }
}