﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PalmParadiseHotelApp.Models;


namespace PalmParadiseHotelApp.Data
{
    public class DataInitializer
    {
        public void MigrateAndSeed(HotelDbContext dbContext)
        {
            try
            {
                // Apply migrations
                dbContext.Database.Migrate();
                Console.WriteLine("Migrations applied successfully.");

                // Seed data with error logging
                SeedRooms(dbContext);
                SeedGuests(dbContext);
                SeedEmployees(dbContext);
                SeedServices(dbContext);
                SeedBookingServices(dbContext);
                SeedBookings(dbContext);
                SeedFeedback(dbContext);
                SeedPaymentInvoices(dbContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migrations or seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private void SeedRooms(HotelDbContext dbContext)
        {
            try
            {
                if (!dbContext.Rooms.Any())
                {
                    Console.WriteLine("Seeding Rooms...");
                    dbContext.Rooms.AddRange(
                        new Room { Type = "Suite", Price = 5000, IsAvailable = true, ExtraBeds = 0, Size = 30, MaxGuests = 2 },
                        new Room { Type = "Deluxe", Price = 3000, IsAvailable = true, ExtraBeds = 1, Size = 25, MaxGuests = 3 },
                        new Room { Type = "Standard", Price = 2000, IsAvailable = true, ExtraBeds = 1, Size = 20, MaxGuests = 2 },
                        new Room { Type = "Economy", Price = 1000, IsAvailable = true, ExtraBeds = 0, Size = 15, MaxGuests = 1 }
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Rooms seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Rooms already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during room seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private void SeedGuests(HotelDbContext dbContext)
        {
            try
            {
                if (!dbContext.Guests.Any())
                {
                    Console.WriteLine("Seeding Guests...");
                    dbContext.Guests.AddRange(
                new Guest { Name = "John Doe", ContactNumber = "123456789", Email = "john.doe@example.com" },
                new Guest { Name = "Jane Smith", ContactNumber = "987654321", Email = "jane.smith@example.com" },
                new Guest { Name = "Alice Johnson", ContactNumber = "555123456", Email = "alice.johnson@example.com" },
                new Guest { Name = "Bob Brown", ContactNumber = "555987654", Email = "bob.brown@example.com" }
            );
                    dbContext.SaveChanges();
                    Console.WriteLine("Guests seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Guests already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during guest seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private void SeedBookings(HotelDbContext dbContext)
        {
            try
            {
                Console.WriteLine("Seeding Bookings...");
                var room1 = dbContext.Rooms.OrderBy(r => r.RoomNumber).FirstOrDefault();
                var room2 = dbContext.Rooms.OrderBy(r => r.RoomNumber).Skip(1).FirstOrDefault();
                var guest1 = dbContext.Guests.OrderBy(g => g.GuestId).FirstOrDefault();
                var guest2 = dbContext.Guests.OrderBy(g => g.GuestId).Skip(1).FirstOrDefault();
                var employee = dbContext.Employees.FirstOrDefault(); // Declare employee here

                if (room1 == null || room2 == null || guest1 == null || guest2 == null)
                {
                    Console.WriteLine("Cannot seed bookings. Dependencies missing.");
                    return;
                }

                // Seed only if no bookings exist
                if (!dbContext.Bookings.Any())
                {
                    dbContext.Bookings.AddRange(
                        new Booking
                        {
                            RoomNumber = room1.RoomNumber,
                            GuestId = guest1.GuestId,
                            CheckInDate = DateTime.Today.AddDays(1),
                            CheckOutDate = DateTime.Today.AddDays(5),
                            EmployeeId = employee.EmployeeId // Assign EmployeeId
                        },
                        new Booking
                        {
                            RoomNumber = room2.RoomNumber,
                            GuestId = guest2.GuestId,
                            CheckInDate = DateTime.Today.AddDays(2),
                            CheckOutDate = DateTime.Today.AddDays(6),
                            EmployeeId = employee.EmployeeId // Assign EmployeeId
                        }
                    );

                    dbContext.SaveChanges();
                    Console.WriteLine("Bookings seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Bookings already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during booking seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
        private void SeedBookingServices(HotelDbContext dbContext)
        {
            try
            {
                if (!dbContext.BookingServices.Any())
                {
                    Console.WriteLine("Seeding Booking Services...");

                    // Fetch existing Booking and Service records
                    var booking = dbContext.Bookings.FirstOrDefault();
                    var services = dbContext.Services.Take(2).ToList(); // Use first two services

                    if (booking == null || !services.Any())
                    {
                        Console.WriteLine("Bookings or Services are missing. Cannot seed Booking Services.");
                        return;
                    }

                    // Add sample BookingService entries
                    dbContext.BookingServices.AddRange(
                        new BookingService
                        {
                            BookingId = booking.BookingId,
                            ServiceId = services[0].ServiceId,
                            Quantity = 1,
                            ServiceName = services[0].Name
                        },
                        new BookingService
                        {
                            BookingId = booking.BookingId,
                            ServiceId = services[1].ServiceId,
                            Quantity = 2,
                            ServiceName = services[1].Name
                        }
                    );

                    dbContext.SaveChanges();
                    Console.WriteLine("Booking Services seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Booking Services already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Booking Services seeding: {ex.Message}");
            }
        }

        private void SeedEmployees(HotelDbContext dbContext)
        {
            try
            {
                if (!dbContext.Employees.Any())
                {
                    dbContext.Employees.AddRange(
                        new Employee { Name = "Anna Johnson", Role = "Receptionist", ContactNumber = "123456789", Email = "anna.johnson@hotel.com" },
                        new Employee { Name = "Tom Brown", Role = "Manager", ContactNumber = "987654321", Email = "tom.brown@hotel.com" }
                       
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Employees seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Employees already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during employee seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }


        private void SeedServices(HotelDbContext dbContext)
        {
            try
            {
                if (!dbContext.Services.Any())
                {
                    Console.WriteLine("Seeding Services...");
                    dbContext.Services.AddRange(
                        new Service { Name = "Spa", Price = 1500, Description = "Relaxing spa treatment" },
                        new Service { Name = "Breakfast", Price = 300, Description = "Delicious breakfast buffet" },
                        new Service { Name = "Gym", Price = 100, Description = "Access to gym facilities" },
                        new Service { Name = "Dinner", Price = 500, Description = "Three-course dinner" }
                    );
                    dbContext.SaveChanges();
                    Console.WriteLine("Services seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Services already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during service seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
        private void SeedFeedback(HotelDbContext dbContext)
        {
            try
            {
                Console.WriteLine("Seeding Feedback...");

                // Check if Guest 'John Doe' exists
                var guest = dbContext.Guests.FirstOrDefault(g => g.Name == "John Doe");

                if (guest == null)
                {
                    Console.WriteLine("Guest 'John Doe' not found. Skipping feedback seeding.");
                    return;
                }

                // Seed feedback only if none exists
                if (!dbContext.Feedbacks.Any())
                {
                    dbContext.Feedbacks.Add(new Feedback
                    {
                        GuestId = guest.GuestId,
                        Comments = "Amazing stay!",
                        Rating = 5,
                        BookingId = null,   // Explicitly set BookingId as null
                        EmployeeId = null   // Explicitly set EmployeeId as null
                    });

                    dbContext.SaveChanges();
                    Console.WriteLine("Feedback seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Feedback already exists. Skipping.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during feedback seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        private void SeedPaymentInvoices(HotelDbContext dbContext)
        {
            try
            {
                Console.WriteLine("Seeding Payment Invoices...");

                // Ensure there is at least one Booking to associate the PaymentInvoice
                var booking1 = dbContext.Bookings.FirstOrDefault();
                if (booking1 == null)
                {
                    Console.WriteLine("No bookings found. Cannot seed PaymentInvoices.");
                    return;
                }

                // Check if PaymentInvoices already exist
                if (!dbContext.PaymentInvoices.Any())
                {
                    dbContext.PaymentInvoices.Add(new PaymentInvoice
                    {
                        BookingId = booking1.BookingId,
                        Amount = 15000,
                        PaymentDate = DateTime.Today,
                        PaymentMethod = "Credit Card",
                        PaymentStatus = "Completed",
                        InvoiceDate = DateTime.Today,
                        InvoiceNotes = "Initial payment for Booking #" + booking1.BookingId
                    });
                    dbContext.SaveChanges();
                    Console.WriteLine("Payment invoices seeded successfully.");
                }
                else
                {
                    Console.WriteLine("Payment invoices already exist. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during payment invoice seeding: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}
