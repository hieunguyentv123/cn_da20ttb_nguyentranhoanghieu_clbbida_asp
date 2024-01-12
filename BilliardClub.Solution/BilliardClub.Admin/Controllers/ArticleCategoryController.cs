using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ArticleCategoryController : Controller
    {
        private INotyfService _notyf;
        private IArticleCategoryRepository _articleCategoryRepository;

        public ArticleCategoryController(INotyfService notyf, IArticleCategoryRepository articleCategoryRepository)
        {
            _notyf = notyf;
            _articleCategoryRepository = articleCategoryRepository;
        }

        public IActionResult Index(int? page, string? key)
        {
            var list = _articleCategoryRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(key))
            {
                list = list.Where(x => x.ArticleCatName.ToLower().Contains(key.ToLower().Trim()));
            }
            return View(list.Reverse().ToPagedList(pageNumber, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ArticleCatName")] ArticleCategory cat)
        {
            cat.IsDisplay = true;
            if (ModelState.IsValid)
            {
                try
                {
                    _articleCategoryRepository.Add(cat);
                    _articleCategoryRepository.SaveChanges();
                    _notyf.Success("Thêm mới thành công danh mục!", 5);
                    return RedirectToAction("Index", "ArticleCategory");
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
            return View(_articleCategoryRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticleCategory cat)
        {
            try
            {
                _articleCategoryRepository.Update(cat);
                _articleCategoryRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công danh mục!", 5);
                return RedirectToAction("Index", "ArticleCategory");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return View(cat);
        }
        public IActionResult Delete(int id)
        {
            var cat = _articleCategoryRepository.GetSingleById(id);
            return View(cat);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var cat = _articleCategoryRepository.GetSingleById(id);
            if (confirm == cat.ArticleCatName)
            {
                try
                {
                    _articleCategoryRepository.Delete(cat);
                    _articleCategoryRepository.SaveChanges();
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
            return RedirectToAction("Index", "ArticleCategory");
        }
        public IActionResult ChangeStatus(int id)
        {
            var cat = _articleCategoryRepository.GetSingleById(id);
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
                _articleCategoryRepository.Update(cat);
                _articleCategoryRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái danh mục thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "ArticleCategory");
        }
    }
}
