using System.Collections.Generic;
using TourismWebsiteAssignment.Models;

namespace TourismWebsiteAssignment.Models
{
    public class ShowData
    {
        public IEnumerable<TourDate> TourDates { get; set; }
        public IEnumerable<TravelPackage> TravelPackages { get; set; }
        public IEnumerable<Feedback> Feedbacks { get; set; }
    }
}
