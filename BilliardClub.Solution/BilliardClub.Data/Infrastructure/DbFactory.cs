namespace BilliardClub.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        BilliardClubDbContext dbContext;
        public BilliardClubDbContext Init()
        {
            return dbContext ?? (dbContext = new BilliardClubDbContext());
        }
        protected override void DisposeCore()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}
