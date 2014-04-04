using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class StudentLogController : Controller
    {
        //
        // GET: /StudentLog/

        StudentLogBiz studenLogBiz = new StudentLogBiz();

        public ActionResult Index(int id)
        {
            ViewBag.id = id;
            List<StudentLog> lsStudentLog = studenLogBiz.GetStudentLogList(new Person() { id = id }); ;

            return View(lsStudentLog);
        }

        //
        // GET: /StudentLog/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /StudentLog/Create

        public ActionResult Create(int id)
        {                        
            return View();
        }

        //
        // POST: /StudentLog/Create

        [HttpPost]
        public ActionResult Create(int id, StudentLog collection)
        {
            try
            {
                #region School identification
                System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                int idColegio = 0;
                int usuario = 0;

                if (context.IsAuthenticated)
                {
                    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                    string[] userRole = ci.Ticket.UserData.Split('|');
                    usuario = int.Parse(userRole[0]);
                    idColegio = int.Parse(userRole[2]);
                }

                School school = new School() { id = idColegio };
                #endregion

                collection.id = 0;
                collection.idEstudiante = id;
                collection.fecha = DateTime.Now;
                collection.idFuncionario = usuario;

                studenLogBiz.SaveStudentLog(collection);

                return RedirectToAction("Index", new { id = id});
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /StudentLog/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /StudentLog/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /StudentLog/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /StudentLog/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
