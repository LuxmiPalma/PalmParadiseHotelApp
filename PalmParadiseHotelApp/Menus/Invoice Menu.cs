using Spectre.Console;
using PalmParadiseHotelApp.Data;
using PalmParadiseHotelApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PalmParadiseHotelApp.Helpers;

namespace PalmParadiseHotelApp.Menus
{
    public class InvoiceMenu
    {
        public static void DisplayMenu(HotelDbContext dbContext)
        {
            var service = new PaymentInvoiceService(dbContext);

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Invoice Menu").Centered().Color(Color.Yellow));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices(
                            "Add Payment Invoice",
                            "View All Invoices",
                            "Search Invoice by ID",
                            "Update Payment Status",
                            "Delete Invoice",
                            "View Invoices by Date Range",
                            "Cancel Unpaid Bookings",
                            "Back to Main Menu"));

                if (choice == "Back to Main Menu") return;

                try
                {
                    switch (choice)
                    {
                        case "Add Payment Invoice":
                            AddInvoice(service);
                            break;
                        case "View All Invoices":
                            ViewAllInvoices(service);
                            break;
                        case "Search Invoice by ID":
                            SearchInvoice(service);
                            break;
                        case "Update Payment Status":
                            UpdateInvoiceStatus(service);
                            break;
                        case "Delete Invoice":
                            DeleteInvoice(service);
                            break;
                        case "View Invoices by Date Range":
                            ViewInvoicesByDateRange(service);
                            break;
                        case "Cancel Unpaid Bookings":
                            CancelUnpaidBookings(dbContext);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

                AnsiConsole.Markup("\n[green]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        // Helper Methods
        private static void HandleError(Exception ex)
        {
            AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            if (ex.InnerException != null)
            {
                AnsiConsole.Markup($"[red]Inner Exception: {ex.InnerException.Message}[/]\n");
            }
        }

        private static T PromptInput<T>(string message, T defaultValue = default)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)AnsiConsole.Ask<string>(message, (string)(object)defaultValue);
            }
            return AnsiConsole.Ask<T>(message);
        }

        private static Table BuildInvoiceTable(IEnumerable<PaymentInvoice> invoices, string[] columns)
        {
            var table = new Table();
            foreach (var column in columns)
            {
                table.AddColumn($"[cyan]{column}[/]");
            }

            foreach (var invoice in invoices)
            {
                table.AddRow(
                    invoice.PaymentInvoiceId.ToString(),
                    invoice.BookingId.ToString(),
                    $"{invoice.Amount:C}",
                    invoice.PaymentDate.ToShortDateString(),
                    invoice.PaymentMethod ?? "",
                    invoice.PaymentStatus ?? "",
                    invoice.InvoiceNotes ?? "");
            }

            return table;
        }

