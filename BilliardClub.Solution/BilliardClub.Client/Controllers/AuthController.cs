using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.Models;
using Microsoft.AspNetCore.Mvc;
using BilliardClub.Data.Repositories;
using System.Diagnostics;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Security.Claims;
using NuGet.Protocol.Plugins;

namespace BilliardClub.Client.Controllers
{
    public class AuthController : Controller
    {
        private IUserRepository _userRepository;
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        public AuthController(IUserRepository userRepository, INotyfService notyf, ILayoutDataReceiver layoutDataReceiver)
        {
            _userRepository = userRepository;
            _notyf = notyf;
            _layoutDataReceiver = layoutDataReceiver;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
        }
        public IActionResult Login()
        {
            GetData();
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            var account = _userRepository.ClientLogin(login.Username, login.Password);
            if (account != null)
            {
                switch (account.IsLocked)
                {
                    case true:
                        _notyf.Error($"Tài khoản của bạn hiện đang bị khoá", 5);
                        return View(login);
                    case false:
                        _notyf.Success($"Chào mừng quay trở lại, {account.Fullname}", 5);
                        SetSession(account.UserId, account.Username, account.Fullname, account.AvatarPath);
                        return RedirectToAction("Index", "Home");
                }

            }
            _notyf.Error("Tài khoản hoặc mật khẩu không khớp với xác thực hệ thống!", 5);
            GetData();
            return View(login);
        }
        public IActionResult Register()
        {
            GetData();
            return View();
        }
        [HttpPost]
        public IActionResult Register([Bind("Username,Password,Fullname,Email,Address,Phone,DOB")] User user)
        {
            if (_userRepository.UsernameCheck(user.Username))
            {
                user.IsAdministrator = false;
                user.IsLocked = false;
                string hash = _userRepository.MD5Hash(user.Password);
                user.Password = hash;
                try
                {
                    _userRepository.Add(user);
                    _userRepository.SaveChanges();
                    _notyf.Success($"Đăng ký thành công tài khoản thành viên, hãy tiến hành đăng nhập", 5);
                    return RedirectToAction("Login", "Auth");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error($"Tài khoản {user.Username} đã được sử dụng, hãy thử tài khoản khác!", 5);
            }
            GetData();
            return View(user);
        }
        [Authorize]
        [AllowAnonymous]
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("SignupGoogle", "Auth")
            }) ;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SignupGoogle()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            User user = new User()
            {
                Email = result.Principal.FindFirst(ClaimTypes.Email).Value,
                Username = result.Principal.FindFirst(ClaimTypes.Email).Value,
                Fullname = result.Principal.FindFirst(ClaimTypes.Name).Value,

            };
            if (result.Succeeded)
            {
                if (!_userRepository.UsernameCheck(user.Username))
                {
                    var account = _userRepository.GetSingleByCondition(x => x.Username == user.Username);
                    switch (account.IsLocked)
                    {
                        case true:
                            _notyf.Error($"Tài khoản của bạn hiện đang bị khoá", 5);
                            return RedirectToAction("Login","Auth");
                        case false:
                            _notyf.Success($"Chào mừng quay trở lại, {account.Fullname}", 5);
                            SetSession(account.UserId, account.Username, account.Fullname, account.AvatarPath);
                            return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    GetData();
                    return View(user);
                }
            }
            else
            {
                return AccessDenied();
            }
        }

        public IActionResult AccessDenied()
        {
            GetData();
            return View();
        }

        private void SetSession(int userId, string username, string fullname, string? avatar)
        {
            HttpContext.Session.SetInt32("_id", userId);
            if (_userRepository.GetSingleById(userId).IsAdministrator)
            {
                HttpContext.Session.SetString("_role", "Admin");
            }
            else
            {
                HttpContext.Session.SetString("_role", "User");
            }
            HttpContext.Session.SetString("clientToken", _userRepository.MD5Hash(username));
            HttpContext.Session.SetString("clientUsername", username);
            HttpContext.Session.SetString("clientName", fullname);
            if (!String.IsNullOrEmpty(avatar))
            {
                HttpContext.Session.SetString("clientAvatar", avatar);
            }
            else
            {
                HttpContext.Session.SetString("clientAvatar", "");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("_id");
            HttpContext.Session.Remove("_role");
            HttpContext.Session.Remove("clientToken");
            HttpContext.Session.Remove("clientUsername");
            HttpContext.Session.Remove("clientName");
            HttpContext.Session.Remove("clientAvatar");
            return RedirectToAction("Login", "Auth");
        }
    }
}
