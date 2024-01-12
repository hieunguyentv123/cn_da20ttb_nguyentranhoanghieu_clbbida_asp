using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        void SaveChanges();
    }

    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private IUnitOfWork _unitOfWork;
        public OrderRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
