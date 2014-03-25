using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class SchoolController : Controller
    {
        SchoolBiz oSchool = new SchoolBiz();
        //
        // GET: /School/
        [Authorize]
        public ActionResult Index()
        {


            List<School> lstSchool = oSchool.GetSchoolList();
            return View(lstSchool);
        }

        //
        // GET: /School/Details/5
        //[Authorize]
        //public ActionResult Details(int id)
        //{
        //    School objSchool = oSchool.GetSchoolbyKey(new School() { id = id });
        //    return View(objSchool);
        //}

        //
        // GET: /School/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /School/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(School school)
        {
            try
            {
                // TODO: Add insert logic here
                oSchool.SaveSchool(school);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /School/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            School school = oSchool.GetSchoolbyKey(new School() { id = id });

            return View(school);
        }

        //
        // POST: /School/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, School school)
        {
            try
            {
                School oldSchool = oSchool.GetSchoolbyKey(new School() { id = id });

                school.logo = oldSchool.logo;

                // TODO: Add insert logic here
                oSchool.SaveSchool(school);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /School/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            School objSchool = oSchool.GetSchoolbyKey(new School() { id = id });
            //return View(objSchool);
            oSchool.DeleteSchool(objSchool);
            return RedirectToAction("Index");
            
        }

        //
        // POST: /School/Delete/5
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                oSchool.DeleteSchool(new School() { id = id });

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        public ActionResult CreateSchoolAdmin(int id )
        {
            PlaceBiz placeBiz = new PlaceBiz();

            //vmPerson newPerson = new vmPerson();
            //newPerson.lsPlace = placeBiz.GetPlaceList(school);
            //return View(newPerson);

            return View();
        }

        [Authorize]
        public ActionResult uploadLogo(int id)
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult uploadLogo(int id, HttpPostedFileBase uploadFile)
        {
            int files = Request.Files.Count;

            if (files > 0)
            {
                string physicalPath = HttpContext.Server.MapPath("~") + "\\" + "UploadedFiles" + "\\";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Request.Files[0].FileName);

                Request.Files[0].SaveAs(physicalPath + fileName);

                School school = oSchool.GetSchoolbyKey(new School() { id = id });

                school.logo = fileName;

                SchoolBiz schoolBiz = new SchoolBiz();
                schoolBiz.SaveSchool(school);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}
