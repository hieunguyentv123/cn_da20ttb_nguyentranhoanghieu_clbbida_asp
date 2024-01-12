using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Globalization;
using X.PagedList;

namespace BilliardClub.Client.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        private IProductCategoryRepository _productCategoryRepository;
        private IProductReviewRepository _productReviewRepository;
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        public const string CARTKEY = "cart";
        public ProductController(IProductRepository productRepository, INotyfService notyf, ILayoutDataReceiver layoutDataReceiver, IProductCategoryRepository articleCategoryRepository, IProductReviewRepository productReviewRepository)
        {
            _productRepository = productRepository;
            _notyf = notyf;
            _layoutDataReceiver = layoutDataReceiver;
            _productCategoryRepository = articleCategoryRepository;
            _productReviewRepository = productReviewRepository;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            foreach (var item in model.ProductCategories)
            {
                ICollection<Product> product = _productRepository.GetMulti(x => x.CategoryId == item.CategoryId).ToList();
                ViewData[item.CategoryName + "Count"] = product.Count();
            }
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;

        }
        public IActionResult Index(int? page, string? name, string? category)
        {
            GetData();
            var list = _productRepository.GetAll().Where(x => x.IsDisplay && x.ProductCategory.IsDisplay);
            if (page == null) page = 1;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(name))
            {
                list = list.Where(x => x.ProductName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (!String.IsNullOrEmpty(category))
            {
                ProductCategory productCategory = _productCategoryRepository.GetSingleById((Convert.ToInt32(category)));
                if (productCategory == null || productCategory.IsDisplay == false)
                {
                    _notyf.Error("Danh mục không tồn tại!", 5);
                    return RedirectToAction("Index", "Product");
                }
                ViewData["Category"] = "Danh sách sản phẩm " + productCategory.CategoryName;
                list = list.Where(x => x.CategoryId.Equals(Convert.ToInt32(category)));
            }
            else
            {
                ViewData["Category"] = "Danh sách tất cả sản phẩm";
            }
            return View(list.OrderBy(x => x.ProductName).ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Details(int id)
        {
            GetData();
            var product = _productRepository.GetSingleById(id);
            ICollection<Product> products = _productRepository.GetMulti(x => x.CategoryId == product.CategoryId && x.ProductId != product.ProductId).OrderBy(x => x.ProductName).ToList();
            ViewBag.RelatedProducts = products;
            ICollection<ProductReview> reviews = _productReviewRepository.GetMulti(x => x.ProductId == product.ProductId).OrderByDescending(x => x.ReviewAt).ToList();
            ViewData["ReviewsCount"] = reviews.Count();
            ViewBag.Reviews = reviews;
            if (!product.IsDisplay || !product.ProductCategory.IsDisplay)
            {
                _notyf.Error("Sản phẩm không tồn tại!", 5);
                return RedirectToAction("Index", "Product");
            }
            return View(product);
        }
        public IActionResult Review(int productId, string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                _notyf.Information("Hãy điền đánh giá của bạn vào nhé", 4);
            }
            else
            {
                ProductReview review = new ProductReview()
                {
                    ReviewAt = DateTime.Now,
                    ReviewBy = (int)HttpContext.Session.GetInt32("_id"),
                    Content = content,
                    ProductId = productId,
                };
                _productReviewRepository.Add(review);
                _productReviewRepository.SaveChanges();
                _notyf.Success("Cảm ơn bạn đã để lại đánh giá sản phẩm", 4);
            }
            return RedirectToAction("Details", "Product", new { id = productId });
        }
        public IActionResult DeleteReview(int reviewId, int productId)
        {
            _productReviewRepository.Delete(reviewId);
            _productReviewRepository.SaveChanges();
            _notyf.Success("Xoá đánh giá thành công!", 4);
            return RedirectToAction("Details", "Product", new { id = productId });
        }
        // Lấy cart từ Session (danh sách CartItemViewModel)
        List<CartItemViewModel> GetCartItems()
        {

            var session = HttpContext.Session;
            string cart = session.GetString(CARTKEY);
            if (cart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItemViewModel>>(cart);
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
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _productRepository.GetSingleById(productId);
            if (product == null)
            {
                _notyf.Error("Không có sản phẩm", 5);
            }
            else
            {
                // Xử lý đưa vào Cart ...
                var cart = GetCartItems();
                var cartitem = cart.Find(p => p.Product.ProductId == productId);
                if (cartitem != null)
                {
                    // Đã tồn tại, tăng thêm 1
                    cartitem.Quantity+= quantity;
                }
                else
                {
                    //  Thêm mới
                    cart.Add(new CartItemViewModel() { Quantity = quantity, Product = product });
                }

                // Lưu cart vào Session
                SaveCartSession(cart);
                _notyf.Success("Thêm sản phẩm vào giỏ hàng thành công", 5);
            }
            return RedirectToAction("Index", "Cart");
        }
    }
}
