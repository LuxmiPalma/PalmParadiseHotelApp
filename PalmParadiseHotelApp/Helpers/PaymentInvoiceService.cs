using PalmParadiseHotelApp.Data;
using PalmParadiseHotelApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmParadiseHotelApp.Helpers
{
    public class PaymentInvoiceService
    {
        private readonly HotelDbContext _context;

        public PaymentInvoiceService(HotelDbContext context)
        {
            _context = context;
        }

        public void AddInvoice(PaymentInvoice invoice)
        {
            _context.PaymentInvoices.Add(invoice);
            _context.SaveChanges();
            Console.WriteLine("Invoice added successfully!");
        }

        public void ViewAllInvoices()
        {
            var invoices = _context.PaymentInvoices.ToList();
            if (invoices.Any())
            {
                foreach (var invoice in invoices)
                {
                    Console.WriteLine($"ID: {invoice.PaymentInvoiceId}, Amount: {invoice.Amount}, Status: {invoice.PaymentStatus}");
                }
            }
            else
            {
                Console.WriteLine("No invoices found.");
            }
        }

        public PaymentInvoice GetInvoiceById(int id)
        {
            return _context.PaymentInvoices.FirstOrDefault(i => i.PaymentInvoiceId == id);
        }

        public void UpdateStatus(int id, string newStatus)
        {
            var invoice = GetInvoiceById(id);
            if (invoice != null)
            {
                invoice.PaymentStatus = newStatus;
                _context.SaveChanges();
                Console.WriteLine("Invoice status updated successfully!");
            }
            else
            {
                Console.WriteLine("Invoice not found.");
            }
        }

        public void DeleteInvoice(int id)
        {
            var invoice = GetInvoiceById(id);
            if (invoice != null)
            {
                _context.PaymentInvoices.Remove(invoice);
                _context.SaveChanges();
                Console.WriteLine("Invoice deleted successfully!");
            }
            else
            {
                Console.WriteLine("Invoice not found.");
            }
        }
        // New method to get all invoices
        public List<PaymentInvoice> GetAllInvoices()
        {
            return _context.PaymentInvoices.ToList();
        }
    }
}


