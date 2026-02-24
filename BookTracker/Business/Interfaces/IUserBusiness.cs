using System.Collections.Generic;

namespace BookTracker.Business.Interfaces
{
    public interface IUserBusiness
    {
        void AddUser(User user);
        List<User> GetAllUsers();
        User? GetUserById(int id);
        void UpdateUser(User user);
        void RemoveUser(int id);
    }
}
