using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentSpace.Models;
using RentSpace.Services;

namespace RentSpace.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDb;

        private readonly IAuthenticateService authService;

        public UserController(AppDbContext dbContext, IAuthenticateService authService)
        {
            this.appDb = dbContext;
            this.authService = authService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult Get(int id)
        {

            User user = new User();
            user = appDb.User.FirstOrDefault(u => u.Id == id);
            user.Password = null;
            return BadRequest(user);
        }

        [HttpPost]
        public IActionResult Post(User user)
        {

            User userFromDb = appDb.User.FirstOrDefault(u => u.Email == user.Email);
            if (userFromDb != null)
            {
                return BadRequest(new { message = "Email already exists", user.Email });
            }

            user.Password = PasswordHandler.Encrypt(user.Password, "sblw-3hn8-sqoy19");
            string token = authService.Authenticate(user.Email);
            user.Token = token;
            appDb.User.Add(user);
            //string token = authService.Authenticate(user.Email);
            appDb.SaveChanges();
            user.Password = null;
            return Ok(user);
        }
    }
}