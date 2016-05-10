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
    public class ManageBusController : Controller
    {
        private CybageBusReservationContext db = new CybageBusReservationContext();

        // GET: /ManageBus/
        public ActionResult Index()
        {
			var busroutes = db.BusRoutes.Include(b => b.Buses).Include(b => b.Routes);
            return View(busroutes.ToList());
        }

		public ActionResult TypeIndex()
		{
			return View(db.BusTypes.ToList());
		}


		public ActionResult BusStationIndex()
		{
			return View(db.BusStations.ToList());
		}

		public ActionResult RouteIndex()
		{
			var routes = db.Routes.Include(b => b.BusStationStart).Include(b => b.BusStationEnd);
			return View(routes.ToList());
			
		}


		public ActionResult BusIndex()
		{
			var buses = db.Buses.Include(b => b.BusType);
			return View(buses.ToList());
		}

		public ActionResult AgeIndex()
		{
			
			return View(db.Ages.ToList());
		}

		public ActionResult BusRateIndex()
		{
			var busrate = db.BusRates.Include(b => b.Bustype).Include(b => b.Age);
			return View(busrate.ToList());
		}

        // GET: /ManageBus/Details/5
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

        // GET: /ManageBus/Create
        public ActionResult Create()
        {
            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "Id");
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id");
            return View();
        }


		public ActionResult TypeCreate()
		{
			return View();
		}

		public ActionResult BusStationCreate()
		{
			return View();
		}

		public ActionResult RouteCreate()
		{
			ViewBag.BusStart_Id = new SelectList(db.BusStations, "Id", "Id");
			ViewBag.BusEnd_Id= new SelectList(db.BusStations, "Id", "Id");
			return View();
		}

		public ActionResult BusCreate()
		{
			ViewBag.BusType_Id = new SelectList(db.BusTypes, "Id", "Id");
			
			return View();
		}

		public ActionResult AgeCreate()
		{
			return View();
		}

		public ActionResult BusRateCreate()
		{
			ViewBag.BusType_Id = new SelectList(db.BusTypes, "Id", "Id");
			ViewBag.Age_Id = new SelectList(db.Ages, "Id", "Id");
			return View();
		}



        // POST: /ManageBus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,Route_Id,Bus_Id,DepartureTime,ArrivalTime")] BusRoute busroute)
        {
            if (ModelState.IsValid)
            {
                db.BusRoutes.Add(busroute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Bus_Id = new SelectList(db.Buses, "Id", "Id", busroute.Bus_Id);
            ViewBag.Route_Id = new SelectList(db.Routes, "Id", "Id", busroute.Route_Id);
            return View(busroute);
        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult TypeCreate([Bind(Include = "Name")] BusType bustype)
		{
			if (ModelState.IsValid)
			{
				db.BusTypes.Add(bustype);
				db.SaveChanges();
				return RedirectToAction("TypeIndex");
			}

			return View(bustype);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult BusStationCreate([Bind(Include = "Name")] BusStation busstation)
		{
			if (ModelState.IsValid)
			{
				db.BusStations.Add(busstation);
				db.SaveChanges();
				return RedirectToAction("BusStationIndex");
			}

			return View(busstation);
		}


		//route 
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RouteCreate([Bind(Include = "Id,BusStart_Id,BusEnd_Id,Distance")] Route route)
		{
			if (ModelState.IsValid)
			{
				db.Routes.Add(route);
				db.SaveChanges();
				return RedirectToAction("RouteIndex");
			}

			ViewBag.BusStart_Id = new SelectList(db.BusStations, "Id", "Id", route.BusStart_Id);
			ViewBag.BusEnd_Id = new SelectList(db.BusStations, "Id", "Id", route.BusEnd_Id);
			return View(route);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult BusCreate([Bind(Include = "Id,BusName,Number,Seats,BusType_Id")] Bus bus)
		{
			if (ModelState.IsValid)
			{
				db.Buses.Add(bus);
				db.SaveChanges();
				return RedirectToAction("BusIndex");
			}

			ViewBag.BusType_Id = new SelectList(db.BusTypes, "Id", "Id", bus.BusType_Id);
			
			return View(bus);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AgeCreate([Bind(Include = "Criteria")] Age age)
		{
			if (ModelState.IsValid)
			{
				db.Ages.Add(age);
				db.SaveChanges();
				return RedirectToAction("AgeIndex");
			}

			return View(age);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult BusRateCreate([Bind(Include = "BusType_Id,Age_Id,Rate")] BusRate busrate)
		{
			if (ModelState.IsValid)
			{
				db.BusRates.Add(busrate);
				db.SaveChanges();
				return RedirectToAction("BusRateIndex");
			}

			return View(busrate);
		}

        // GET: /ManageBus/Edit/5
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

        // POST: /ManageBus/Edit/5
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

        // GET: /ManageBus/Delete/5
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

        // POST: /ManageBus/Delete/5
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
