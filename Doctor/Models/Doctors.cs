using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Doctor.Models
{
    public class Doctors
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [Display(Name = "Enter Doctor Name")]
        public string doctorName { get; set; }


        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date Of Birth")]
        public DateTime doctorBirthDate { get; set; }
        

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string doctorEmail { get; set; }

        [Display(Name = "Upload Image")]
        public string doctorImagePath { get; set; }


        [NotMapped]
        [Required]
        public HttpPostedFileBase doctorImagefile { get; set; }


        [Required(AllowEmptyStrings = false)]
        [StringLength(200)]
        [Display(Name = "Details")]
        public string doctorDetails { get; set; }
       

        [Display(Name = "Degree")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(200)]
        public string doctorDegree { get; set; }


        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [StringLength(200)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password minimum 6 character required")]
        public string doctorPassword { get; set; }


        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password minimum 6 character required")]
        [Compare("doctorPassword", ErrorMessage = "Password Doesn't Match")]
        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Required]
        public string doctorConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Registration No")]
        public string regNo { get; set; }
        public bool IsEmailVarifide { get; set; }
        public Guid ActivationCode { get; set; }
        


        public Department Department { get; set; }
        [Display(Name = "Department")]
        [Required]public int DepartmentId { get; set; }
    }
}