using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using BilliardClub.Data.Repositories;
using System.Security.Principal;

namespace BilliardClub.Admin.Controllers
{
    public class AuthController : Controller
    {
        private IUserRepository _userRepository;
        private INotyfService _notyf;
        public AuthController(IUserRepository userRepository, INotyfService notyf)
        {
            _userRepository = userRepository;
            _notyf = notyf;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            var account = _userRepository.AdministratorLogin(login.Username, login.Password);
            if (account != null)
            {
                switch (account.IsLocked)
                {
                    case true:
                        _notyf.Error($"Tài khoản của bạn hiện đang bị khoá", 5);
                        return View(login);
                    case false:
                        _notyf.Success($"Chào mừng quay trở lại, {account.Fullname}", 5);
                        SetSession(account.UserId, account.Username, account.Fullname, "System Administrator", account.AvatarPath);
                        return RedirectToAction("Index", "Home");
                }
                
            }
            _notyf.Error("Tài khoản hoặc mật khẩu không khớp với xác thực hệ thống!", 5);
            return View(login);
        }
        private void SetSession(int userId, string username, string fullname, string roleName, string? avatar)
        {
            HttpContext.Session.SetInt32("_id", userId);
            HttpContext.Session.SetString("adminToken", _userRepository.MD5Hash(username));
            HttpContext.Session.SetString("adminUsername", username);
            HttpContext.Session.SetString("adminName", fullname);
            HttpContext.Session.SetString("adminRoleName", roleName);
            if (!String.IsNullOrEmpty(avatar))
            {
                HttpContext.Session.SetString("adminAvatar", avatar);
            }
            else
            {
                HttpContext.Session.SetString("adminAvatar", "");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
