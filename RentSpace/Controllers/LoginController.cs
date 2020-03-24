using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RentSpace.Models;
using Microsoft.EntityFrameworkCore;
using RentSpace.Services;


namespace RentSpace.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IAuthenticateService authService;

        public LoginController(AppDbContext appDbContext, IAuthenticateService authService)
        {
            this.appDbContext = appDbContext;
            this.authService = authService;
        }

        [HttpPost]
        public ActionResult Post([FromBody] User userObj)
        {
            try
            {
                User user = appDbContext.User.SingleOrDefault(p => p.Email == userObj.Email);
                
                if (user != null && userObj.Password == PasswordHandler.Decrypt(user.Password, "sblw-3hn8-sqoy19"))
                {
                    string token = authService.Authenticate(user.Email);
                    user.Token = token;

                    user.Password = null;
                    return Ok(user);
                }
                else
                {
                    return BadRequest(new { message = "Email or password is wrong",});
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}