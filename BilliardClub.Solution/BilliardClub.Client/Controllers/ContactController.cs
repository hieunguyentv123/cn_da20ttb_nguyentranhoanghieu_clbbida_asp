using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;

namespace BilliardClub.Client.Controllers
{
    public class ContactController : Controller
    {
        private ILayoutDataReceiver _layoutDataReceiver;
        private IContactRepository _contactRepository;
        private INotyfService _notyf;
        public ContactController(ILayoutDataReceiver layoutDataReceiver, IContactRepository contactRepository, INotyfService notyf)
        {
            _layoutDataReceiver = layoutDataReceiver;
            _contactRepository = contactRepository;
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
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SendContact(string email, string fullname, string phone, string message, string detail)
        {
            Contact contact = new Contact()
            {
                Fullname = fullname,
                Email = email,
                Phone = phone,
                Message = message,
                ContactDetail = detail,
                SendAt = DateTime.Now,
                IsReplied = false
            };
            try
            {
                _contactRepository.Add(contact);
                _contactRepository.SaveChanges();
                _notyf.Success("Gửi phản hồi thành công, chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất!", 4);
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 4);
            }
            return RedirectToAction("Index", "Contact");
        }
    }
}
