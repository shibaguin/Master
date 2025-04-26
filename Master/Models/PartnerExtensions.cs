using System.ComponentModel.DataAnnotations.Schema;

namespace Master.Models
{
    public partial class Partner
    {
        [NotMapped]
        public int TotalSales { get; set; }

        [NotMapped]
        public decimal Discount { get; set; }

        public void ComputeDiscount(int totalSales)
        {
            TotalSales = totalSales;
            if (totalSales >= 300000)
                Discount = 0.15m;
            else if (totalSales >= 50000)
                Discount = 0.10m;
            else if (totalSales >= 10000)
                Discount = 0.05m;
            else
                Discount = 0m;
        }
    }
} 