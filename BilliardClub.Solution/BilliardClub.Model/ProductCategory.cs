using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("ProductCategories")]
    public class ProductCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required, StringLength(256)]
        public string CategoryName { get; set; }
        public string DataFilter { get; set; }
        [Required]
        public bool IsDisplay { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
