using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IContactInformationRepository : IRepository<ContactInformation>
    {
        void SaveChanges();
    }

    public class ContactInformationRepository : RepositoryBase<ContactInformation>, IContactInformationRepository
    {
        private IUnitOfWork _unitOfWork;
        public ContactInformationRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
