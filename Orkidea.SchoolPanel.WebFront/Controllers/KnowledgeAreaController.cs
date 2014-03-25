using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class KnowledgeAreaController : Controller
    {
        KnowledgeAreaBiz oKnowledgeAreaBiz = new KnowledgeAreaBiz();

        //
        // GET: /KnowledgeArea/
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

            List<KnowledgeArea> lstKnowledgeArea = oKnowledgeAreaBiz.GetKnowledgeAreaList(school);
            return View(lstKnowledgeArea);
        }

        //
        // GET: /KnowledgeArea/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /KnowledgeArea/Create
        [Authorize]
        public ActionResult Create()
        {
            KnowledgeArea knowledgeArea = new KnowledgeArea();
            return View(knowledgeArea);
        }

        //
        // POST: /KnowledgeArea/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(KnowledgeArea knowledgeArea)
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

                knowledgeArea.idColegio = school.id;
                oKnowledgeAreaBiz.SaveKnowledgeArea(knowledgeArea);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /KnowledgeArea/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            KnowledgeArea knowledgeArea = oKnowledgeAreaBiz.GetKnowledgeAreaByKey(new KnowledgeArea() { id = id });
            return View(knowledgeArea);
        }

        //
        // POST: /KnowledgeArea/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, KnowledgeArea knowledgeArea)
        {
            try
            {
                //// TODO: Add insert logic here
                //#region School identification
                //System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                //int idColegio = 0;

                //if (context.IsAuthenticated)
                //{
                //    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                //    string[] userRole = ci.Ticket.UserData.Split('|');
                //    idColegio = int.Parse(userRole[2]);
                //}

                //School school = new School() { id = idColegio };
                //#endregion

                knowledgeArea.id = id;
                oKnowledgeAreaBiz.SaveKnowledgeArea(knowledgeArea);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /KnowledgeArea/Delete/5
        [Authorize]        
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                oKnowledgeAreaBiz.DeleteKnowledgeArea(new KnowledgeArea() { id = id });
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
