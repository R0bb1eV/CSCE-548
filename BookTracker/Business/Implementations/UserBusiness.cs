using BookTracker.Business.Interfaces;
using System;
using System.Collections.Generic;

namespace BookTracker.Business.Implementations
{
    public class UserBusiness : IUserBusiness
    {
        private readonly DataProvider _dataProvider;

        public UserBusiness(DataProvider dataProvider)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        public void AddUser(User user)
        {
            _dataProvider.CreateUser(user);
        }

        public List<User> GetAllUsers()
        {
            return _dataProvider.ReadAllUsers();
        }

        public User? GetUserById(int id)
        {
            return _dataProvider.ReadUserById(id);
        }

        public void UpdateUser(User user)
        {
            _dataProvider.UpdateUser(user);
        }

        public void RemoveUser(int id)
        {
            _dataProvider.DeleteUser(id);
        }
    }
}
