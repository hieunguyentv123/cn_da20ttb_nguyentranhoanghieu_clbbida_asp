using BilliardClub.Client.Models;
using BilliardClub.Data.Repositories;
using BilliardClub.Model;

namespace BilliardClub.Client.DataReceivers
{
    public interface ILayoutDataReceiver
    {
        HomeViewModel GetData();
    }
    public class LayoutDataReceiver : ILayoutDataReceiver
    {
        private IArticleCategoryRepository _articleCategoryRepository;
        private IProductCategoryRepository _productCategoryRepository;
        private IContactInformationRepository _contactInformationRepository;
        private ISlideRepository _slideRepository;
        public LayoutDataReceiver(IArticleCategoryRepository articleCategoryRepository, IProductCategoryRepository productCategoryRepository, IContactInformationRepository contactInformationRepository, ISlideRepository slideRepository)
        {
            _articleCategoryRepository = articleCategoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _contactInformationRepository = contactInformationRepository;
            _slideRepository = slideRepository;
        }
        public HomeViewModel GetData()
        {
            HomeViewModel model = new HomeViewModel();
            model.ArticleCategories = _articleCategoryRepository.GetAll().Where(x => x.IsDisplay).OrderBy(x => x.ArticleCatName);
            model.ProductCategories = _productCategoryRepository.GetAll().Where(x => x.IsDisplay).OrderBy(x => x.CategoryName);
            model.ContactInformations = _contactInformationRepository.GetAll().Where(x => x.IsDisplay).OrderBy(x => x.ContactInfoId);
            model.Slides = _slideRepository.GetAll().Where(x => x.IsDisplay).OrderBy(x => x.SlideId);
            return model;
        }
    }
}
