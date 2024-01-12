using AspNetCoreHero.ToastNotification.Abstractions;
using BilliardClub.Client.DataReceivers;
using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BilliardClub.Client.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleRepository _articleRepository;
        private IArticleCategoryRepository _articleCategoryRepository;
        private INotyfService _notyf;
        private ILayoutDataReceiver _layoutDataReceiver;
        public ArticleController(IArticleRepository articleRepository, INotyfService notyf, ILayoutDataReceiver layoutDataReceiver, IArticleCategoryRepository articleCategoryRepository)
        {
            _articleRepository = articleRepository;
            _notyf = notyf;
            _layoutDataReceiver = layoutDataReceiver;
            _articleCategoryRepository = articleCategoryRepository;
        }
        private void GetData()
        {
            HomeViewModel model = _layoutDataReceiver.GetData();
            foreach (var item in model.ArticleCategories)
            {
                ICollection<Article> articles = _articleRepository.GetMulti(x => x.CategoryId == item.ArticleCatId).ToList();
                ViewData[item.ArticleCatName + "Count"] = articles.Count();
            }
            ViewBag.ArticleCategories = model.ArticleCategories;
            ViewBag.ProductCategories = model.ProductCategories;
            ViewBag.ContactInformations = model.ContactInformations;
            
        }
        public IActionResult Index(int? page, string? title, string? category)
        {
            GetData();
            var list = _articleRepository.GetAll().Where(x => x.IsDisplay);
            if (page == null) page = 1;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(title))
            {
                list = list.Where(x => x.Title.ToLower().Contains(title.ToLower().Trim()));
            }
            if (!String.IsNullOrEmpty(category))
            {
                ArticleCategory articleCategory = _articleCategoryRepository.GetSingleById((Convert.ToInt32(category)));
                if (articleCategory == null || articleCategory.IsDisplay == false)
                {
                    _notyf.Error("Danh mục không tồn tại!", 5);
                    return RedirectToAction("Index", "Article");
                }
                ViewData["Category"] = "Danh sách bài viết thuộc danh mục " + articleCategory.ArticleCatName;
                list = list.Where(x => x.CategoryId.Equals(Convert.ToInt32(category)));
            }
            else
            {
                ViewData["Category"] = "Danh sách tất cả bài viết";
            }
            return View(list.OrderByDescending(x => x.CreatedAt).ToPagedList(pageNumber, pageSize));
        }
        public IActionResult Details(int id)
        {
            GetData();
            var article = _articleRepository.GetSingleById(id);
            article.ViewCount++;
            _articleRepository.Update(article);
            _articleRepository.SaveChanges();
            if (!article.IsDisplay)
            {
                _notyf.Error("Bài viết đã bị xoá hoặc ẩn đi", 5);
                return RedirectToAction("Index", "Article");
            }
            ViewBag.RelatedArticles = _articleRepository.GetMulti(x => x.ArticleId != id && x.IsDisplay && x.CategoryId == article.CategoryId).OrderByDescending(x => x.CreatedAt).Take(5);
            return View(article);
        }
    }
}
