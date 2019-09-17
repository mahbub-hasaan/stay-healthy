using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctor.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Appointment
    {
        public int Id { get; set; }
       
        public DateTime? Date { get; set; }
        [Display(Name = "Running any Medicine?")]
        public bool runningMedicine { get; set; }

        [Display(Name = "Blood Pressure ")]
        public string bp { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Your Problem Description ")]
        public string Problem { get; set; }

        [Required]
        [Display(Name = ("Your Weight?"))]
        public string Weight { get; set; }

        public bool Status { get; set; }

        public virtual Doctors Doctors { get; set; }
        public int DoctorsId { get; set; }

        public virtual Patient Patient { get; set; }
        public int PatientId { get; set; }

    }
}