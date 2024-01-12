using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Admin.Utils;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ContactController : Controller
    {
        private INotyfService _notyf;
        private IContactRepository _contactRepository;

        public ContactController(INotyfService notyf, IContactRepository contactRepository)
        {
            _notyf = notyf;
            _contactRepository = contactRepository;
        }

        public IActionResult Index(int? page)
        {
            var list = _contactRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.OrderByDescending(x => x.SendAt).ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Details(int id)
        {
            var contact = _contactRepository.GetSingleById(id);
            return View(contact);
        }
        public IActionResult ChangeStatus(int id)
        {
            var contact = _contactRepository.GetSingleById(id);
            contact.IsReplied = true;
            try
            {
                _contactRepository.Update(contact);
                _contactRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Contact");
        }
        public IActionResult Delete(int id)
        {
            var contact = _contactRepository.GetSingleById(id);
            return View(contact);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var contact = _contactRepository.GetSingleById(id);
            if (confirm == "OK")
            {
                try
                {
                    _contactRepository.Delete(contact);
                    _contactRepository.SaveChanges();
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
                return View(contact);
            }
            return RedirectToAction("Index", "contact");
        }
    }
}
