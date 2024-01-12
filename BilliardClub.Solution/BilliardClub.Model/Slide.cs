using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("Slides")]
    public class Slide
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlideId { get; set; }
        [Required, StringLength(4000)]
        public string SlidePath { get; set; }
        public bool IsDisplay { get; set; }
    }
}
