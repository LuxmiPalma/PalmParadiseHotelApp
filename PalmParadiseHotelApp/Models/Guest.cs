using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmParadiseHotelApp.Models
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; } // Primary Key: Unique identifier for the guest.

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; } // Guest's name.

        [MaxLength(15)]
        public string? ContactNumber { get; set; } // Guest's contact number.
       
        [MaxLength(100)]
        public string? Email { get; set; } // Guest's email.
        public List<Booking> Bookings { get; set; } = new List<Booking>(); // Navigation property for Bookings.
    }
}
