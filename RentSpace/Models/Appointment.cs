using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentSpace.Models
{
    public class Appointment 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PeerId { get; set; }
        public string PeerTitle { get; set; }
        public long RegisterDate { get; set; }
        public long AppointmentDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
