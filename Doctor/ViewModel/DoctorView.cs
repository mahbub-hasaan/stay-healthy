using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doctor.ViewModel
{
    using Doctor.Models;

    public class DoctorView
    {
        public IEnumerable<Department> Departments { get; set; }

        public Doctors Doctors { get; set; }
    }
}