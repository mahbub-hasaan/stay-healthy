﻿// ReSharper disable All

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Doctor.Models;
using Doctor.ViewModel;

namespace Doctor.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Net;
    using System.Web.Helpers;
    using System.Web.Security.AntiXss;

    using Microsoft.ApplicationInsights.Web;

    using Newtonsoft.Json;

    using Crypto = Doctor.Crypto;

    public class PatientController : Controller
    {
        //DbContex
        private DataContex _contex;

        public PatientController()
        {
            _contex = new DataContex();
        }

        protected override void Dispose(bool dispossing)
        {
            _contex.Dispose();
        }

        // GET: Patient
        [Authorize]
        public ActionResult Index()
        {
            return View(this.CurrentUser());
        }

        // Crate Account View
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }
        //Crate Account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(
            [Bind(Exclude = "EmailIsVerifed,ActivationCode")]
            Patient patient)
        {
            ModelState["Id"].Errors.Clear();
            string Message = null;
            bool Status = false;
            if (!ModelState.IsValid)
            {
                Message = "Ivalide Request";
                return View();
            }
            else
            {
                if (patient.Id == 0)
                {
                    if (IsEmailExist(patient.PatientEmail))
                    {
                        ModelState.AddModelError("EmailExist", "Email Is Already Exist");
                        return View();
                    }

                    #region Grenatat unique Id

                    patient.ActivationCode = Guid.NewGuid();

                    #endregion

                    #region Password Hasing

                    patient.PatientPassword = Crypto.Hash(patient.PatientPassword);
                    patient.PatientConfPassword = Crypto.Hash(patient.PatientConfPassword);

                    #endregion

                    patient.EmailIsVerifed = false;

                    #region Save Data

                    _contex.Patients.Add(patient);
                    _contex.SaveChanges();

                    #endregion

                    #region Send Email

                    var url = "/Patient/VarifyAccount/" + patient.ActivationCode.ToString();
                    string link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);
                    Email.SendVarificationEmail(patient.PatientEmail, link);
                    Message = "Registration Successfully done,Check your Email";
                    Status = true;

                    #endregion
                }
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View();
        }
        //Check Email existance
        [NonAction]
        public bool IsEmailExist(string Email)
        {
            var e = _contex.Patients.Where(a => a.PatientEmail == Email).FirstOrDefault();
            return e != null;
        }
        //Verify Account
        [HttpGet]
        public ActionResult VarifyAccount(string id)
        {
            bool Status = false;
            _contex.Configuration.ValidateOnSaveEnabled = false;
            var patient = _contex.Patients.Where(d => d.ActivationCode == new Guid(id)).FirstOrDefault();
            if (patient != null && patient.EmailIsVerifed != true)
            {
                patient.EmailIsVerifed = true;

                // doctor.ActivationCode = null;
                _contex.SaveChanges();
                Status = true;
                ViewBag.Message = "Your account has been successfully activated!";
            }
            else if (patient.EmailIsVerifed == true)
            {
                ViewBag.Message = "This account already active";
            }
            else
            {
                ViewBag.Message = "Invalide request";
            }

            ViewBag.Status = Status;
            return View();
        }

        // Login view
        public ActionResult Login()
        {
            return this.View();
        }
        //Login
        [HttpPost]
        public ActionResult Login(Login login, string ReturnUrl)
        {
            bool Status = false;
            string Message = null;
            var patient = _contex.Patients.Where(d => d.PatientEmail == login.Email).FirstOrDefault();
            if (patient != null)
            {
                if (string.Compare(Crypto.Hash(login.Password), patient.PatientPassword) == 0
                    && patient.EmailIsVerifed == true)
                {
                    Status = true;
                    Session["Patient"] = patient.PatientEmail;
                    int TimeOut = login.Remember ? 525600 : 20;
                    var tiket = new FormsAuthenticationTicket(login.Email, login.Remember, TimeOut);
                    string encrypts = FormsAuthentication.Encrypt(tiket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypts);
                    cookie.Expires = DateTime.Now.AddMinutes(TimeOut);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        Status = true;
                        return RedirectToAction("Index", "Patient");
                    }
                }
                else
                {
                    Message = "Password is incorrect or Email is not Verified";
                }
            }
            else
            {
                Message = "This email is not registerd";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View();
        }
        //Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Remove("Patient");
            return RedirectToAction("Index", "Home");
        }

        //View Profile
        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            var email = User.Identity.Name;
            var patient = this._contex.Patients.Where(p => p.PatientEmail == email).FirstOrDefault();
            return this.PartialView("Profile", patient);
        }
        //Update  Profile
        [HttpPost]
        public ActionResult Update(Patient patient)
        {
            var Patien = this._contex.Patients.Single(p => p.Id == patient.Id);
            ModelState["PatientPassword"].Errors.Clear();
            ModelState["PatientConfPassword"].Errors.Clear();
            patient.PatientConfPassword = Patien.PatientPassword;
            patient.PatientPassword = Patien.PatientPassword;
            if (ModelState.IsValid)
            {
                if (patient.Imagefile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(patient.Imagefile.FileName);
                    string Extantion = Path.GetExtension(patient.Imagefile.FileName);
                    fileName = fileName + DateTime.Now.Year + Extantion;
                    patient.ImagePath = "/Image/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Image/"), fileName);
                    patient.Imagefile.SaveAs(fileName);
                }
                else
                {
                    patient.ImagePath = "/Image/user.jpg";
                }
                Patien.PatientName = patient.PatientName;
                Patien.PatientPassword = patient.PatientPassword;
                Patien.PatientConfPassword = patient.PatientConfPassword; ;
                Patien.PatientEmail = patient.PatientEmail;
                Patien.PatientPhone = patient.PatientPhone;
                Patien.BirthDate = patient.BirthDate;
                Patien.ImagePath = patient.ImagePath;
                Patien.BloodGroup = patient.BloodGroup;
                this._contex.SaveChanges();

            }
            return RedirectToAction("Index", "Patient");
        }
        //Deshboard
        [Authorize]
        public ActionResult Deshboard()
        {
            var user = this.CurrentUser();
            ViewBag.Appointment = this._contex.Appointments
                .Where(a => a.Status == false && a.PatientId == user.Id).Count();
            return this.PartialView();
        }
        //Appointment:Select Doctor
        [Authorize]
        [HttpGet]
        public ActionResult Apointment(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                try
                {
                    int doc = Convert.ToInt32(id);
                    var doctor = this._contex.Doctorses.Single(d => d.Id == doc);
                    var appoint = new AppointmentView
                    {
                        Doctorses = doctor,
                        Patient = CurrentUser(),
                        Appointment = new Appointment()
                    };
                    return this.View(appoint);
                }
                catch (Exception)
                {

                    return new HttpStatusCodeResult(HttpStatusCode.NotFound); 
                }
            }
        }
        //Appointment Submite
        [HttpPost]
        public ActionResult Apointment(Appointment appointment,int id,int pId)
        {
            appointment.PatientId = pId;
            appointment.DoctorsId = id;
            appointment.Date = DateTime.Now;
            if (ModelState.IsValid)
            {               
                this._contex.Appointments.Add(appointment);
                this._contex.SaveChanges();
                ViewBag.Status = true;
                ViewBag.Message = "Appointment Complited ";
                return Redirect("~/patient");
            }
            else
            {
                ViewBag.Status = false;
                ViewBag.Message = "Error !! try again";
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        //Appointment:View
        public ActionResult Appointment()
        {
            int id = this.CurrentUser().Id;
            var appointments = this._contex.Appointments.Include("Doctors").OrderByDescending(p => p.Id).Where(a=>a.PatientId==id).Take(10)
                .ToList();
           
            return this.PartialView(appointments);
        }
        //Reports
        [Authorize]
        public ActionResult Reports()
        {
            var p = this.CurrentUser();
            List<Report> Report = new List<Report>();
            var appont = this._contex.Appointments.Where(a => a.PatientId == p.Id).ToList();
            foreach (var app in appont)
            {
                foreach (var r in this._contex.Reports.Where(m => m.AppointmentId == app.Id).ToList())
                {
                    Report.Add(r);
                }
            }
            return this.PartialView(Report);
        }
        //Prescription
        [Authorize]
        public ActionResult Perescription()
        {
            var p = this.CurrentUser();
            List<Medicine> Medicine=new List<Medicine>();
            List<Prescription> Press=new List<Prescription>();
            List<Report> Report=new List<Report>();
            var appont = this._contex.Appointments.Where(a => a.PatientId == p.Id).ToList();
            foreach (var app in appont)
            {
                foreach (var m in this._contex.Medicines.Where(m => m.AppointmentId == app.Id).ToList())
                {
                    Medicine.Add(m);
                }
                foreach (var r in this._contex.Reports.Where(m => m.AppointmentId == app.Id).ToList())
                {
                    Report.Add(r);
                }
                foreach (var pr in this._contex.Prescriptions.Where(m => m.AppointmentId == app.Id).ToList())
                {
                    Press.Add(pr);
                }
            }
            var PresView=new PrescriptonView
                             {
                                Reports = Report,
                                Medicines = Medicine,
                                Prescriptions = Press
                             };
            return this.PartialView(PresView);
        }

        public ActionResult MedicineChart()
        {
            ArrayList xvalue = new ArrayList();
            ArrayList yvalue = new ArrayList();
            var p = this.CurrentUser();
            var appoint = this._contex.Appointments.Where(a => a.PatientId == p.Id).ToList();
            var med = this._contex.Medicines.ToList();
            var appoi = appoint.Distinct();
            foreach (var ap in appoi)
            {
                xvalue.Add(ap.Id);
                var medici = med.GroupBy(m => m.AppointmentId == ap.Id)
                    .Select(g => new { name = g.Key, count = g.Count() });
                yvalue.Add(medici.Count());
            }                      
            new Chart(width: 500, height: 300, theme: ChartTheme.Blue).AddTitle("Medicine Per Appointment").AddSeries(
                "Default",
                chartType: "Column",
                xValue: xvalue,
                yValues: yvalue).Write("png");
            return null;
        }
        //History
        [Authorize]
        public ActionResult History()
        {
            return Redirect("~/Patient/ChatBox");
        }
        //Search Docotr
        [Authorize]
        [HttpPost]
        public JsonResult SearchDoctor(string search)
        {
            var allserch = (from d in this._contex.Doctorses
                            where d.doctorName.Contains(search)
                            select new { d.doctorName });
            return new JsonResult { Data = allserch, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [NonAction]
        public Patient CurrentUser()
        {
            var id = User.Identity.Name;
            return this._contex.Patients.Single(p => p.PatientEmail == id);
        }
        [Authorize]
        public ActionResult ChatBox()
        {
            var patient = this.CurrentUser();
            List<Doctors> Doctor = new List<Doctors>();
            var appointment = this._contex.Appointments.Include("Doctors").Where(a => a.PatientId == patient.Id);
            foreach (var ap in appointment)
            {
                Doctor.Add(ap.Doctors);
            }

            List<Doctors> dlist = Doctor.Distinct().ToList();
            var chatView = new ChatViewForPatient
                               {
                                   Doctors = dlist,
                                   Patient = patient,
                                   Chat = new Chat()
                               };
            return this.View(chatView);
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetMessage(string id)
        {
            var doctor = this._contex.Doctorses.Single(p => p.doctorEmail == id);
            var patient = CurrentUser();
            var message = JsonConvert.SerializeObject(this._contex.Chats.OrderByDescending(c => c.Id).Where(c=>c.Sender==patient.PatientEmail && c.Reciver==doctor.doctorEmail ||(c.Reciver==patient.PatientEmail && c.Sender==doctor.doctorEmail)).ToList());
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [HttpPost]
        public ActionResult SendMessage(string MessageText, string Reciver)
        {
            Chat chat = new Chat();
            var sender = this.CurrentUser();
            chat.MessageText = MessageText;
            chat.Sender = sender.PatientEmail;
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

        public ActionResult ReportSubmit(string id)
        {
            int Id = Convert.ToInt32(id);
            var report = this._contex.Reports.FirstOrDefault(r => r.Id == Id);
            return this.View(report);
        }
    }
}