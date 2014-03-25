using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class PlaceController : Controller
    {

        PlaceBiz placeBiz = new PlaceBiz();
        //
        // GET: /Place/
        [Authorize]
        public ActionResult Index()
        {
            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            List<Place> lstPlace = placeBiz.GetPlaceList(school);
            return View(lstPlace);
        }        

        //
        // GET: /Place/Create
        [Authorize]
        public ActionResult Create()
        {
            Place place = new Place();
            return View(place);
        }

        //
        // POST: /Place/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Place place)
        {
            try
            {
                // TODO: Add insert logic here
                #region School identification
                System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                int idColegio = 0;

                if (context.IsAuthenticated)
                {
                    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                    string[] userRole = ci.Ticket.UserData.Split('|');
                    idColegio = int.Parse(userRole[2]);
                }

                School school = new School() { id = idColegio };
                #endregion

                place.idColegio = school.id;
                placeBiz.SavePlace(place);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Place/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Place place = placeBiz.GetPlaceByKey(new Place() { id = id });
            return View(place);
        }

        //
        // POST: /Place/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Place place)
        {
            try
            {
                // TODO: Add update logic here
                
                #region School identification
                System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                int idColegio = 0;

                if (context.IsAuthenticated)
                {
                    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                    string[] userRole = ci.Ticket.UserData.Split('|');
                    idColegio = int.Parse(userRole[2]);
                }

                School school = new School() { id = idColegio };
                #endregion

                place.id = id;

                placeBiz.SavePlace(place);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Place/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                Place place = placeBiz.GetPlaceByKey(new Place() { id = id });
                placeBiz.DeletePlace(place);
                
                Res = "OK";
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }

            return Json(Res, JsonRequestBehavior.AllowGet);            
        }       
    }
}
