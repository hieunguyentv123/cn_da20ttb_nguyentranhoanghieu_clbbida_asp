using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ContactInformationController : Controller
    {
        private INotyfService _notyf;
        private IContactInformationRepository _contactInformationRepository;

        public ContactInformationController(INotyfService notyf, IContactInformationRepository _contactInformationRepository)
        {
            _notyf = notyf;
            this._contactInformationRepository = _contactInformationRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _contactInformationRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.Reverse().ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Phone,Address")] ContactInformation contactInformation)
        {
            contactInformation.IsDisplay = true;
            if (ModelState.IsValid)
            {
                try
                {
                    _contactInformationRepository.Add(contactInformation);
                    _contactInformationRepository.SaveChanges();
                    _notyf.Success("Thêm mới thành công thông tin liên hệ!", 5);
                    return RedirectToAction("Index", "ContactInformation");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            return View(contactInformation);
        }

        public IActionResult Edit(int id)
        {
            return View(_contactInformationRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ContactInformation contactInformation)
        {
            try
            {
                _contactInformationRepository.Update(contactInformation);
                _contactInformationRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công thông tin liên hệ!", 5);
                return RedirectToAction("Index", "ContactInformation");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return View(contactInformation);
        }
        public IActionResult Delete(int id)
        {
            var contactInformation = _contactInformationRepository.GetSingleById(id);
            return View(contactInformation);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var contactInformation = _contactInformationRepository.GetSingleById(id);
            if (confirm == "OK")
            {
                try
                {
                    _contactInformationRepository.Delete(contactInformation);
                    _contactInformationRepository.SaveChanges();
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
                return View(contactInformation);
            }
            return RedirectToAction("Index", "ContactInformation");
        }
        public IActionResult ChangeStatus(int id)
        {
            var contactInformation = _contactInformationRepository.GetSingleById(id);
            if (contactInformation.IsDisplay)
            {
                contactInformation.IsDisplay = false;
            }
            else
            {
                contactInformation.IsDisplay = true;
            }
            try
            {
                _contactInformationRepository.Update(contactInformation);
                _contactInformationRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "ContactInformation");
        }
    }
}
