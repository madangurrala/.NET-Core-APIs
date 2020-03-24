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
    public class PropertyController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public PropertyController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult Get()
        {
            List<Property> properties = new List<Property>();
            var propertiesFromDb = appDbContext.Property.ToList();

            if(propertiesFromDb.Count <= 0)
            {
                return BadRequest(new { message = "There are no properties to display"});
            }

            return Ok(propertiesFromDb);


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public ActionResult Post(Property property)
        {
;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if(user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            property.RegisterDate = DateTime.Now.ToString();
            property.UserId = user.Id;
            property.User = user.Name;
            property.Status = Static.PropertyPosted;
            property.UserObject.Password = null;
            appDbContext.Property.Add(property);
            appDbContext.SaveChanges();
            return Ok(property);
            
        }


    }
}