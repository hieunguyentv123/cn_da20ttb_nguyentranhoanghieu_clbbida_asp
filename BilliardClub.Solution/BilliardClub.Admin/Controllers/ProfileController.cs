using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Admin.Models;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using BilliardClub.Data.Repositories;

namespace BilliardClub.Admin.Controllers
{
    public class ProfileController : Controller
    {
        private INotyfService _notyf;
        private IUserRepository _userRepository;

        public ProfileController(INotyfService notyf, IUserRepository userRepository)
        {
            _notyf = notyf;
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("adminUsername");
            User profile = _userRepository.GetSingleByCondition(x => x.Username == username);
            return View(profile);
        }
        public IActionResult Edit()
        {
            string username = HttpContext.Session.GetString("adminUsername");
            User profile = _userRepository.GetSingleByCondition(x => x.Username == username);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            try
            {
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                HttpContext.Session.SetString("adminName", user.Fullname);
                _notyf.Success("Cập nhật thông tin cá nhân thành công");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Profile");
        }
        public IActionResult ChangePassword()
        {
            string sessionName = HttpContext.Session.GetString("adminUsername");
            ViewBag.Username = sessionName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword([Bind("Username,OldPassword,ConfirmPassword,NewPassword")] ChangePasswordViewModel changes)
        {
            if (changes.NewPassword != changes.ConfirmPassword)
            {
                _notyf.Error("Nhập lại mật khẩu mới không trùng khớp", 4);
            }
            else
            {
                if (_userRepository.ChangePassword(changes.Username, changes.OldPassword, changes.NewPassword))
                {
                    _notyf.Success($"Đổi mật khẩu thành công, hãy tiến hành đăng nhập lại bằng mật khẩu mới", 3);
                    return RedirectToAction("Logout", "Auth");
                }
                else
                {
                    _notyf.Error("Mật khẩu cũ không chính xác", 4);
                }
            }
            string sessionName = HttpContext.Session.GetString("adminUsername");
            ViewBag.Username = sessionName;
            return View(changes);
        }
        public IActionResult ChangeAvatar()
        {
            string username = HttpContext.Session.GetString("adminUsername");
            User user = _userRepository.GetSingleByCondition(x => x.Username == username);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAvatar(User user, IFormFile avatar)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\avatar");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (avatar != null && avatar.Length > 0)
            {
                if (!String.IsNullOrEmpty(user.AvatarPath))
                {
                    var oldFilePath = Path.Combine(path, user.AvatarPath);
                    if (Directory.Exists(oldFilePath))
                    {
                        Directory.Delete(oldFilePath);
                    }
                }
                string fileName = Path.GetFileName(avatar.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                user.AvatarPath = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    avatar.CopyTo(stream);
                    HttpContext.Session.SetString("adminAvatar", fileName);
                    _notyf.Success($"Cập nhật ảnh đại diện thành công!", 5);
                }
                try
                {
                    _userRepository.Update(user);
                    _userRepository.SaveChanges();
                    _notyf.Success("Cập nhật thông tin cá nhân thành công");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Information("Hãy chọn ảnh để thay đổi ảnh đại diện", 5);
            }

            return View(user);
        }
    }
}
