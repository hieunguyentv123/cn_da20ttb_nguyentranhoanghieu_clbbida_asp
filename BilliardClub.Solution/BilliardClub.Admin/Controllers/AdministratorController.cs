using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class AdministratorController : Controller
    {
        private INotyfService _notyf;
        private IUserRepository _userRepository;

        public AdministratorController(INotyfService notyf, IUserRepository userRepository)
        {
            _notyf = notyf;
            _userRepository = userRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _userRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(key))
            {
                list = list.Where(x => x.Username.ToLower().Contains(key.ToLower().Trim()));
            }
            return View(list.Where(x => x.IsAdministrator && x.Username != HttpContext.Session.GetString("adminUsername")).Reverse().ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Details(int id)
        {
            return View(_userRepository.GetSingleById(id));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Username,Fullname,Email,Address,Phone,DOB")] User user)
        {
            
            if (_userRepository.UsernameCheck(user.Username))
            {
                user.IsAdministrator = true;
                user.Password = _userRepository.MD5Hash(user.Username);
                try
                {
                    _userRepository.Add(user);
                    _userRepository.SaveChanges();
                    _notyf.Success("Thêm mới thành công quản trị viên!", 5);
                    return RedirectToAction("Index", "Administrator");
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
            return View(user);
        }

        public IActionResult Edit(int id)
        {
            return View(_userRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            try
            {
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công quản trị viên!", 5);
                return RedirectToAction("Details", "Administrator", new { id = user.UserId });
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return View(user);
        }
        public IActionResult Delete(int id)
        {
            var cat = _userRepository.GetSingleById(id);
            return View(cat);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var user = _userRepository.GetSingleById(id);
            if (confirm == user.Username)
            {
                try
                {
                    _userRepository.Delete(user);
                    _userRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận không đúng!", 5);
                return View(user);
            }
            return RedirectToAction("Index", "Administrator");
        }
        public IActionResult ChangeStatus(int id)
        {
            var user = _userRepository.GetSingleById(id);
            if (user.IsLocked)
            {
                user.IsLocked = false;
            }
            else
            {
                user.IsLocked = true;
            }
            try
            {
                _userRepository.Update(user);
                _userRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái tài khoản quản trị viên thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Administrator");
        }
    }
}
