using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using RentSpace.Models;
using System.Security.Claims;
using System.Collections.Generic;
using RentSpace.Services;

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
            appointmentList = appDbContext.Appointment
                .Where(u => u.UserId == user.Id && u.Status != Static.AptStatusRejected).ToList();

            var appointmentReqList = appDbContext.Appointment.Where(u => u.PeerId == user.Id).ToList();

            if (appointmentReqList.Count <= 0)
            {
                return BadRequest(new { message = "You don't have any appointments" });
            }

            foreach(var appointment in appointmentReqList)
            {
                appointment.PeerId = appointment.UserId;
                appointment.UserId = user.Id;

                var peerTitle = appDbContext.User.FirstOrDefault(u => u.Id == appointment.PeerId);
                appointment.PeerTitle = peerTitle.Name;
            }

            return Ok(appointmentReqList);
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

            if (appointmentList.Count <= 0)
            {
                return BadRequest(new { message = "There are no appointments booked with you" });
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

            var appointment = appDbContext.Appointment.FirstOrDefault(a => a.Id == id && a.PeerId == user.Id);

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

            var property = appDbContext.Property.FirstOrDefault(p => p.Id == appointment.PropertyId);

            if (property == null)
            {
                return BadRequest(new { message = "The property Id doesn't exist" });
            }


            var propertyOwnerAptCheck = appDbContext.Property
                .FirstOrDefault(p => p.Id == property.Id && p.UserId == user.Id);

            if (propertyOwnerAptCheck != null)
            {
                return BadRequest(new { message = "You can not book appointment for the property you have posted" });
            }

            var propertyAppointment = appDbContext.Appointment
                .FirstOrDefault(a => a.UserId == user.Id && a.PropertyId == appointment.PropertyId);


            if (propertyAppointment != null)
            {
                return BadRequest(new { message = "You have already booked an appointment for this property" });
            }

        
            appointment.PeerId = property.UserId;
            appointment.PeerTitle = property.User;
            appointment.RegisterDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            appointment.UserId = user.Id;
            appointment.Status = Static.AptStatusRequested;
            appDbContext.Add(appointment);
            appDbContext.SaveChanges();
            property.AppointmentRequested = true;
            appointment.Property = null;
            appointment.User = null;
            return Ok(property);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public ActionResult Update(int id, Appointment appointment)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userEmail = claimsIdentity.FindFirst(ClaimTypes.Name).Value;

            User user = appDbContext.User.FirstOrDefault(u => u.Email == userEmail);

            var appointmentDb = appDbContext.Appointment.FirstOrDefault(a => a.Id == id && a.PeerId == user.Id);

            if (appointmentDb == null)
            {
                return BadRequest(new { message = "This appointment was not for you" });
            }

            if (appointment.Status == Static.AptStatusAccepted)
            {
                appointmentDb.Status = Static.AptStatusAccepted;
           
            }
            else if (appointment.Status == Static.AptStatusRejected)
            {
                appointmentDb.Status = Static.AptStatusRejected;
            }
            appDbContext.SaveChanges();
            appointmentDb.User = null;
            return Ok(appointmentDb);
        }
    }
}