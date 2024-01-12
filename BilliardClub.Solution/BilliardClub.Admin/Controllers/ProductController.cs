using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ProductController : Controller
    {
        private INotyfService _notyf;
        private IProductRepository _productRepository;
        private IProductCategoryRepository _productCategoryRepository;

        public ProductController(INotyfService notyf, IProductRepository productRepository, IProductCategoryRepository productCategoryRepository)
        {
            _notyf = notyf;
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }
        public IActionResult Index(int? page, string? name, string? category)
        {
            GetInfo();
            var list = _productRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(name))
            {
                list = list.Where(x => x.ProductName.ToLower().Contains(name.ToLower().Trim()));
            }
            if (!String.IsNullOrEmpty(category))
            {
                list = list.Where(x => x.CategoryId.Equals(Convert.ToInt32(category)));
            }
            return View(list.OrderByDescending(x => x.CreatedAt).ToPagedList(pageNumber, pageSize));
        }

        private void GetInfo()
        {
            var categoryList = _productCategoryRepository.GetAll();
            ViewBag.CategoryList = categoryList;
        }
        public IActionResult Details(int id)
        {
            return View(_productRepository.GetSingleById(id));
        }

        public IActionResult Create()
        {
            GetInfo();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ProductName,ProductDetail,Price,DiscountPercent,Stock,CategoryId")] Product product, IFormFile? img)
        {
            product.IsDisplay = true;
            product.CreatedAt = DateTime.Now;
            //Xử lý lưu ảnh (nếu có)
            if (img != null && img.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\product");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                product.ProductImage = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Upload ảnh sản phẩm thành công!", 5);
                }
            }
            try
            {
                _productRepository.Add(product);
                _productRepository.SaveChanges();
                _notyf.Success("Thêm mới thành công sản phẩm!", 5);
                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            _notyf.Error("Thêm mới không thành công!", 5);
            GetInfo();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            GetInfo();
            return View(_productRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product, IFormFile? img)
        {
            product.CreatedAt = DateTime.Now;
            //Xử lý lưu ảnh (nếu có)
            if (img != null && img.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\product");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!String.IsNullOrEmpty(product.ProductImage))
                {
                    var oldFilePath = Path.Combine(path, product.ProductImage);
                    if (Directory.Exists(oldFilePath))
                    {
                        Directory.Delete(oldFilePath);
                    }
                }
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                product.ProductImage = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Upload ảnh sản phẩm thành công!", 5);
                }
            }
            try
            {
                _productRepository.Update(product);
                _productRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công sản phẩm!", 5);
                return RedirectToAction("Details", "Product", new { id = product.ProductId });
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            _notyf.Error("Chỉnh sửa không thành công!", 5);
            GetInfo();
            return View(product);
        }
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetSingleById(id);
            return View(product);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var product = _productRepository.GetSingleById(id);
            if (confirm == product.ProductName)
            {
                try
                {
                    _productRepository.Delete(product);
                    _productRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận tên sản phẩm không đúng!", 5);
                return View(product);
            }
            return RedirectToAction("Index", "Product");
        }
        public IActionResult ChangeStatus(int id)
        {
            var product = _productRepository.GetSingleById(id);
            if (product.IsDisplay)
            {
                product.IsDisplay = false;
            }
            else
            {
                product.IsDisplay = true;
            }
            try
            {
                _productRepository.Update(product);
                _productRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái sản phẩm thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Product");
        }
    }
}
