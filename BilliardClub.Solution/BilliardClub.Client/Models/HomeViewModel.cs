using BilliardClub.Model;

namespace BilliardClub.Client.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ContactInformation> ContactInformations { get; set; }
        public IEnumerable<Slide> Slides { get; set; }
    }
}
