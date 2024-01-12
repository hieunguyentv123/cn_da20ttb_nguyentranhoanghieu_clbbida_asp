using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ProductCategoryController : Controller
    {
        private INotyfService _notyf;
        private IProductCategoryRepository _productCategoryRepository;

        public ProductCategoryController(INotyfService notyf, IProductCategoryRepository productCategoryRepository)
        {
            _notyf = notyf;
            _productCategoryRepository = productCategoryRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _productCategoryRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(key))
            {
                list = list.Where(x => x.CategoryName.ToLower().Contains(key.ToLower().Trim()));
            }
            return View(list.OrderBy(x => x.CategoryName).ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("CategoryName")]ProductCategory cat)
        {
            cat.IsDisplay = true;
            cat.DataFilter = RemoveUnicode(cat.CategoryName).Trim().Replace(" ", "");
            if (ModelState.IsValid)
            {
                try
                {
                    _productCategoryRepository.Add(cat);
                    _productCategoryRepository.SaveChanges();
                    _notyf.Success("Thêm mới thành công danh mục!", 5);
                    return RedirectToAction("Index", "ProductCategory");
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            return View(cat);
        }

        public IActionResult Edit(int id)
        {
            return View(_productCategoryRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductCategory cat)
        {
            cat.DataFilter = RemoveUnicode(cat.CategoryName).ToLower().Replace(" ", "");
            try
            {
                _productCategoryRepository.Update(cat);
                _productCategoryRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công danh mục!", 5);
                return RedirectToAction("Index", "ProductCategory");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return View(cat);
        }
        public IActionResult Delete(int id)
        {
            var cat = _productCategoryRepository.GetSingleById(id);
            return View(cat);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var cat = _productCategoryRepository.GetSingleById(id);
            if (confirm == cat.CategoryName)
            {
                try
                {
                    _productCategoryRepository.Delete(cat);
                    _productCategoryRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận tên danh mục không đúng!", 5);
                return View(cat);
            }
            return RedirectToAction("Index", "ProductCategory");
        }
        public IActionResult ChangeStatus(int id)
        {
            var cat = _productCategoryRepository.GetSingleById(id);
            if (cat.IsDisplay)
            {
                cat.IsDisplay = false;
            }
            else
            {
                cat.IsDisplay = true;
            }
            try
            {
                _productCategoryRepository.Update(cat);
                _productCategoryRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái danh mục thành công!", 5);
                
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "ProductCategory");
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
    }
}
