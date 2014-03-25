using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Business;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class StudentBitacoreController : Controller
    {
        ////
        //// GET: /StudentBitacore/
        //[Authorize]
        //public ActionResult Index(int? id)
        //{
        //    #region School identification
        //    System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //    int idColegio = 0, usuario = 0, rol = 0;

        //    if (context.IsAuthenticated)
        //    {
        //        System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //        string[] userRole = ci.Ticket.UserData.Split('|');
        //        usuario = int.Parse(userRole[0]);
        //        rol = int.Parse(userRole[1]);
        //        idColegio = int.Parse(userRole[2]);
        //    }

        //    School school = new School() { id = idColegio };
        //    #endregion

        //    if (id == null)
        //        id = usuario;

        //    StudentBitacoreBiz studentBitacoreBiz = new StudentBitacoreBiz();

        //    List<StudentBitacore> lstStudentBitacore = studentBitacoreBiz.GetStudentBitacoreList(new Student() { id =(int)id, idColegio = school.id });
        //    ViewBag.id = id;
        //    ViewBag.rol = rol;
        //    return View(lstStudentBitacore);
        //}

        //////
        ////// GET: /StudentBitacore/Details/5
        ////[Authorize]
        ////public ActionResult Details(string id)
        ////{
        ////    return View();
        ////}

        ////
        //// GET: /StudentBitacore/Create
        //[Authorize]
        //public ActionResult Create(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /StudentBitacore/Create
        //[Authorize]
        //[HttpPost]
        //public ActionResult Create(int id, StudentBitacore studentBitacore)
        //{
        //    try
        //    {
        //        StudentBitacoreBiz studentBitacoreBiz = new StudentBitacoreBiz();

        //        #region School identification
        //        System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //        int idColegio = 0;

        //        if (context.IsAuthenticated)
        //        {
        //            System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //            string[] userRole = ci.Ticket.UserData.Split('|');
        //            idColegio = int.Parse(userRole[2]);
        //        }

        //        School school = new School() { id = idColegio };
        //        #endregion

        //        studentBitacore.idEstudiante = id;
        //        studentBitacore.idColegio = school.id;
        //        studentBitacore.fecha = DateTime.Now;

        //        studentBitacoreBiz.SaveStudentBitacore(studentBitacore);

        //        return RedirectToAction("Index", new { id = id });
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

    }
}
