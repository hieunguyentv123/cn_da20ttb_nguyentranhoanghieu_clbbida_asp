using BilliardClub.Model;

namespace BilliardClub.Admin.Models
{
    public class TopCustomerViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public int TotalPrice { get; set; }
        public int TotalSell { get; set; }
    }
}
