using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Admin.Controllers
{
    public class ArticleController : Controller
    {
        private INotyfService _notyf;
        private IArticleRepository _articleRepository;
        private IArticleCategoryRepository _articleCategoryRepository;
        private IUserRepository _userRepository;

        public ArticleController(INotyfService notyf, IArticleRepository articleRepository, IUserRepository userRepository, IArticleCategoryRepository articleCategoryRepository)
        {
            _notyf = notyf;
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _articleCategoryRepository = articleCategoryRepository;
        }

        public IActionResult Index(int? page, string? title, string? category)
        {
            GetInfo();
            var list = _articleRepository.GetAll();
            if (page == null) page = 1;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(title))
            {
                list = list.Where(x => x.Title.ToLower().Contains(title.ToLower().Trim()));
            }
            if (!String.IsNullOrEmpty(category))
            {
                list = list.Where(x => x.CategoryId.Equals(Convert.ToInt32(category)));
            }
            return View(list.Reverse().ToPagedList(pageNumber, pageSize));
        }

        private void GetInfo()
        {
            var categoryList = _articleCategoryRepository.GetAll();
            ViewBag.CategoryList = categoryList;
        }

        public IActionResult Details(int id)
        {
            return View(_articleRepository.GetSingleById(id));
        }

        public IActionResult Create()
        {
            GetInfo();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Content,VideoPath,CategoryId")] Article article, IFormFile? img)
        {
            //Thông tin bài viết
            article.CreateBy = (int)HttpContext.Session.GetInt32("_id");
            article.CreatedAt = DateTime.Now;
            article.IsDisplay = true;
            article.ViewCount = 0;
            //Xử lý lưu ảnh (nếu có)
            if (img != null && img.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\articleimg");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                article.ImagePath = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Upload ảnh bài viết thành công!", 5);
                }
            }
            try
            {
                _articleRepository.Add(article);
                _articleRepository.SaveChanges();
                _notyf.Success("Thêm mới thành công bài viết!", 5);
                return RedirectToAction("Index", "Article");
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            _notyf.Error("Thêm mới không thành công!", 5);
            GetInfo();
            return View(article);
        }

        public IActionResult Edit(int id)
        {
            GetInfo();
            return View(_articleRepository.GetSingleById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Article article, IFormFile? img)
        {
            //Xử lý lưu ảnh (nếu có)
            if (img != null && img.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "..\\shared\\articleimg");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!String.IsNullOrEmpty(article.ImagePath))
                {
                    var oldFilePath = Path.Combine(path, article.ImagePath);
                    if (Directory.Exists(oldFilePath))
                    {
                        Directory.Delete(oldFilePath);
                    }
                }
                string fileName = Path.GetFileName(img.FileName);
                fileName = Guid.NewGuid().ToString() + fileName;
                article.ImagePath = fileName;
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    img.CopyTo(stream);
                    _notyf.Success($"Upload ảnh bài viết thành công!", 5);
                }
            }
            try
            {
                _articleRepository.Update(article);
                _articleRepository.SaveChanges();
                _notyf.Success("Cập nhật thành công bài viết!", 5);
                return RedirectToAction("Details", "Article", new { id = article.ArticleId });
            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            _notyf.Error("Chỉnh sửa không thành công!", 5);
            GetInfo();
            return View(article);
        }
        public IActionResult Delete(int id)
        {
            var article = _articleRepository.GetSingleById(id);
            return View(article);
        }
        [HttpPost]
        public IActionResult Delete(int id, string confirm)
        {
            var article = _articleRepository.GetSingleById(id);
            if (confirm == article.Title)
            {
                try
                {
                    _articleRepository.Delete(article);
                    _articleRepository.SaveChanges();
                    _notyf.Success("Xóa thành công", 5);
                }
                catch (Exception ex)
                {
                    _notyf.Error(ex.Message, 5);
                }
            }
            else
            {
                _notyf.Error("Xác nhận tiêu đề không đúng!", 5);
                return View(article);
            }
            return RedirectToAction("Index", "Article");
        }
        public IActionResult ChangeStatus(int id)
        {
            var article = _articleRepository.GetSingleById(id);
            if (article.IsDisplay)
            {
                article.IsDisplay = false;
            }
            else
            {
                article.IsDisplay = true;
            }
            try
            {
                _articleRepository.Update(article);
                _articleRepository.SaveChanges();
                _notyf.Success("Cập nhật trạng thái danh mục thành công!", 5);

            }
            catch (Exception ex)
            {
                _notyf.Error(ex.Message, 5);
            }
            return RedirectToAction("Index", "Article");
        }
    }
}
