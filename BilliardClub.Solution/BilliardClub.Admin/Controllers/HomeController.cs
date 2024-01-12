using BilliardClub.Admin.Models;
using BilliardClub.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Licenses;
using System.Diagnostics;

namespace BilliardClub.Admin.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository _productRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IOrderRepository _orderRepository;

        public HomeController(IProductRepository productRepository, IOrderDetailRepository orderDetailRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            GetData();
            return View();
        }

        private void GetData()
        {
            //best seller
            var orderDetails = _orderDetailRepository.GetAll();
            var bestSellerProducts = orderDetails.Where(x => x.Order.IsPaid).GroupBy(x => new { x.ProductId, x.Product.ProductName }, (Key, group) => new BestSellerProductViewModel()
            {
                ProductId = Key.ProductId,
                ProductName = Key.ProductName,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
                Product = _productRepository.GetSingleById(Key.ProductId),
            }).OrderByDescending(x => x.TotalSell).Take(10).ToList();
            ViewBag.BestSellerProducts = bestSellerProducts;

            //top khách hàng
            var topCustomers = orderDetails.Where(x => x.Order.IsPaid).GroupBy(x => new { x.Order.OrderBy, x.Order.User.Fullname }, (Key, group) => new TopCustomerViewModel()
            {
                UserId = Key.OrderBy,
                Fullname = Key.Fullname,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
            }).OrderByDescending(x => x.TotalPrice).Take(10).ToList();
            ViewBag.TopCustomers = topCustomers;

            //đơn hàng mỗi ngày
            var orders = _orderRepository.GetAll().OrderByDescending(x => x.OrderAt);
            ViewBag.RecentOrders = orders.Take(10).ToList();
            if (orders.Where(x => x.OrderAt.Date == DateTime.Today).Count() == 0)
            {
                ViewBag.TodayOrders = orders.Where(x => x.OrderAt.Date == DateTime.Today).Take(10).ToList();
                ViewData["TodayOrdersStatus"] = "Hôm nay chưa có đơn hàng nào!";
            }
            else
            {
                ViewData["TodayOrdersStatus"] = "";
                ViewBag.TodayOrders = orders.Where(x => x.OrderAt.Date == DateTime.Today).Take(10).ToList();
            }

            //Top doanh thu ngày, tháng, năm
            var dailyTopProducts = orderDetails.Where(x => x.Order.IsPaid && x.Order.OrderAt.Day == DateTime.Today.Day).GroupBy(x => new { x.ProductId, x.Product.ProductName }, (Key, group) => new BestSellerProductViewModel()
            {
                ProductId = Key.ProductId,
                ProductName = Key.ProductName,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
                Product = _productRepository.GetSingleById(Key.ProductId),
            }).OrderByDescending(x => x.TotalSell).Take(10).ToList();

            var monthlyTopProducts = orderDetails.Where(x => x.Order.IsPaid && x.Order.OrderAt.Month == DateTime.Today.Month).GroupBy(x => new { x.ProductId, x.Product.ProductName }, (Key, group) => new BestSellerProductViewModel()
            {
                ProductId = Key.ProductId,
                ProductName = Key.ProductName,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
                Product = _productRepository.GetSingleById(Key.ProductId),
            }).OrderByDescending(x => x.TotalSell).Take(10).ToList();

            var yearlyTopProducts = orderDetails.Where(x => x.Order.IsPaid && x.Order.OrderAt.Year == DateTime.Today.Year).GroupBy(x => new { x.ProductId, x.Product.ProductName }, (Key, group) => new BestSellerProductViewModel()
            {
                ProductId = Key.ProductId,
                ProductName = Key.ProductName,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
                Product = _productRepository.GetSingleById(Key.ProductId),
            }).OrderByDescending(x => x.TotalSell).Take(10).ToList();

            ViewBag.DailyTopProduct = dailyTopProducts;
            ViewBag.MonthlyTopProduct = monthlyTopProducts;
            ViewBag.YearlyTopProduct = yearlyTopProducts;
            ViewBag.DailyRevenue = dailyTopProducts.Sum(x => x.TotalPrice);
            ViewBag.MonthlyRevenue = monthlyTopProducts.Sum(x => x.TotalPrice);
            ViewBag.YearlyRevenue = yearlyTopProducts.Sum(x => x.TotalPrice);
            ViewBag.TotalRevenue = bestSellerProducts.Sum(x => x.TotalPrice);
        }

        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View();
        }
    }
}