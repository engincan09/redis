using Redis.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Api.Services.Abstract
{
    public interface IUserRepository
    {
        List<User> GetAllUser();
    }
}
