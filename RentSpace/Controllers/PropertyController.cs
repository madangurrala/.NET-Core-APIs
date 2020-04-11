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
            var propertiesFromDb = appDbContext.Property.Where(p => p.Status == Static.PropertyPosted).ToList();

            if (propertiesFromDb.Count <= 0)
            {
                return BadRequest(new { message = "There are no properties to display" });
            }

            return Ok(propertiesFromDb);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public ActionResult GetProperty(int id)
        {

            Property property = appDbContext.Property.FirstOrDefault(p => p.Id == id);

            if (property == null)
            {
                return BadRequest(new { message = "Please enter a valid property Id" });
            }

            return Ok(property);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public ActionResult Post(Property property)
        {
            ;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            property.RegisterDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            property.UserId = user.Id;
            property.User = user.Name;
            property.Status = Static.PropertyPosted;
            appDbContext.Property.Add(property);
            appDbContext.SaveChanges();
            property.UserObject = null;
            return Ok(property);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public ActionResult UpdateProperty(Property property, int id)
        {
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            var propertyFromDb = appDbContext.Property.FirstOrDefault(p => p.Id == id && p.UserId == user.Id);

            if (propertyFromDb == null)
            {
                return BadRequest(new { message = "The property doesn't exist/The given property is not posted by you" });
            }

            propertyFromDb.RegisterDate = propertyFromDb.RegisterDate;
            propertyFromDb.UserId = propertyFromDb.UserId;
            propertyFromDb.User = propertyFromDb.User;
            if (property.Status == Static.PopertyRented)
            {
                propertyFromDb.Status = Static.PopertyRented;
            }
            propertyFromDb.Status = propertyFromDb.Status;
            propertyFromDb.BigImagePath = property.BigImagePath;
            propertyFromDb.SmallImagePath = property.SmallImagePath;
            propertyFromDb.ShortDescription = property.ShortDescription;
            propertyFromDb.LongDescription = property.LongDescription;
            propertyFromDb.Latitude = property.Latitude;
            propertyFromDb.Longitude = property.Longitude;
            propertyFromDb.Address = property.Address;
            propertyFromDb.City = property.City;
            propertyFromDb.Size = property.Size;
            propertyFromDb.Price = property.Price;
            propertyFromDb.Rate = property.Rate;
            appDbContext.SaveChanges();
            propertyFromDb.UserObject = null;
            return Ok(propertyFromDb);

        }


        


    }
}