using BilliardClub.Model;

namespace BilliardClub.Admin.Models
{
    public class BestSellerProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalPrice { get; set; }
        public int TotalSell { get; set; }
        public Product Product { get; set; }
    }
}
