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
using System.Data.SqlClient;



namespace Cybage.BusReservation.Controllers
{
    public class BookController : Controller
    {
        private CybageBusReservationContext db = new CybageBusReservationContext();
		
        // GET: /Book/
		public ActionResult Index(string[] passengername, int[] age, string[] gender, int[] seatnumber)
        {
            List<BusRoute> busroutedetails = (List<BusRoute>)Session["bus"];
            int busroutid = 0;
            int busid = 0;
            int seats = 0;
            int bustypeid = 0;
            int dist = 0;

            foreach (var item in busroutedetails)
            {
                busroutid = item.Id;
                busid = item.Buses.Id;
                seats = item.Buses.Seats;
                bustypeid = item.Buses.BusType_Id;
                dist = item.Routes.Distance;
            }


            var passengerdetails = db.PassengerDetails.Include(p => p.BookingDetail).Include(p => p.BusRate);
			int length = age.Length;
			double totalfare=0.0;
			BookingDetails bd=(BookingDetails)Session["bookdetails"];
			 int availableSeats=0;
		      int occupiedSeats=0;

			  List<int> seatarray =(List<int>)Session["seatarray"];

			  List<int> duplicateseat = new List<int>();
			  for (int i = 0; i < seatarray.Count(); i++)
			  {
				  if (seatarray.Contains(seatnumber[i]))
				  {
					  duplicateseat.Add(seatnumber[i]);
					 
				  }
			  }
			  if (seatarray.Count() != 0)
			  {
				  if (duplicateseat.Count() != 0)
				  {
					  TempData["error"] = duplicateseat;
					  return RedirectToAction("seatduplicationerror");
				  }
			  }

			//bus capacity
			int totalSeats = seats;

			//linq query to get total no. of occupied seats of a particular bus
			var getseatcount = from sc in db.PassengerDetails
							   where sc.BookingDetail.Busroute.Buses.Id==busid
							   group sc by sc.BookingDetail.Busroute.Buses.Id into mygroup
							   select mygroup.Count();


			foreach (var item in getseatcount)
			{
				occupiedSeats = item;
			}

			availableSeats = totalSeats - occupiedSeats;

            User user = (User)Session["user"];

			//fetching current user 

            //int userid = user.Id;
            //var userquery = from uid in db.Users
            //                select uid;
            //userquery=userquery.Where(u=>u.Id==userid);

            //User udetails = new User();

            //foreach (var item in userquery)
            //{
            //    udetails = item;
            //}

            //Session["udetails"] = udetails;
			
			//all current session passenger details list
			List<PassengerDetails> pdlist = new List<PassengerDetails>();
			
			for (int i = 0; i < length; i++)
			{
				if (age[i] != 0)
				{
					PassengerDetails pd = new PassengerDetails();

					if (totalSeats > occupiedSeats)
					{
						availableSeats = totalSeats - occupiedSeats;
						occupiedSeats++;
						
						

						int ageid;
						int getage = age[i];
						if (getage >= 16)
						{
							ageid = 3;
						}
						else if (getage >= 5 && getage < 16)
						{
							ageid = 2;
						}
						else
						{
							ageid = 1;
						}



						var getbusrateid = from br in db.BusRates
										   select br;

						//int bustypeid = bd.Busroute.Buses.BusType.Id;

						getbusrateid = getbusrateid.Where(s => s.Age_Id == ageid && s.BusType_Id == bustypeid);
						int gbt = 0;
						double rate = 0.0;
						//int dist = bd.Busroute.Routes.Distance;



						foreach (var it in getbusrateid)
						{
							gbt = it.Id;
							rate = it.Rate;
						}

						double price = dist * rate;
						totalfare = totalfare + price;

						pd.BookingDetails_Id = bd.Id;
						pd.Name = passengername[i];
						pd.Age = age[i];
						pd.Gender = gender[i];
						pd.SeatNumber = seatnumber[i];
						pd.BusRate_Id = gbt;
						pd.Price = price;
						db.PassengerDetails.Add(pd);
						db.SaveChanges();
						pdlist.Add(pd);


					}
					else
					{
						if (seatnumber.Length > availableSeats && availableSeats!=0)
						{
							db.ChangeTracker.Entries().ToList().ForEach(entry => entry.State = System.Data.Entity.EntityState.Detached);
							var maxid = db.PassengerDetails.Max(d => d.BookingDetails_Id);
							var deletequery = from m in db.PassengerDetails
											  where m.BookingDetails_Id == maxid
											  select m;

							foreach (var detail in deletequery)
							{
								db.PassengerDetails.Remove(detail);
								db.SaveChanges();
							}
						}

						TempData["error"] = "not enough seats available";
					//	ViewData["error"] = TempData["error"];
						return RedirectToAction("error");
					}
				}
				else
				{
					break;
				}
			}

			
			Session["pdlist"] = pdlist;
			Session["totalfare"] = totalfare;
				return View(passengerdetails.ToList());
        }

		public ActionResult error()
		{
			return View();
		}


		public ActionResult seatduplicationerror()
		{
			return View();
		}


		public ActionResult confirmbooking()
		{
			return View();
		}

        // GET: /Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassengerDetails passengerdetails = db.PassengerDetails.Find(id);
            if (passengerdetails == null)
            {
                return HttpNotFound();
            }
            return View(passengerdetails);
        }

        // GET: /Book/Create
        public ActionResult Create()
        {
            ViewBag.BookingDetails_Id = new SelectList(db.BookingDetails, "Id", "Id");
            ViewBag.BusRate_Id = new SelectList(db.BusRates, "Id", "Id");
            return View();
        }

        // POST: /Book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name,Age,Gender,SeatNumber,Price,BookingDetails_Id,BusRate_Id")] PassengerDetails passengerdetails)
        {
            if (ModelState.IsValid)
            {
                db.PassengerDetails.Add(passengerdetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookingDetails_Id = new SelectList(db.BookingDetails, "Id", "Id", passengerdetails.BookingDetails_Id);
            ViewBag.BusRate_Id = new SelectList(db.BusRates, "Id", "Id", passengerdetails.BusRate_Id);
            return View(passengerdetails);
        }

        // GET: /Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassengerDetails passengerdetails = db.PassengerDetails.Find(id);
            if (passengerdetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.BookingDetails_Id = new SelectList(db.BookingDetails, "Id", "Id", passengerdetails.BookingDetails_Id);
            ViewBag.BusRate_Id = new SelectList(db.BusRates, "Id", "Id", passengerdetails.BusRate_Id);
            return View(passengerdetails);
        }

        // POST: /Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Age,Gender,SeatNumber,Price,BookingDetails_Id,BusRate_Id")] PassengerDetails passengerdetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(passengerdetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BookingDetails_Id = new SelectList(db.BookingDetails, "Id", "Id", passengerdetails.BookingDetails_Id);
            ViewBag.BusRate_Id = new SelectList(db.BusRates, "Id", "Id", passengerdetails.BusRate_Id);
            return View(passengerdetails);
        }

        // GET: /Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PassengerDetails passengerdetails = db.PassengerDetails.Find(id);
            if (passengerdetails == null)
            {
                return HttpNotFound();
            }
            return View(passengerdetails);
        }

        // POST: /Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PassengerDetails passengerdetails = db.PassengerDetails.Find(id);
            db.PassengerDetails.Remove(passengerdetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
