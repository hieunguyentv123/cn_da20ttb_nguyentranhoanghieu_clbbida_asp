using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;

namespace BilliardClub.Data.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        void SaveChanges();
    }

    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        private IUnitOfWork _unitOfWork;
        public ArticleRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
