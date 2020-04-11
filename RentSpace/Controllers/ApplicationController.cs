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
    public class ApplicationController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public ApplicationController(AppDbContext appDbContext)
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

            if (user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            List<Application> applcationList = new List<Application>();
            applcationList = appDbContext
                .Application.Where(a => a.UserId == user.Id && a.Status != Static.AptStatusRejected).ToList();

            var applicationReqList = appDbContext.Application.Where(a => a.PeerId == user.Id).ToList();

            foreach (var application in applicationReqList)
            {
                applcationList.Add(application);
            }

            if (applcationList.Count <= 0)
            {
                return BadRequest(new { message = "You don't have any applications" });
            }
            
            return Ok(applcationList);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public ActionResult Apply(Application application)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            var property = appDbContext.Property.FirstOrDefault(P => P.Id == application.PropertyId);

            if (property == null)
            {
                return BadRequest(new { message = "The property doesn't exist" });
            }

            var propertyOwnerAptCheck = appDbContext.Property
                .FirstOrDefault(p => p.Id == property.Id && p.UserId == user.Id);

            if (propertyOwnerAptCheck != null)
            {
                return BadRequest(new { message = "You can not apply for the property you have posted" });
            }

            var applicationCheck = appDbContext.Application
                .FirstOrDefault(a => a.UserId == user.Id && a.PropertyId == application.PropertyId);
            if (applicationCheck != null)
            {
                return BadRequest(new { message = "You have already submitted application for this property" });
            }

            application.UserId = user.Id;
            application.PeerId = property.UserId;
            application.ApplicantName = user.Name;
            application.Status = Static.Applied;
            appDbContext.Application.Add(application);
            appDbContext.SaveChanges();
            application.User = null;
            application.Property = null;
            return Ok(application);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public ActionResult Update(int id, Application application)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            var applicationDb = appDbContext.Application.FirstOrDefault(a => a.Id == id && a.PeerId == user.Id);

            if (applicationDb == null)
            {
                return BadRequest(new { message = "This application was not for you" });
            }

            if (application.Status == Static.AptStatusAccepted)
            {
                applicationDb.Status = Static.AptStatusAccepted;

            }
            else if (application.Status == Static.AptStatusRejected)
            {
                applicationDb.Status = Static.AptStatusRejected;
            }
            appDbContext.SaveChanges();
            applicationDb.User = null;
            return Ok(applicationDb);
        }
    }
}