using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class OrderController : Controller
    {
        private INotyfService _notyf;
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IProductRepository _productRepository;
        private IUserRepository _userRepository;
        public OrderController(IUserRepository userRepository, INotyfService notyf, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, IProductRepository productRepository)
        {
            _notyf = notyf;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
        }
        public IActionResult Index(int? page, string? code, DateTime? date)
        {
            var list = _orderRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(code))
            {
                var order = _orderRepository.GetSingleByCondition(x => x.OrderId == new Guid(code));
                if (order != null)
                {
                    return RedirectToAction("Details", "Order", new {id = new Guid(code) });
                }
                else
                {
                    _notyf.Error("Đơn hàng không tồn tại!",5);
                }
            }
            if (!String.IsNullOrEmpty(date.ToString()))
            {
                list = list.Where(x => x.OrderAt.Date == date.Value.Date);
            }
            return View(list.OrderByDescending(x => x.OrderAt).ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Details(string id)
        {
            var orderDetails = _orderDetailRepository.GetMulti(x => x.OrderId == new Guid(id));
            return View(orderDetails);
        }
    }
}
