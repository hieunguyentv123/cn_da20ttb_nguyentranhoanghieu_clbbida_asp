using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilliardClub.Model
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required, StringLength(500)]
        public string ProductName { get; set; }
        [Required, StringLength(5000)]
        public string ProductDetail { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int DiscountPercent { get; set; }
        [Required]
        public int Stock {  get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDisplay { get; set; }
        [StringLength(5000)]
        public string ProductImage { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductReview>? ProductReviews { get; set; }
    }
}
