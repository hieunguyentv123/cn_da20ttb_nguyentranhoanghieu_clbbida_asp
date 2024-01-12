using BilliardClub.Model;
using System.Data.Entity;

namespace BilliardClub.Data
{
    public class BilliardClubDbContext : DbContext
    {
        public BilliardClubDbContext() : base("Data Source=ADMIN;Initial Catalog=BilliardClubDb;Integrated Security=True;MultipleActiveResultSets=true")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}