using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class EvaluationNoteController : Controller
    {
        EvaluationNoteBiz evaluationNoteBiz = new EvaluationNoteBiz();

        //
        // GET: /EvaluationNote/
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

            List<EvaluationNote> lsEvaluationNote = evaluationNoteBiz.GetEvaluationNoteList(school);
            return View(lsEvaluationNote);
        }

        //
        // GET: /EvaluationNote/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /EvaluationNote/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(EvaluationNote evaluationNote)
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

                evaluationNote.idColegio = school.id;

                evaluationNoteBiz.SaveEvaluationNote(evaluationNote);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EvaluationNote/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            EvaluationNote evaluationNote = evaluationNoteBiz.GetEvaluationNoteByKey(new EvaluationNote() { id = id });
            return View(evaluationNote);
        }

        //
        // POST: /EvaluationNote/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, EvaluationNote evaluationNote)
        {
            try
            {
                // TODO: Add update logic here
                evaluationNote.id = id;
                evaluationNoteBiz.SaveEvaluationNote(evaluationNote);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /EvaluationNote/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                evaluationNoteBiz.DeleteEvaluationNote(new EvaluationNote() { id = id });
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
