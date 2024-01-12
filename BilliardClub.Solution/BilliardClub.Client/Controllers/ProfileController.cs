using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;

namespace BilliardClub.Client.Controllers
{
    public class ProfileController : Controller
    {
        private ILayoutDataReceiver _layoutDataReceiver;
        private IUserRepository _userRepository;
        private INotyfService _notyf;
        public ProfileController(ILayoutDataReceiver layoutDataReceiver, IUserRepository userRepository, INotyfService notyf)
        {
            _layoutDataReceiver = layoutDataReceiver;
            _userRepository = userRepository;
            _notyf = notyf;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
        }
        public IActionResult Index()
        {
            GetData();
            if (HttpContext.Session.GetInt32("_id") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(_userRepository.GetSingleById((int)HttpContext.Session.GetInt32("_id")));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(User user)
        {
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
            return RedirectToAction("Index", "Profile");
        }
        public IActionResult ChangePassword()
        {
            GetData();
            if (HttpContext.Session.GetInt32("_id") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            string sessionName = HttpContext.Session.GetString("clientUsername");
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
            string sessionName = HttpContext.Session.GetString("clientUsername");
            ViewBag.Username = sessionName;
            GetData();
            return View(changes);
        }
        public IActionResult ChangeAvatar()
        {
            GetData();
            if (HttpContext.Session.GetInt32("_id") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            int id = (int)HttpContext.Session.GetInt32("_id");
            User user = _userRepository.GetSingleById(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAvatar(IFormFile avatar)
        {
            var user = _userRepository.GetSingleById((int)HttpContext.Session.GetInt32("_id"));
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
                    HttpContext.Session.SetString("clientAvatar", fileName);
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
            GetData();
            return View(user);
        }
    }
}
