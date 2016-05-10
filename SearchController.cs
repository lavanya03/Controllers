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

namespace Cybage.BusReservation.Controllers
{
    public class SearchController : Controller
    {
        private CybageBusReservationContext db = new CybageBusReservationContext();
		ViewModel model = new ViewModel();
		static int check;

        // GET: /Search/
		public ActionResult Index(string source, string destination, string category,DateTime? doj)
        {

            Session["source"] = source;
            Session["destination"] = destination;

            var busroutes = db.BusRoutes.Include(b => b.Buses).Include(b => b.Routes);

			var sourcelist = new List<string>();
			var destlist = new List<string>();
			var catlist = new List<string>();

			var sourcequery = from t1 in db.BusStations
							  orderby t1.Name
							  select t1.Name;
			sourcelist.AddRange(sourcequery.Distinct());
			ViewBag.source = new SelectList(sourcelist);

			var destquery = from t1 in db.Routes
							orderby t1.BusStationEnd.Name
							select t1.BusStationEnd.Name;

			destlist.AddRange(destquery.Distinct());
			ViewBag.destination = new SelectList(destlist);


			var catquery = from t1 in db.BusTypes
						   orderby t1.Name
						   select t1.Name;
			catlist.AddRange(catquery.Distinct());
			ViewBag.category = new SelectList(catlist);

			var busdetails = from t in db.BusRoutes
							 select t;
		

			
			//string dateofjourney;
			
			//if (doj != null)
			//{
			//	string s = DateOfJourney.ToString();
			//	dateofjourney = s.Substring(0, 10);
			//}




			if (!String.IsNullOrEmpty(source) && !String.IsNullOrEmpty(destination) && !String.IsNullOrEmpty(category) && !String.IsNullOrEmpty(doj.ToString()))
			{			
				bool result=false;
				foreach (var item in busdetails)
				{
					if (item.DepartureTime.Date.Equals(doj))
					{
						result = true;
						check = 1;
						
						break;
					}
				}
				if (result)
				{
					busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
						&& s.Routes.BusStationEnd.Name.Contains(destination)
						&& s.Buses.BusType.Name.CompareTo(category) == 0
						&& s.Bus_Id == s.Buses.Id
						&& s.Route_Id == s.Routes.Id
						&& s.Buses.BusType_Id == s.Buses.BusType.Id);
				}
				
			}
			else if (!String.IsNullOrEmpty(source) && !String.IsNullOrEmpty(destination) && !String.IsNullOrEmpty(category))
			{
				busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
					&& s.Routes.BusStationEnd.Name.Contains(destination)
					&& s.Buses.BusType.Name.CompareTo(category)==0 
					&& s.Bus_Id == s.Buses.Id 
					&& s.Route_Id == s.Routes.Id 
					&& s.Buses.BusType_Id == s.Buses.BusType.Id);


			}
			else if (!String.IsNullOrEmpty(source) && !String.IsNullOrEmpty(destination))
			{
				busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
					&& s.Routes.BusStationEnd.Name.Contains(destination)
					&& s.Bus_Id == s.Buses.Id
					&& s.Route_Id == s.Routes.Id
					&& s.Buses.BusType_Id == s.Buses.BusType.Id);
			}
			else if (!String.IsNullOrEmpty(source) && !String.IsNullOrEmpty(category))
			{
				busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
					&& s.Buses.BusType.Name.CompareTo(category)==0
					&& s.Bus_Id == s.Buses.Id
					&& s.Route_Id == s.Routes.Id
					&& s.Buses.BusType_Id == s.Buses.BusType.Id);
			}
			else if (!String.IsNullOrEmpty(source))
			{
				busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
					&& s.Bus_Id == s.Buses.Id
					&& s.Route_Id == s.Routes.Id
					&& s.Buses.BusType_Id == s.Buses.BusType.Id);
			}
			
			

			return View(busdetails);
           // return View(busroutes.ToList());
        }

        // GET: /Search/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusRoute busroute = db.BusRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            return View(busroute);
        }
        public ActionResult ReturnTicket()
        {
            string destination = Session["source"] as string;
            string source = Session["destination"] as string;
            var catlist = new List<string>();
            var busdetails = from t in db.BusRoutes
                             select t;
            var catquery = from t1 in db.BusTypes
                           orderby t1.Name
                           select t1.Name;
            catlist.AddRange(catquery.Distinct());
            ViewBag.category = new SelectList(catlist);

            busdetails = busdetails.Where(s => s.Routes.BusStationStart.Name.Contains(source)
                         && s.Routes.BusStationEnd.Name.Contains(destination)
                         && s.Bus_Id == s.Buses.Id
                         && s.Route_Id == s.Routes.Id
                         && s.Buses.BusType_Id == s.Buses.BusType.Id);
            Session["busdetails"] = busdetails.ToList();
            return View(busdetails.ToList());
        }


        // GET: /Search/Delete/5
        public ActionResult ReturnDetails(int id)
        {

            var items = from d in db.BusRoutes
                        select d;

            model.Id = id;
            items = items.Where(s => s.Id == model.Id);
            model.busRoutes.AddRange(Session["bus"] as List<BusRoute>);
            Session["bus"] = items.ToList();
            model.busRoutes.AddRange(Session["bus"] as List<BusRoute>);
            return View(model.busRoutes);

        }
        //public ActionResult Book(int id)
        //{
           
        //    var items = from d in db.BusRoutes
        //                select d;

        //    model.Id = id;
        //    items = items.Where(s => s.Id == model.Id);
        //    if (Session["bus"] == null)
        //    {
        //        Session["bus"] = items.ToList();
        //    }

        //    model.busRoutes.AddRange(Session["bus"] as List<BusRoute>);

        //    DateTime dp = new DateTime();
        //    foreach (var d in items)
        //    {
        //        dp = d.DepartureTime;
        //    }


        //    //DateTime dt = (DateTime)Session["doj"];
        //    BookingDetails bookdetails = new BookingDetails();
        //    bookdetails.User_Id = 3;
        //    bookdetails.BookingDate = System.DateTime.Now;
        //    bookdetails.JourneyDate = dp;
        //    bookdetails.BusRoute_Id = id;
        //    db.BookingDetails.Add(bookdetails);
        //    db.SaveChanges();
        //    Session["bookdetails"] = bookdetails;

        //    //Session["bus"] = items;

        //    return View(model.busRoutes);

        //}

        // GET: /Search/Create
        public ActionResult Create()
        {
            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "BusName");
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id");
            return View();
        }

        // POST: /Search/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Route_Id,Bus_Id,DepartureTime,ArrivalTime")] BusRoute busroute)
        {
            if (ModelState.IsValid)
            {
                db.BusRoutes.Add(busroute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "BusName", busroute.Bus_Id);
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id", busroute.Route_Id);
            return View(busroute);
        }

        public ActionResult DisplayDetails(int id)
        {
            var items = from d in db.BusRoutes
                        select d;

            model.Id = id;
            items = items.Where(s => s.Id == model.Id);

            Session["bus"] = items.ToList();

            model.busRoutes.AddRange(Session["bus"] as List<BusRoute>);
            return View(model.busRoutes);
        }
		
		public ActionResult Book()
        {
          
            model.busRoutes.AddRange(Session["bus"] as List<BusRoute>);

			DateTime dp= new DateTime();
			foreach (var d in model.busRoutes)
			{
				dp = d.DepartureTime;
			}

            List<BusRoute> busroutedetails = (List<BusRoute>)Session["bus"];
            int busroutid=0;
            foreach(var item in busroutedetails)
            {
            busroutid=item.Id;
            }
			User user=(User)Session["user"];
            int uid = user.Id;
			BookingDetails bookdetails = new BookingDetails();
			bookdetails.User_Id = uid;
			bookdetails.BookingDate = System.DateTime.Now;
			bookdetails.JourneyDate = dp;
			bookdetails.BusRoute_Id = busroutid;
			db.BookingDetails.Add(bookdetails);
			db.SaveChanges();
			Session["bookdetails"] = bookdetails;
			
			
            //var getseatno = from p in db.PassengerDetails
            //                where p.BookingDetail.Busroute.Buses.Id == bookdetails.Busroute.Buses.Id
            //                select p.SeatNumber;

            //List<int> seatarray = new List<int>();
			
            //foreach (var item in getseatno)
            //{
            //    seatarray.Add(item);
            //}

            //Session["seatarray"] = seatarray;

            //if (seatarray.Count() == bookdetails.Busroute.Buses.Seats)
            //{
            //    TempData["error"] = "not enough seats available";
            //    return RedirectToAction("error");
            //}


			return View(model.busRoutes);

		}


		public ActionResult error()
		{
			return View();
		}





        // GET: /Search/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusRoute busroute = db.BusRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "BusName", busroute.Bus_Id);
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id", busroute.Route_Id);
            return View(busroute);
        }

        // POST: /Search/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Route_Id,Bus_Id,DepartureTime,ArrivalTime")] BusRoute busroute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busroute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "BusName", busroute.Bus_Id);
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id", busroute.Route_Id);
            return View(busroute);
        }

        // GET: /Search/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusRoute busroute = db.BusRoutes.Find(id);
            if (busroute == null)
            {
                return HttpNotFound();
            }
            return View(busroute);
        }

        // POST: /Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusRoute busroute = db.BusRoutes.Find(id);
            db.BusRoutes.Remove(busroute);
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
