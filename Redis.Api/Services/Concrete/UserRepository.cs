using Redis.Api.Models;
using Redis.Api.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Api.Services.Concrete
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetAllUser()
        {
            List<User> users = new List<User>() {
                new User(){ Id =1, Name="Engin",Surname="CAN",Age=23},
                new User(){ Id =2, Name="Deniz",Surname="CAN",Age=8},
            };

            return users;
        }
    }
}
