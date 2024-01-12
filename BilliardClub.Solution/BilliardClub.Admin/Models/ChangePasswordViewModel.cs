using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Admin.Models
{
    public class ChangePasswordViewModel
    {
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu cũ là thông tin bắt buộc *")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới là thông tin bắt buộc *")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Xác nhận mật khẩu là thông tin bắt buộc *")]
        public string ConfirmPassword { get; set; }
    }
}