        // Methods for Invoice Menu
        private static void AddInvoice(PaymentInvoiceService service)
        {

            var bookingId = PromptInput<int>("Enter [yellow]Booking ID[/]:");

            using (var context = new HotelDbContext())
            {
                // Fetch booking and associated room price
                var booking = context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    AnsiConsole.Markup("[red]Error: Booking not found.[/]\n");
                    return;
                }

                var roomPrice = booking.Room.Price;

                // Prompt for amount and validate against room price
                var amount = PromptInput<decimal>($"Enter [yellow]Amount (Room Price: {roomPrice:C})[/]:");

                if (amount != roomPrice)
                {
                    if (!AnsiConsole.Confirm("[yellow]The entered amount does not match the room price. Do you want to proceed?[/]"))
                    {
                        AnsiConsole.Markup("[red]Operation canceled by the user.[/]\n");
                        return;
                    }
                }
                var method = PromptInput<string>("Enter [yellow]Payment Method[/] (e.g., Credit Card, Cash):");
                var status = PromptInput<string>("Enter [yellow]Payment Status[/] (e.g., Pending, Completed):");
                var notes = PromptInput<string>("Enter [yellow]Invoice Notes[/] (optional):", "");

                var invoice = new PaymentInvoice
                {
                    BookingId = bookingId,
                    Amount = amount,
                    PaymentDate = DateTime.Today,
                    PaymentMethod = method,
                    PaymentStatus = status,
                    InvoiceDate = DateTime.Today,
                    InvoiceNotes = notes
                };

                service.AddInvoice(invoice);
                AnsiConsole.Markup("[green]Invoice added successfully![/]\n");
            }
        }

        private static void ViewAllInvoices(PaymentInvoiceService service)
        {
            var invoices = service.GetAllInvoices();
            if (!invoices.Any())
            {
                AnsiConsole.Markup("[red]No invoices found.[/]\n");
                return;
            }

            var columns = new[] { "Invoice ID", "Booking ID", "Amount", "Payment Date", "Method", "Status", "Notes" };
            PaginateTable(BuildInvoiceTable(invoices, columns));
        }

        private static void ViewInvoicesByDateRange(PaymentInvoiceService service)
        {
            var startDate = PromptInput<DateTime>("Enter [yellow]Start Date (yyyy-MM-dd)[/]:");
            var endDate = PromptInput<DateTime>("Enter [yellow]End Date (yyyy-MM-dd)[/]:");

            var invoices = service.GetAllInvoices()
                .Where(i => i.PaymentDate >= startDate && i.PaymentDate <= endDate)
                .ToList();

            if (!invoices.Any())
            {
                AnsiConsole.Markup("[red]No invoices found within the specified date range.[/]\n");
                return;
            }

            var columns = new[] { "Invoice ID", "Booking ID", "Amount", "Payment Date", "Method", "Status", "Notes" };
            AnsiConsole.Write(BuildInvoiceTable(invoices, columns));
        }

        private static void CancelUnpaidBookings(HotelDbContext context)
        {
            var tenDaysAgo = DateTime.Today.AddDays(-10);
            var unpaidInvoices = context.PaymentInvoices
                .Where(pi => pi.PaymentStatus == "Pending" && pi.InvoiceDate <= tenDaysAgo)
                .Include(pi => pi.Booking)
                .ToList();

            if (!unpaidInvoices.Any())
            {
                AnsiConsole.Markup("[green]No unpaid bookings found for cancellation.[/]\n");
                return;
            }

            foreach (var invoice in unpaidInvoices)
            {
                var booking = invoice.Booking;
                context.Bookings.Remove(booking);
                AnsiConsole.Markup($"[red]Cancelled Booking ID {booking.BookingId} due to non-payment.[/]\n");
            }

            context.SaveChanges();
            AnsiConsole.Markup("[green]Unpaid bookings cancelled successfully![/]\n");
        }

        private static void PaginateTable(Table table)
        {
            const int pageSize = 10;
            var rows = table.Rows.ToList();

            for (int i = 0; i < rows.Count; i += pageSize)
            {
                var pageTable = new Table();
                foreach (var column in table.Columns)
                {
                    pageTable.AddColumn(new TableColumn(column.Header));
                }

                foreach (var row in rows.Skip(i).Take(pageSize))
                {
                    pageTable.AddRow(row);
                }

                AnsiConsole.Clear();
                AnsiConsole.Write(pageTable);

                if (i + pageSize < rows.Count)
                {
                    AnsiConsole.Markup("\n[green]Press any key to view the next page...[/]");
                    Console.ReadKey();
                }
            }

        }
        private static void SearchInvoice(PaymentInvoiceService service)
        {
            var invoiceId = PromptInput<int>("Enter [yellow]Invoice ID[/]:");
            var invoice = service.GetInvoiceById(invoiceId);

            if (invoice != null)
            {
                AnsiConsole.Markup($"[cyan]Invoice ID:[/] [yellow]{invoice.PaymentInvoiceId}[/]\n");
                AnsiConsole.Markup($"[cyan]Booking ID:[/] [yellow]{invoice.BookingId}[/]\n");
                AnsiConsole.Markup($"[cyan]Amount:[/] [yellow]{invoice.Amount:C}[/]\n");
                AnsiConsole.Markup($"[cyan]Payment Method:[/] [yellow]{invoice.PaymentMethod}[/]\n");
                AnsiConsole.Markup($"[cyan]Payment Status:[/] [yellow]{invoice.PaymentStatus}[/]\n");
                AnsiConsole.Markup($"[cyan]Invoice Date:[/] [yellow]{invoice.InvoiceDate:yyyy-MM-dd}[/]\n");
                AnsiConsole.Markup($"[cyan]Notes:[/] [yellow]{invoice.InvoiceNotes}[/]\n");
            }
            else
            {
                AnsiConsole.Markup("[red]Invoice not found.[/]\n");
            }
        }

        private static void UpdateInvoiceStatus(PaymentInvoiceService service)
        {
            var invoiceId = PromptInput<int>("Enter [yellow]Invoice ID[/]:");
            var status = PromptInput<string>("Enter [yellow]New Status[/] (e.g., Completed, Pending):");

            service.UpdateStatus(invoiceId, status);
            AnsiConsole.Markup("[green]Payment status updated successfully![/]\n");
        }

        private static void DeleteInvoice(PaymentInvoiceService service)
        {
            var invoiceId = PromptInput<int>("Enter [yellow]Invoice ID[/] to delete:");
            service.DeleteInvoice(invoiceId);
            AnsiConsole.Markup("[green]Invoice deleted successfully![/]\n");
        }

    }
}
