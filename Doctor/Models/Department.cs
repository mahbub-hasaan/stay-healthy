using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Doctor.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        public string deptName { get; set; }
       
        public string deptDetail { get; set; }
    }
}