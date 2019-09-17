﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Doctor.Models;
using Doctor.ViewModel;

namespace Doctor.Controllers
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Services.Description;

    using Microsoft.Ajax.Utilities;

    using Newtonsoft.Json;

    public class DoctorController : Controller
    {
        private DataContex _contex;

        public DoctorController()
        {
            _contex = new DataContex();
        }

        protected override void Dispose(bool dispossing)
        {
            _contex.Dispose();
        }

        // GET: Doctor
        public ActionResult Index()
        {
            var Doctor = this.GetCurrentDoctor();
            if (Doctor != null)
            {
                return View(GetCurrentDoctor());
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }
            
        }

        public ActionResult New()
        {
            ViewBag.Message = null;
            ViewBag.Status = false;
            var department = this._contex.Departments.ToList();
            var doctorview = new DoctorView { Doctors = new Doctors(), Departments = department, };
            return this.View(doctorview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(
            [Bind(Exclude = "IsEmailVarifide,ActivationCode")]
            Doctors doctors)
        {
            bool Status = false;
            string Message = null;
            var ViewModel = new DoctorView { Doctors = new Doctors(), Departments = _contex.Departments.ToList(), };
            if (!ModelState.IsValid)
            {
                Message = "Invalid Request";
                return this.View("New", ViewModel);
            }
            else
            {
                if (doctors.Id == 0)
                {
                    if (this.IsEmailExist(doctors.doctorEmail))
                    {
                        this.ModelState.AddModelError("EmailExist", "Email Is Already Exist");
                        return this.View("New", ViewModel);
                    }



                    doctors.ActivationCode = Guid.NewGuid();



                    #region Password Hasshiing

                    doctors.doctorPassword = Crypto.Hash(doctors.doctorPassword);
                    doctors.doctorConfirmPassword = Crypto.Hash(doctors.doctorConfirmPassword);

                    #endregion

                    doctors.IsEmailVarifide = false;

                    #region Image file upload

                    string fileName = Path.GetFileName(doctors.doctorImagefile.FileName);
                    doctors.doctorImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);

                    #endregion

                    #region Save Data

                    doctors.doctorImagefile.SaveAs(fileName);
                    this._contex.Doctorses.Add(doctors);
                    this._contex.SaveChanges();

                    #endregion

                    #region Send Email

                    var url = "/Doctor/VarifyAccount/" + doctors.ActivationCode.ToString();
                    var requestUrl = this.Request.Url;
                    if (requestUrl != null)
                    {
                        string link = requestUrl.AbsoluteUri.Replace(requestUrl.PathAndQuery, url);
                        Email.SendVarificationEmail(doctors.doctorEmail, link);
                    }

                    Message = "Registration Successfully done,Check your Email";
                    Status = true;

                    this.ViewBag.Message = Message;
                    this.ViewBag.Status = true;

                    #endregion
                }
                else
                {
                    var doctorInDb = this._contex.Doctorses.Single(d => d.Id == doctors.Id);
                    doctorInDb.doctorName = doctors.doctorName;
                    doctorInDb.doctorBirthDate = doctors.doctorBirthDate;
                    doctorInDb.DepartmentId = doctors.DepartmentId;
                    this._contex.SaveChanges();
                }

                return View("New", ViewModel);
            }

            this.ViewBag.Message = Message;
            this.ViewBag.Status = Status;
            return this.View();
        }

        [HttpGet]
        public ActionResult VarifyAccount(string id)
        {
            bool Status = false;
            this._contex.Configuration.ValidateOnSaveEnabled = false;
            var patient = this._contex.Doctorses.Where(d => d.ActivationCode == new Guid(id)).FirstOrDefault();
            if (patient != null && patient.IsEmailVarifide != true)
            {
                patient.IsEmailVarifide = true;

                // doctor.ActivationCode = null;
                this._contex.SaveChanges();
                Status = true;
                this.ViewBag.Message = "Your account has been successfully activated!";
            }
            else if (patient != null && patient.IsEmailVarifide == true)
            {
                this.ViewBag.Message = "This account already active";
            }
            else
            {
                this.ViewBag.Message = "Invalide request";
            }

            this.ViewBag.Status = Status;
            return View();
        }

        // Check Email Existence
        [NonAction]
        public bool IsEmailExist(string Email)
        {
            var e = this._contex.Doctorses.FirstOrDefault(a => a.doctorEmail == Email);
            return e != null;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            string Meggage = null;
            bool Status = false;
            var doc = this._contex.Doctorses.FirstOrDefault(d => d.doctorEmail == login.Email);
            if (doc != null)
            {
                if (string.Compare(Crypto.Hash(login.Password), doc.doctorPassword) == 0 && doc.IsEmailVarifide == true)
                {
                    Status = true;
                    int timeout = login.Remember ? 505600 : 20;
                    Session["Doctor"] = doc.doctorEmail;
                    var cokie = new HttpCookie("Doctor", doc.doctorEmail);
                    cokie.Expires = DateTime.Now.AddMinutes(timeout);
                    cokie.HttpOnly = true;
                    Response.Cookies.Add(cokie);

                    return RedirectToAction("Index", "Doctor");
                }
                else
                {
                    Meggage = "Account is not verified or Password incorrect ";
                    return this.View();
                }
            }
            else
            {
                Meggage = "This doctor doesn't exist";
                return this.View();
            }

            ViewBag.Message = Meggage;
            ViewBag.Status = Status;
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var Doctor = this.GetCurrentDoctor();
            if (Doctor != null)
            {
                Session.Clear();

                // Session.Abandon();
                if (Request.Cookies["Doctor"] != null)
                {
                    var c = new HttpCookie("Doctor");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }

                return RedirectToAction("Login", "Doctor");
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }           
        }

        public ActionResult Appointment()
        {

            var doctor = this.GetCurrentDoctor();
            if (doctor != null)
            {
                var appointment = this._contex.Appointments.Include("Patient").OrderByDescending(a => a.Id)
                    .Where(a => a.DoctorsId == doctor.Id).Take(10);
                return this.PartialView(appointment);
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }
            
        }

        public ActionResult Chat()
        {
            if (this.GetCurrentDoctor() != null)
            {
                return Redirect("~/Doctor/ChatBox");
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }
            
        }

        public ActionResult Patient()
        {
            List<Patient>Pa=new List<Patient>();
            var doctor = this.GetCurrentDoctor();
            var Patient = this._contex.Appointments.Include("Patient").Where(a => a.DoctorsId == doctor.Id).ToList();
            foreach (var p in Patient)
            {
                Pa.Add(p.Patient);
            }

            var past = Pa.Distinct();
            return this.PartialView(past);
        }

        public ActionResult Profile()
        {
            var doctor = this.GetCurrentDoctor();
            return this.PartialView(doctor);
        }

        public ActionResult Deshboard()
        {
            var doc = this.GetCurrentDoctor();
            var ap = this._contex.Appointments.Where(a => a.DoctorsId == doc.Id && a.Status == false);
            return this.PartialView(ap);
        }

        public ActionResult History()
        {
            var doc = this.GetCurrentDoctor();
            var ap = this._contex.Appointments.Where(a => a.DoctorsId == doc.Id && a.Status == false);
            return this.PartialView(ap);
        }

        public ActionResult Prescription(string id)
        {
            var doc = this.GetCurrentDoctor();
            if (doc != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    try
                    {
                        int appointId = Convert.ToInt32(id);
                        var appointment = this._contex.Appointments.Include("Patient").Include("Doctors")
                            .Single(a => a.Id == appointId);
                        ViewBag.Report = this._contex.Reports.Where(r => r.Id == appointment.PatientId).ToList();
                        var solution = new SloliutionView { Appointment = appointment, Prescription = new Prescription() };
                        return this.View(solution);
                    }
                    catch (Exception e)
                    {                     
                        throw;
                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }
        }

        private Doctors GetCurrentDoctor()
        {
            string dName = this.Request.Cookies["Doctor"].Value;
            return this._contex.Doctorses.Single(d => d.doctorEmail == dName);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMedicine(Medicine medicine, string id)
        {
            if (id != null)
            {
                try
                {
                    int ID = Convert.ToInt32(id);
                    var appointment = this._contex.Appointments.Single(a => a.Id == ID);
                    medicine.AppointmentId = appointment.Id;
                    this._contex.Medicines.Add(medicine);
                    this._contex.SaveChanges();
                    return Json("Success!");
                }
                catch (Exception ex)
                {
                    return Json("Error try again!");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
        //Add new medical test
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(Report report, string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ID = Convert.ToInt32(id);
                var appointment = this._contex.Appointments.Single(a => a.Id == ID);
                report.AppointmentId = appointment.Id;
                if (ModelState.IsValid)
                {
                    try
                    {
                        this._contex.Reports.Add(report);
                        this._contex.SaveChanges();
                        return Json("Success!");
                    }
                    catch (Exception ex)
                    {
                        return Json("Error! try again");
                    }
                }
                else
                {
                    return Json("Input is invalide");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPrescription(Prescription prescription, string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                int ID = Convert.ToInt32(id);
                var appointment = this._contex.Appointments.Single(a => a.Id == ID);
                prescription.AppointmentId = appointment.Id;
                if (ModelState.IsValid)
                {
                    try
                    {
                        this._contex.Prescriptions.Add(prescription);
                        this._contex.SaveChanges();
                        return Json("Success!");
                    }
                    catch (Exception e)
                    {
                        return Json("Error! try again");
                    }
                }
                else
                {
                    return Json("Input is invalide");
                }
            }
        }

        public ActionResult ChatBox()
        {
            var doctor = this.GetCurrentDoctor();
            if (doctor != null)
            {
                List<Patient> patient = new List<Patient>();
                var appointment = this._contex.Appointments.Include("Patient").Where(a => a.DoctorsId == doctor.Id);
                foreach (var ap in appointment)
                {
                    patient.Add(ap.Patient);
                }

                List<Patient> plist = patient.Distinct().ToList();
                var chatView = new ChatViewForDoctor
                                   {
                                       Doctors = doctor,
                                       Patient = plist,
                                       Chat = new Chat()
                                   };
                return this.View(chatView);
            }
            else
            {
                return RedirectToAction("Login", "Doctor");
            }
        }
        [HttpGet]
        public JsonResult GetMeeage(string id)
        {
            var patient = this._contex.Patients.Single(p => p.PatientEmail == id);
            var doctor = this.GetCurrentDoctor();
            var message = JsonConvert.SerializeObject(
                this._contex.Chats.OrderByDescending(c => c.Id).Where(
                    c => c.Sender == doctor.doctorEmail && c.Reciver == patient.PatientEmail
                         || (c.Sender == patient.PatientEmail && c.Reciver == doctor.doctorEmail)).ToList());
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendMessage(string MessageText, string Reciver)
        {
            Chat chat = new Chat();
            var sender = this.GetCurrentDoctor();
            chat.MessageText = MessageText;
            chat.Sender = sender.doctorEmail;
            chat.Reciver = Reciver;
            chat.time = DateTime.Now;
            try
            {
                this._contex.Chats.Add(chat);
                this._contex.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        public ActionResult Solution(string Id)
        {                     
            if (Id != null)
            {
                try
                {
                    int id = Convert.ToInt32(Id);
                    Appointment appointment = this._contex.Appointments.FirstOrDefault(a => a.Id == id);
                    appointment.Status = true;
                    this._contex.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception x)
                {
                    return Json(x);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}