using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class ValuationLevelController : Controller
    {
        //
        // GET: /ValuationLevel/
        ValuationLevelBiz valuationLevelBiz = new ValuationLevelBiz();
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

            NewsAttachmentBiz newsAttachmentBiz = new NewsAttachmentBiz();

            List<ValuationLevel> lsValuationLevel = valuationLevelBiz.GetValuationLevelList(school);            

            return View(lsValuationLevel);
        }

        //
        // GET: /ValuationLevel/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ValuationLevel/Create

        [HttpPost]
        public ActionResult Create(ValuationLevel valuationLevel)
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

                valuationLevel.idColegio = school.id;
                valuationLevelBiz.SaveValuationLevel(valuationLevel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ValuationLevel/Edit/5

        public ActionResult Edit(int id)
        {
            ValuationLevel valuationLevel = valuationLevelBiz.GetValuationLevelByKey(new ValuationLevel() { id = id });

            return View(valuationLevel);
        }

        //
        // POST: /ValuationLevel/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, ValuationLevel valuationLevel)
        {
            try
            {
                // TODO: Add update logic here
                valuationLevel.id = id;
                valuationLevelBiz.SaveValuationLevel(valuationLevel);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ValuationLevel/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                valuationLevelBiz.DeleteValuationLevel(new ValuationLevel() { id = id });                
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
