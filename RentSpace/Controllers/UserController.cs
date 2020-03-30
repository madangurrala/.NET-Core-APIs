using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public ActionResult Get()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDb.User.FirstOrDefault(u => u.Email == userEmail); 
            user.Password = null;
            user.Token = null;
            return Ok(user);
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
            appDb.User.Add(user);
            //string token = authService.Authenticate(user.Email);
            appDb.SaveChanges();
            user.Token = token;
            user.Password = null;
            return Ok(user);
        }
    }
}