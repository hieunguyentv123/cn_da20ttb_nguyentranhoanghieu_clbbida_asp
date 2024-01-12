using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void SaveChanges();
    }

    public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository
    {
        private IUnitOfWork _unitOfWork;
        public PaymentRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
