using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilliardClub.Model
{
    [Table("ArticleCategories")]
    public class ArticleCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleCatId { get; set; }
        [Required, StringLength(256)]
        public string ArticleCatName { get; set; }
        [Required]
        public bool IsDisplay { get; set; }
        public virtual ICollection<Article>? Articles { get; set; }
    }
}
