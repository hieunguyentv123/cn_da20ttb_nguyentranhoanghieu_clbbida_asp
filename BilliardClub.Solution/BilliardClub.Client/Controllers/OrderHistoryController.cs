using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;
using X.PagedList;

namespace BilliardClub.Client.Controllers
{
    public class OrderHistoryController : Controller
    {
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IProductRepository _productRepository;
        private IUserRepository _userRepository;
        public const string CARTKEY = "cart";
        public OrderHistoryController(IUserRepository userRepository, INotyfService notyf, ILayoutDataReceiver layoutDataReceiver, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IProductRepository productRepository)
        {
            _notyf = notyf;
            _layoutDataReceiver = layoutDataReceiver;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
        }
        public IActionResult Index(int? page)
        {
            GetData();
            if (HttpContext.Session.GetInt32("_id") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var list = _orderRepository.GetAll().Where(x => x.OrderBy == HttpContext.Session.GetInt32("_id"));
            if (page == null) page = 1;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(list.OrderByDescending(x => x.OrderAt).ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Details(string id)
        {
            GetData();
            var orderDetails = _orderDetailRepository.GetMulti(x => x.OrderId == new Guid(id));
            return View(orderDetails);
        }
    }
}
