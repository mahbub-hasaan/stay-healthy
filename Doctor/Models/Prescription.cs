using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctor.Models;
using System.ComponentModel.DataAnnotations;
namespace Doctor.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        [Required]
        public String Advice { get; set; }

        public virtual Appointment Appointment { get; set; }
        public int AppointmentId { get; set; }
    }
}