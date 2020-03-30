using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using RentSpace.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace RentSpace.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public AppointmentController(AppDbContext appDbContext)
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

            List<Appointment> appointmentList = new List<Appointment>();
            appointmentList = appDbContext.Appointment.Where(u => u.UserId == user.Id).ToList();

            if (appointmentList.Count <= 0)
            {
                return BadRequest(new { message = "You haven't booked any appointments" });
            }

            return Ok(appointmentList);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public ActionResult GetAppointment(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            var appointment = appDbContext.Appointment.FirstOrDefault(a => a.Id == id && a.UserId == user.Id);

            if (appointment == null)
            {
                return BadRequest(new { message = "Please enter a valid appointment Id" });
            }

            return Ok(appointment);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAll")]
        public ActionResult GetAll()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            List<Appointment> appointmentList = new List<Appointment>();
            appointmentList = appDbContext.Appointment.Where(u => u.PeerId == user.Id).ToList();

            if(appointmentList.Count <= 0)
            {
                return BadRequest(new {message = "There are no appointments booked with you" });
            }

            return Ok(appointmentList);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAll/{id}")]
        public ActionResult GetPropertyAppointment(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

           var appointment = appDbContext.Appointment.FirstOrDefault(a => a.Id == id && a.PeerId == user.Id );

            if (appointment == null)
            {
                return BadRequest(new { message = "Please enter a valid appointment Id" });
            }

            return Ok(appointment);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public ActionResult Post(Appointment appointment)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return BadRequest(new { message = "User is not logged in" });
            }

            var peerUser = appDbContext.User.FirstOrDefault(u => u.Id == appointment.PeerId);

            if(peerUser == null)
            {
                return BadRequest(new { message = "The peer user doesn't exist" });
            }

            appointment.PeerTitle = peerUser.Name;
            appointment.RegisterDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            appointment.UserId = user.Id;
            appDbContext.Add(appointment);
            appDbContext.SaveChanges();
            return Ok(appointment);

        }

    }
}