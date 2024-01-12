using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilliardClub.Model
{
    [Table("Articles")]
    public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }
        [Required, StringLength(500)]
        public string Title { get; set; }
        [Required, StringLength(4000)]
        public string Content { get; set; }
        [StringLength(4000)]
        public string VideoPath { get; set; }
        [StringLength(500)]
        public string ImagePath { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int CreateBy { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public bool IsDisplay { get; set; }
        [Required]
        public int ViewCount { get; set; }
        [ForeignKey("CreateBy")]
        public virtual User User { get; set; }
        [ForeignKey("CategoryId")]
        public virtual ArticleCategory ArticleCategory { get; set; }
    }
}
