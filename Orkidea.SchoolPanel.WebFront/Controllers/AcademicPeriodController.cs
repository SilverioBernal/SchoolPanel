using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class AcademicPeriodController : Controller
    {
        AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
        //
        // GET: /AcademicPeriod/

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

            List<AcademicPeriod> lstAcademicPeriod = new List<AcademicPeriod>();

            lstAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);
            return View(lstAcademicPeriod);
        }

        //
        // GET: /AcademicPeriod/Details/5
        [Authorize]
        public ActionResult Details(int id)
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

            AcademicPeriod oAcademicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = id, idColegio = school.id });
            return View(oAcademicPeriod);
        }

        //
        // GET: /AcademicPeriod/Create
        [Authorize]
        public ActionResult Create()
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

            AcademicPeriod oAcademicPeriod = new AcademicPeriod() { idColegio = school.id };

            return View(oAcademicPeriod);
        }

        //
        // POST: /AcademicPeriod/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(AcademicPeriod academicPeriod)
        {
            try
            {
                // TODO: Add insert logic here
                academicPeriodBiz.SaveAcademicPeriod(academicPeriod);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /AcademicPeriod/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
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

            AcademicPeriod oAcademicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = id, idColegio = school.id });

            return View(oAcademicPeriod);
        }

        //
        // POST: /AcademicPeriod/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, AcademicPeriod academicPeriod)
        {
            try
            {
                // TODO: Add update logic here
                academicPeriodBiz.SaveAcademicPeriod(academicPeriod);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /AcademicPeriod/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                AcademicPeriod oAcademicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = id });
                academicPeriodBiz.DeleteAcademicPeriod(oAcademicPeriod);
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
