using BilliardClub.Data.Infrastructure;
using BilliardClub.Model;
using System.Security.Cryptography;
using System.Text;

namespace BilliardClub.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        bool UsernameCheck(string username);
        User ClientLogin(string username, string password);
        User AdministratorLogin(string username, string password);
        bool ChangePassword(string userId, string oldPassword, string newPassword);
        string MD5Hash(string str);
        void SaveChanges();
    }

    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private IUnitOfWork _unitOfWork;
        public UserRepository(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory)
        {
            _unitOfWork = unitOfWork;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public User ClientLogin(string username, string password)
        {
            string hashedPassword = MD5Hash(password);
            var user = DbContext.Users.Where(x => x.Username == username && x.Password == hashedPassword).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public User AdministratorLogin(string username, string password)
        {
            string hashedPassword = MD5Hash(password);
            var user = DbContext.Users.Where(x => x.Username == username && x.Password == hashedPassword && x.IsAdministrator == true).FirstOrDefault();
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = GetSingleByCondition(x => x.Username == username);
            bool isValid = MD5Hash(oldPassword) == user.Password;
            if (isValid)
            {
                user.Password = MD5Hash(newPassword);
                Update(user);
                SaveChanges();
                return true;
            }
            return false;
        }

        public bool UsernameCheck(string username)
        {
            User user = DbContext.Users.Where(x => x.Username == username).FirstOrDefault();
            return user == null;
        }
    }
}