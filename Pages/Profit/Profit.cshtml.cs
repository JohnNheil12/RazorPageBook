using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace RazorPageBooks.Pages.Profit
{
    public class BestSeller
    {
        public int Rank { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyStat
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class IndexModel : PageModel
    {
        public List<BestSeller> BestSellers { get; set; } = new();
        public List<MonthlyStat> MonthlyStats { get; set; } = new();

        // Summary properties
        public decimal TotalRevenue => MonthlyStats.Sum(m => m.Revenue);
        public int TotalOrders => MonthlyStats.Sum(m => m.Orders);
        public decimal AvgOrderValue => TotalOrders > 0 ? TotalRevenue / TotalOrders : 0;
        public int TotalBooksSold => BestSellers.Sum(b => b.UnitsSold);

        public decimal ThisMonthRevenue => 9840m;
        public int ThisMonthOrders => 34;
        public decimal ThisWeekRevenue => 2620m;
        public int ThisWeekOrders => 9;

        // For chart — returns 0–100 as a percentage of max
        public int BarHeight(decimal revenue)
        {
            var max = MonthlyStats.Max(m => m.Revenue);
            return max == 0 ? 0 : (int)(revenue / max * 92);
        }

        public void OnGet()
        {
            // Monthly revenue — replace with real DbContext aggregation
            MonthlyStats = new List<MonthlyStat>
            {
                new MonthlyStat { Month="Jan", Revenue=4500m,  Orders=16 },
                new MonthlyStat { Month="Feb", Revenue=5200m,  Orders=19 },
                new MonthlyStat { Month="Mar", Revenue=3900m,  Orders=14 },
                new MonthlyStat { Month="Apr", Revenue=6100m,  Orders=22 },
                new MonthlyStat { Month="May", Revenue=7200m,  Orders=26 },
                new MonthlyStat { Month="Jun", Revenue=5900m,  Orders=21 },
                new MonthlyStat { Month="Jul", Revenue=6700m,  Orders=24 },
                new MonthlyStat { Month="Aug", Revenue=8800m,  Orders=31 },
                new MonthlyStat { Month="Sep", Revenue=7600m,  Orders=27 },
                new MonthlyStat { Month="Oct", Revenue=7000m,  Orders=25 },
                new MonthlyStat { Month="Nov", Revenue=5100m,  Orders=18 },
                new MonthlyStat { Month="Dec", Revenue=3800m,  Orders=13 },
            };

            BestSellers = new List<BestSeller>
            {
                new BestSeller { Rank=1, Title="Atomic Habits",        Author="James Clear",   UnitsSold=84, Revenue=35280m },
                new BestSeller { Rank=2, Title="The Midnight Library",  Author="Matt Haig",     UnitsSold=67, Revenue=25460m },
                new BestSeller { Rank=3, Title="Project Hail Mary",    Author="Andy Weir",     UnitsSold=52, Revenue=26520m },
                new BestSeller { Rank=4, Title="Dune",                 Author="Frank Herbert", UnitsSold=48, Revenue=21600m },
                new BestSeller { Rank=5, Title="The Alchemist",        Author="Paulo Coelho",  UnitsSold=41, Revenue=13940m },
            };
        }
    }
}