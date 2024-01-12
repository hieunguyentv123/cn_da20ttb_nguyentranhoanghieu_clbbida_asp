using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        void SaveChanges();
    }

    public class ProductReviewRepository : RepositoryBase<ProductReview>, IProductReviewRepository
    {
        private IUnitOfWork _unitOfWork;
        public ProductReviewRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
