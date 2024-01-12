using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilliardClub.Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required, StringLength(256)]
        public string Username { get; set; }
        [Required, StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(256)]
        public string Fullname { get; set; }
        [Required, StringLength(256), EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [Required, StringLength(4000)]
        public string Address { get; set; }
        [StringLength(10), Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        [StringLength(4000)]
        public string AvatarPath { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsLocked { get; set; }
    }
}