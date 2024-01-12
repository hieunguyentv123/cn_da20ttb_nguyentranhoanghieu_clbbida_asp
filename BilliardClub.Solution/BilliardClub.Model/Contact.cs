using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        [Required, StringLength(256)]
        public string ContactDetail { get; set; }
        [Required, StringLength(256)]
        public string Fullname { get; set; }
        [Required, StringLength(256), EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [StringLength(10), Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string Phone { get; set; }
        [Required, StringLength(4000)]
        public string Message { get; set; }
        public DateTime SendAt { get; set; }
        public bool IsReplied { get; set; }
    }
}
