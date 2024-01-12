using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IProductCategoryRepository : IRepository<ProductCategory>
    {
        void SaveChanges();
    }

    public class ProductCategoryRepository : RepositoryBase<ProductCategory>, IProductCategoryRepository
    {
        private IUnitOfWork _unitOfWork;
        public ProductCategoryRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
