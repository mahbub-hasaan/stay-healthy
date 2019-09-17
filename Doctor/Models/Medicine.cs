﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctor.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Medicine
    {
        public int Id { get; set; }
        [Display(Name = "Medicine Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "MG/ML")]
        [Required]
        public double Amount { get; set; }
        [Required]
        public int Dose { get; set; }
        [Required]
        public int Day { get; set; }
        [Required]
        public string Comment { get; set; }

        public Appointment Appointment { get; set; }
        public int AppointmentId { get; set; }

    }
}