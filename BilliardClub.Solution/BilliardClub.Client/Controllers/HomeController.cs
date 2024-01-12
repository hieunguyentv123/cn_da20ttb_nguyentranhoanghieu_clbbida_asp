using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Licenses;
using System.Diagnostics;

namespace BilliardClub.Client.Controllers
{
    public class HomeController : Controller
    {
        private ILayoutDataReceiver _layoutDataReceiver;
        private IProductRepository _productRepository;
        IOrderDetailRepository _orderDetailRepository;

        public HomeController(ILayoutDataReceiver layoutDataReceiver, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository)
        {
            _layoutDataReceiver = layoutDataReceiver;
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
            ViewBag.Slides = model.Slides;
            var orderDetails = _orderDetailRepository.GetAll();
            var bestSellerProducts = orderDetails.Where(x => x.Order.IsPaid).GroupBy(x => new { x.ProductId, x.Product.ProductName }, (Key, group) => new BestSellerProductViewModel()
            {
                ProductId = Key.ProductId,
                ProductName = Key.ProductName,
                TotalPrice = group.Sum(x => x.Order.TotalPrice),
                TotalSell = group.Sum(x => x.Quantity),
                Product = _productRepository.GetSingleById(Key.ProductId),
            }).OrderByDescending(x => x.TotalSell).Take(12).ToList();
            ViewBag.BestSellerProducts = bestSellerProducts;
        }

        public IActionResult Index()
        {
            GetData();
            return View(_productRepository.GetMulti(x => x.IsDisplay && x.ProductCategory.IsDisplay).OrderByDescending(x => x.CreatedAt).Take(12));
        }

        [Route("Home/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            GetData();
            ViewBag.StatusCode = statusCode;
            return View();
        }
    }
}