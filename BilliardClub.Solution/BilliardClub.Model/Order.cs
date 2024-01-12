using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Model
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        [Required, StringLength(256)]
        public string Fullname { get; set; }
        [Required, StringLength(500)]
        public string Address { get; set; }
        [StringLength(10), Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string Phone { get; set; }
        [Required, StringLength(256), EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [StringLength(5000)]
        public string Note { get; set; }
        [Required]
        public DateTime OrderAt { get; set; }
        [Required]
        public int OrderBy { get; set; }
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        public int TotalPrice { get; set; }
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }
        [ForeignKey("OrderBy")]
        public virtual User User { get; set; }
    }
}
