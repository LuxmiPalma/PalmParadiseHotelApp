﻿using Microsoft.EntityFrameworkCore;
using PalmParadiseHotelApp.Data;
using PalmParadiseHotelApp.Helpers;
using PalmParadiseHotelApp.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmParadiseHotelApp.Menus
{
    public class MainMenu
    {
        private static readonly string[] TitleAscii = new string[]
      {
            "  ____       _    __  __       _      ____            _       ",
            " |  _ \\ __ _| |_ |  \\/  | __ _| |__  |  _ \\ __ _  ___| | ____ ",
            " | |_) / _` | __|| |\\/| |/ _` | '_ \\ | |_) / _` |/ __| |/ /\\ \\",
            " |  __/ (_| | |_ | |  | | (_| | | | ||  __/ (_| | (__|   <  > |",
            " |_|   \\__,_|\\__||_|  |_|\\__,_|_| |_| \\_|   \\__,_|\\___|_|\\_\\_/"
      };
        private readonly BookingHistoryHelper bookingHistoryHelper; // Declare BookingHistoryHelper

        public MainMenu()
        {
            // Initialize BookingHistoryHelper with DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
            optionsBuilder.UseSqlServer("YourConnectionStringHere"); // Use your actual connection string
            bookingHistoryHelper = new BookingHistoryHelper(optionsBuilder.Options);
        }

        public void DisplayMenu()
        {
            // Visa titel
            ConsoleHelper.PrintSpectreTitle("Palm Paradise Hotel");


            // Visa interaktiv meny
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices(new[] {
                            "1. Rooms",
                            "2. Guests",
                            "3. Bookings",
                            "4. Invoices",
                            "5. View Booking History",
                            "6. Employees",
                            "7. Feedback",
                            "8. Service Management",
                            "9. Exit"
                        }));

                switch (choice)
                {
                    case "1. Rooms":
                        new RoomMenu().DisplayRoomMenu();
                        break;
                    case "2. Guests":
                        new GuestMenu().DisplayGuestMenu();
                        break;
                    case "3. Bookings":
                        new PalmParadiseHotelApp.Menus.BookingMenu.BookingMenu().DisplayBookingMenu();
                        break;
                    case "4. Invoices":
                        using (var dbContext = new HotelDbContext())
                        {
                            InvoiceMenu.DisplayMenu(dbContext);
                        }
                        break;
                    case "5. View Booking History":
                        Console.Write("Enter Guest ID to view booking history: ");
                        if (int.TryParse(Console.ReadLine(), out int guestId))
                        {
                            bookingHistoryHelper.DisplayBookingHistory(guestId);
                        }
                        else
                        {
                            AnsiConsole.Markup("[red]Invalid Guest ID.[/]");
                        }
                        break;
                    case "6. Employees":
                        new EmployeeMenu().DisplayEmployeeMenu();
                        break;
                    case "7. Feedback":
                        new FeedbackMenu().DisplayFeedbackMenu();
                        break;
                    case "8. Service Management":
                        DisplayServiceManagementMenu();
                        break;
                    case "9. Exit":
                        AnsiConsole.Markup("[green]Exiting... Goodbye![/]");
                        return;
                }
            }
        }
        // Service Management Menu
        private void DisplayServiceManagementMenu()
        {
            var serviceChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Service Management Options:[/]")
                    .AddChoices(new[] {
                        "1. Manage Services (CRUD)",
                        "2. Manage Booking Services",
                        "3. Back to Main Menu"
                    }));

            switch (serviceChoice)
            {
                case "1. Manage Services (CRUD)":
                    new ServiceMenu().DisplayServiceMenu();
                    break;
                case "2. Manage Booking Services":
                    new BookingServiceMenu().DisplayBookingServiceMenu();
                    break;
                case "3. Back to Main Menu":
                    return;
            }

        }
    }
}


 

