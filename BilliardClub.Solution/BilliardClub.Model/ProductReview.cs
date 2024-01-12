using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("ProductReviews")]
    public class ProductReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }
        [Required, StringLength(4000)]
        public string Content { get; set; }
        [Required]
        public DateTime ReviewAt { get; set; }
        [Required]
        public int ReviewBy { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ReviewBy")]
        public virtual User User { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
