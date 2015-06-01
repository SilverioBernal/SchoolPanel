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
    public class AsignatureController : Controller
    {
        AsignatureBiz asignatureBiz = new AsignatureBiz();
        //
        // GET: /Asignature/
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

            KnowledgeAreaBiz knowledgeAreaBiz = new KnowledgeAreaBiz();
            GradeBiz gradeBiz = new GradeBiz();

            List<Asignature> lsAsignature = asignatureBiz.GetAsignatureList(school);
            List<KnowledgeArea> lsKnowledgeArea = knowledgeAreaBiz.GetKnowledgeAreaList(school);
            List<Grade> lsGrade = gradeBiz.GetGradeList(school);

            List<vmAsignature> lsVmAsignature = new List<vmAsignature>();

            foreach (Asignature item in lsAsignature)
            {
                vmAsignature newVmAsignature = new vmAsignature()
                {
                    id = item.id,
                    Descripcion = item.Descripcion,
                    intensidadHoraria = item.intensidadHoraria
                };

                newVmAsignature.desAreaConocimiento = lsKnowledgeArea.Where(c => c.id.Equals(item.idAreaConocimiento)).Select(c => c.descripcion).First();
                newVmAsignature.desGrado = lsGrade.Where(c => c.id.Equals(item.idGrado)).Select(c => c.NombreGrado).First();

                lsVmAsignature.Add(newVmAsignature);
            }
            return View(lsVmAsignature);
        }

        //
        // GET: /Asignature/Create
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

            KnowledgeAreaBiz knowledgeAreaBiz = new KnowledgeAreaBiz();
            GradeBiz gradeBiz = new GradeBiz();

            vmAsignature newVmAsignature = new vmAsignature();

            newVmAsignature.lsGrade = gradeBiz.GetGradeList(school);
            newVmAsignature.lsKnowledgeArea = knowledgeAreaBiz.GetKnowledgeAreaList(school);

            return View(newVmAsignature);
        }

        //
        // POST: /Asignature/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Asignature asignature)
        {
            try
            {
                // TODO: Add insert logic here

                asignatureBiz.SaveAsignature(asignature);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Asignature/Edit/5
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

            KnowledgeAreaBiz knowledgeAreaBiz = new KnowledgeAreaBiz();
            GradeBiz gradeBiz = new GradeBiz();

            Asignature asignature = asignatureBiz.GetAsignaturebyKey(new Asignature() { id = id });
            vmAsignature currentVmAsignature = new vmAsignature()
            {
                idAreaConocimiento = asignature.idAreaConocimiento,
                idGrado = asignature.idGrado,
                intensidadHoraria = asignature.intensidadHoraria,
                Descripcion = asignature.Descripcion,
                ignorarEnPromedio = asignature.ignorarEnPromedio == null ? false : (bool)asignature.ignorarEnPromedio,
                pesoPorcentualAreaConocimiento = asignature.pesoPorcentualAreaConocimiento
            };

            currentVmAsignature.lsGrade = gradeBiz.GetGradeList(school);
            currentVmAsignature.lsKnowledgeArea = knowledgeAreaBiz.GetKnowledgeAreaList(school);

            return View(currentVmAsignature);
        }

        //
        // POST: /Asignature/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Asignature asignature)
        {
            try
            {
                // TODO: Add update logic here
                asignature.id = id;
                asignatureBiz.SaveAsignature(asignature);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Asignature/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            string Res = "";
            try
            {
                asignatureBiz.DeleteAsignature(new Asignature() { id = id });
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
