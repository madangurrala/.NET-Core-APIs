using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RentSpace.Models
{
    public class Property
    {
        public int Id { get; set; }

        public string BigImagePath { get; set; }
        public string SmallImagePath { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public long RegisterDate { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        public float Rate { get; set; }

        [ForeignKey("UserId")]
        public virtual User UserObject { get; set; }

    }
}
