using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cybage.BusReservation.Models;
using Cybage.BusReservation.DAL;
using System.Web.UI.WebControls;

namespace Cybage.BusReservation.Controllers
{
    public class RegistrationController : Controller
    {
        private CybageBusReservationContext db = new CybageBusReservationContext();

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        public ActionResult Login()
        {
            User us = (User)Session["user"];
            if (us == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Book", "Search");
            }

        }

        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public ActionResult LoginPost(UserLogin login)
        {
            
           
            
                bool emailcheck = false;
                bool pwdcheck = false;
                var emails = from u in db.Users
                             select u.EmailId;
                var pwds = from e in db.Users
                           select e.Password;

                var userquery = from u in db.Users
                                where u.EmailId.Equals(login.EmailId) && u.Password.Equals(login.Password)
                                select u;

                User user = new User();
                foreach (var item in userquery)
                {
                    user = item;
                }

                foreach (string email in emails)
                {
                    if (login.EmailId.Equals(email))
                    {
                        emailcheck = true;
                    }
                }
                foreach (string pwd in pwds)
                {
                    if (login.Password.Equals(pwd))
                    {
                        pwdcheck = true;
                    }
                }
                if (emailcheck.Equals(true) && pwdcheck.Equals(true))
                {
                    List<BusRoute> busroutedetails = (List<BusRoute>)Session["bus"];
                    int busroutid = 0;
                    int busid = 0;
                    int seats = 0;

                    foreach (var item in busroutedetails)
                    {
                        busroutid = item.Id;
                        busid = item.Buses.Id;
                        seats = item.Buses.Seats;
                    }
                    Session["user"] = user;



                    DateTime dp = new DateTime();
                    foreach (var d in busroutedetails)
                    {
                        dp = d.DepartureTime;
                    }



                    int uid = user.Id;
                    BookingDetails bookdetails = new BookingDetails();
                    bookdetails.User_Id = uid;
                    bookdetails.BookingDate = System.DateTime.Now;
                    bookdetails.JourneyDate = dp;
                    bookdetails.BusRoute_Id = busroutid;
                    db.BookingDetails.Add(bookdetails);
                    db.SaveChanges();
                    Session["bookdetails"] = bookdetails;
                    var getseatno = from p in db.PassengerDetails
                                    where p.BookingDetail.Busroute.Buses.Id == busid
                                    select p.SeatNumber;

                    List<int> seatarray = new List<int>();

                    foreach (var item in getseatno)
                    {
                        seatarray.Add(item);
                    }

                    Session["seatarray"] = seatarray;

                    if (seatarray.Count() == seats)
                    {
                        TempData["error"] = "not enough seats available";
                        return RedirectToAction("error", "Search");
                    }

                    return RedirectToAction("Book", "Search");

                }
                return View();
            
            
        }
        public ActionResult Register()
        {
            return View();
        }


        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPost([Bind(Include = "EmailId,FirstName,LastName,Password,MobileNo,Dob", Exclude = "UserId,Type")]User user)
        {



            int currentyear = DateTime.Now.Year;
            int birthyear = user.Dob.Year;
            int calculation = currentyear - birthyear;

            if (!(calculation < 18 || calculation > 100))
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("DOB", "your age should be between 18 and 100");
            }
            return View();
        }

        //if (ModelState.IsValid)
        //{

        //    db.Users.Add(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");

        //}
        //return View("Register");

        public JsonResult checkforduplication(string emailid)
        {
            var data = db.Users.Where(p => p.EmailId.Equals(emailid, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (data != null)
            {
                return Json("sorry, EmailId already exists", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }



    }
}