using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IContactRepository : IRepository<Contact>
    {
        void SaveChanges();
    }

    public class ContactRepository : RepositoryBase<Contact>, IContactRepository
    {
        private IUnitOfWork _unitOfWork;
        public ContactRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
