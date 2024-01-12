using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Globalization;

namespace BilliardClub.Client.Controllers
{
    public class CartController : Controller
    {
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        public const string CARTKEY = "cart";
        public CartController(INotyfService notyf, ILayoutDataReceiver layoutDataReceiver)
        {
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
        List<CartItemViewModel> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItemViewModel>>(jsoncart);
            }
            return new List<CartItemViewModel>();
        }
        // Lưu Cart (Danh sách CartItemViewModel) vào session
        void SaveCartSession(List<CartItemViewModel> cartItems)
        {
            var session = HttpContext.Session;
            string cart = JsonConvert.SerializeObject(cartItems, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            session.SetString(CARTKEY, cart);
        }
        public IActionResult RemoveItem(int id)
        {
            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var removeItem = cart.Find(p => p.Product.ProductId == id);
            cart.Remove(removeItem);
            // Lưu cart vào Session
            SaveCartSession(cart);
            _notyf.Success("Xoá sản phẩm khỏi giỏ hàng thành công", 5);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
            _notyf.Success("Xoá tất cả sản phẩm khỏi giỏ hàng thành công", 5);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult IncItem(int id)
        {
            // Xử thêm số lượng vào Cart ...
            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.ProductId == id);
            if ((item.Quantity + 1) > item.Product.Stock)
            {
                _notyf.Success("Số lượng sản phẩm vượt quá số lượng tồn, không thể tăng", 5);
            }
            else
            {
                item.Quantity += 1;
            }
            SaveCartSession(cart);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult DecItem(int id)
        {
            // Xử thêm số lượng vào Cart ...
            var cart = GetCartItems();
            var item = cart.Find(p => p.Product.ProductId == id);
            if ((item.Quantity - 1) <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity -= 1;
            }
            SaveCartSession(cart);
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult Index()
        {
            GetData();
            return View(GetCartItems());
        }
        
    }
}
