using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        void SaveChanges();
    }

    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private IUnitOfWork _unitOfWork;
        public ProductRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
