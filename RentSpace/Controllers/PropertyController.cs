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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);
            List<Property> properties = new List<Property>();
            var propertiesFromDb = appDbContext.Property.Where(p => p.Status == Static.PropertyPosted).ToList();


            if (propertiesFromDb.Count <= 0)
            {
                return BadRequest(new { message = "There are no properties to display" });
            }


            foreach (var property in propertiesFromDb)
            {
                var propAptFromDb = appDbContext.Appointment
                    .FirstOrDefault(a => a.UserId == user.Id && a.PropertyId == property.Id);

                if (propAptFromDb != null)
                {
                    property.AppointmentRequested = true;
                }
                else
                {
                    property.AppointmentRequested = false;
                }
            } 

            return Ok(propertiesFromDb);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public ActionResult GetProperty(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            Property property = appDbContext.Property.FirstOrDefault(p => p.Id == id);

            if (property == null)
            {
                return BadRequest(new { message = "Please enter a valid property Id" });
            }

            var propertyAppointment = appDbContext.Appointment
             .FirstOrDefault(a => a.UserId == user.Id && a.PropertyId == property.Id);

           if(propertyAppointment != null)
            {
                property.AppointmentRequested = true;
            }
            else
            {
                property.AppointmentRequested = false;
            }

            return Ok(property);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public ActionResult Post(Property property)
        {
    
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
            property.AppointmentRequested = false;
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

            if (property.Status == Static.PopertyRented)
            {
                propertyFromDb.Status = Static.PopertyRented;
            }
            appDbContext.SaveChanges();
            propertyFromDb.UserObject = null;
            return Ok(propertyFromDb);

        }

    }
}