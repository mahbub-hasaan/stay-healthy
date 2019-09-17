// ReSharper disable All
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Doctor.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Enter Name")]
        public string PatientName { get; set; }


        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string PatientEmail { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PatientPhone { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date Of Birth")]
        public DateTime? BirthDate { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PatientPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password minimum 6 cherecter requird")]
        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Required]
        [Compare("PatientPassword",ErrorMessage = "Password dosn't match")]
        public string PatientConfPassword { get; set; }

        public bool EmailIsVerifed { get; set; }

        public Guid ActivationCode { get; set; }
        [Display(Name = "Upload Image")]
        public string ImagePath { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Upload Image")]
        [NotMapped]
        public HttpPostedFileBase Imagefile { get; set; }
    }
}