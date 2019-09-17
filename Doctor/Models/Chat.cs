using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Doctor.Models;
using System.ComponentModel.DataAnnotations;
namespace Doctor.Models
{  
    public class Chat
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Message Text")]
        public string MessageText { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Sender { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Reciver { get; set; }
        public DateTime time { get; set; }
    }
}