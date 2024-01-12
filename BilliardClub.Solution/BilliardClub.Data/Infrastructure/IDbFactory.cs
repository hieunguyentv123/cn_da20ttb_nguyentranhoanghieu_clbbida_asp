namespace BilliardClub.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        BilliardClubDbContext Init();
    }
}
