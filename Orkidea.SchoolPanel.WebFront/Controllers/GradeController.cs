using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class GradeController : Controller
    {
        GradeBiz gradeBiz = new GradeBiz();

        //
        // GET: /Grade/
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

            List<Grade> lstGrade = new List<Grade>();

            lstGrade = gradeBiz.GetGradeList(school);
            return View(lstGrade);
        }       

        //
        // GET: /Grade/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Grade/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Grade oGrade)
        {
            try
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

                // TODO: Add insert logic here
                oGrade.idColegio = school.id;
                gradeBiz.SaveGrade(oGrade);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Grade/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Grade oGrade = gradeBiz.GetGradebyKey(new Grade() { id = id });
            return View(oGrade);
        }

        //
        // POST: /Grade/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Grade oGrade)
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
                oGrade.id = id;
                oGrade.idColegio = school.id;
                gradeBiz.SaveGrade(oGrade);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Grade/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                gradeBiz.DeleteGrade(new Grade() { id = id });
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
