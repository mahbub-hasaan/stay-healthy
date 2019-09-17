using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctor.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Report
    {
        public int Id { get; set; }
        [Display(Name = "Test Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        public bool Status { get; set; }
        [Display(Name = "Upload Report file")]
        public string ImagePath { get; set; }

        [NotMapped]
        [Required]
        public HttpPostedFileBase Imagefile { get; set; }

        public virtual Appointment Appointment { get; set; }
        public int AppointmentId { get; set; }
    }
}