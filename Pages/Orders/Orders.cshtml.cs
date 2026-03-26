using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorPageBooks.Pages.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string BookTitle { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }      // Pending, Processing, Completed, Cancelled
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
    }

    public class IndexModel : PageModel
    {
        public List<Order> Orders { get; set; } = new();
        public string SearchString { get; set; }

        // Summary counts
        public int TotalOrders => Orders.Count;
        public int PendingOrders => Orders.Count(o => o.Status == "Pending");
        public int CompletedOrders => Orders.Count(o => o.Status == "Completed");
        public int CancelledOrders => Orders.Count(o => o.Status == "Cancelled");

        public void OnGet(string searchString)
        {
            SearchString = searchString;

            // Sample data — replace with your real DbContext query
            var allOrders = new List<Order>
            {
                new Order { Id=1, OrderNumber="ORD-0041", CustomerName="Maria Santos",   CustomerEmail="maria@email.com",   BookTitle="The Midnight Library", Quantity=2, Status="Pending",    OrderDate=new DateTime(2025,3,24), Amount=760m  },
                new Order { Id=2, OrderNumber="ORD-0040", CustomerName="Juan Dela Cruz", CustomerEmail="juan@email.com",    BookTitle="Atomic Habits",        Quantity=1, Status="Completed",  OrderDate=new DateTime(2025,3,22), Amount=420m  },
                new Order { Id=3, OrderNumber="ORD-0039", CustomerName="Ana Reyes",      CustomerEmail="ana@email.com",     BookTitle="Dune",                 Quantity=3, Status="Processing", OrderDate=new DateTime(2025,3,21), Amount=1350m },
                new Order { Id=4, OrderNumber="ORD-0038", CustomerName="Carlos Tan",     CustomerEmail="carlos@email.com",  BookTitle="Project Hail Mary",    Quantity=1, Status="Cancelled",  OrderDate=new DateTime(2025,3,20), Amount=510m  },
                new Order { Id=5, OrderNumber="ORD-0037", CustomerName="Lea Gomez",      CustomerEmail="lea@email.com",     BookTitle="The Alchemist",        Quantity=2, Status="Pending",    OrderDate=new DateTime(2025,3,19), Amount=680m  },
                new Order { Id=6, OrderNumber="ORD-0036", CustomerName="Ben Torres",     CustomerEmail="ben@email.com",     BookTitle="Atomic Habits",        Quantity=1, Status="Completed",  OrderDate=new DateTime(2025,3,18), Amount=420m  },
                new Order { Id=7, OrderNumber="ORD-0035", CustomerName="Rosa Lim",       CustomerEmail="rosa@email.com",    BookTitle="Dune",                 Quantity=2, Status="Completed",  OrderDate=new DateTime(2025,3,17), Amount=900m  },
                new Order { Id=8, OrderNumber="ORD-0034", CustomerName="Mark Sy",        CustomerEmail="mark@email.com",    BookTitle="The Midnight Library", Quantity=1, Status="Pending",    OrderDate=new DateTime(2025,3,16), Amount=380m  },
            };

            if (!string.IsNullOrEmpty(searchString))
            {
                var q = searchString.ToLower();
                allOrders = allOrders
                    .Where(o => o.OrderNumber.ToLower().Contains(q)
                             || o.CustomerName.ToLower().Contains(q)
                             || o.BookTitle.ToLower().Contains(q)
                             || o.Status.ToLower().Contains(q))
                    .ToList();
            }

            Orders = allOrders;
        }
    }
}