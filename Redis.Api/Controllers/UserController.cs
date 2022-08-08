using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Redis.Api.Models;
using Redis.Api.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Redis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _distributedCache;
        public UserController(IUserRepository userRepository, IDistributedCache distributedCache)
        {
            _userRepository = userRepository;
            _distributedCache = distributedCache;
        }


        [HttpGet]
        public async Task<List<User>> GetAllUser()
        {
            var userList = new List<User>();
            string cacheJsonItem;

            //Verilen key değerine göre bellekte bu değerleri sorguluyoruz.
            var usersFromCache = await _distributedCache.GetAsync("users");

            //Eğer bellekde bir değer varsa bunu okuyup string olarak alıyoruz. Belleğe json olarak yazdığımız için değerleri deserialize edip alıyoruz.
            if (usersFromCache != null)
            {
                cacheJsonItem = Encoding.UTF8.GetString(usersFromCache);
                userList = JsonConvert.DeserializeObject<List<User>>(cacheJsonItem);
            }
            else
            {
                //Eğer bellekte yok ise ilk önce servisten okuyup belleğe yazıyoruz. Json olarak kaydediyoruz.
                userList = await Task.Run(() => _userRepository.GetAllUser());
                cacheJsonItem =JsonConvert.SerializeObject(userList);
                usersFromCache = Encoding.UTF8.GetBytes(cacheJsonItem);

                var options = new DistributedCacheEntryOptions()
                             //Verinin verilen AbsoluteExpiration süresi    içerisinde çağırılır ise verinin tutulma süresini bir o kadar daha uzatmasını sağlar
                              .SetSlidingExpiration(TimeSpan.FromDays(1))
                              //Verinin bellek üzerinde tutulma süresi
                              .SetAbsoluteExpiration(DateTime.Now.AddMonths (1));

                await _distributedCache.SetAsync("users", usersFromCache, options);
            }

            return userList;
        }
    }
}
