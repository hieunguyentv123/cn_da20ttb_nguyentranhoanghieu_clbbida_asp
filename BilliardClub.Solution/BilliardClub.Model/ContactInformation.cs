using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("ContactInformations")]
    public class ContactInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactInfoId { get; set; }
        [StringLength(10), Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string Phone { get; set; }
        [Required, StringLength(4000)]
        public string Address { get; set; }
        public bool IsDisplay { get; set; }
    }
}
