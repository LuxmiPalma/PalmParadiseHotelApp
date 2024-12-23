using Microsoft.EntityFrameworkCore;
using PalmParadiseHotelApp.Data;
using PalmParadiseHotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmParadiseHotelApp.Helpers
{
    public class BookingHistoryHelper
    {
        private readonly DbContextOptions<HotelDbContext> _options;

        // Constructor to inject DbContext options
        public BookingHistoryHelper(DbContextOptions<HotelDbContext> options)
        {
            _options = options;
        }

        /// <summary>
        /// Retrieves all bookings for a specific guest.
        /// </summary>
        public List<Booking> GetBookingHistory(int guestId)
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    return context.Bookings
                        .Where(b => b.GuestId == guestId)
                        .Include(b => b.Room) // Include related Room data
                        .ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to retrieve booking history. {ex.Message}");
                    return new List<Booking>();
                }
            }
        }

        /// <summary>
        /// Displays the booking history for a specific guest.
        /// </summary>
        public void DisplayBookingHistory(int guestId)
        {
            var bookings = GetBookingHistory(guestId);

            if (bookings.Count == 0)
            {
                Console.WriteLine("\nNo bookings found for this guest.");
                return;
            }

            Console.WriteLine("=====================================================================");
            Console.WriteLine($"{"Booking ID",-12} {"Room",-10} {"Check-In",-15} {"Check-Out",-15} {"Price",-10}");
            Console.WriteLine("=====================================================================");

            foreach (var booking in bookings)
            {
                Console.WriteLine($"{booking.BookingId,-12} {booking.Room.RoomNumber,-10} {booking.CheckInDate.ToShortDateString(),-15} {booking.CheckOutDate.ToShortDateString(),-15} {booking.Room.Price + "kr",-10}");
            }

            Console.WriteLine("=====================================================================");
        }
    }
}

