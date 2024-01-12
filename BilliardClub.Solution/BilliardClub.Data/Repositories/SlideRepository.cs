using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface ISlideRepository : IRepository<Slide>
    {
        void SaveChanges();
    }

    public class SlideRepository : RepositoryBase<Slide>, ISlideRepository
    {
        private IUnitOfWork _unitOfWork;
        public SlideRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
