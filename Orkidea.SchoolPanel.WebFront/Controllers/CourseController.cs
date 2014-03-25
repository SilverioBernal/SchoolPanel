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
    public class CourseController : Controller
    {
        CourseBiz courseBiz = new CourseBiz();
        //
        // GET: /Course/
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

            PersonBiz personBiz = new PersonBiz();
            PlaceBiz placeBiz = new PlaceBiz();
            GradeBiz gradeBiz = new GradeBiz();

            List<Person> lsTeacher = personBiz.GetPersonList(school, 4);
            List<Grade> lsGrade = gradeBiz.GetGradeList(school);
            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<Course> lstCourse = courseBiz.GetCourseList(school);

            List<vmCourse> lstVmCourse = new List<vmCourse>();

            foreach (Course item in lstCourse)
            {
                vmCourse itemVmCourse = new vmCourse()
                {
                    id = item.id,
                    idColegio = item.idColegio,
                    idGrado = item.idGrado,
                    ano = item.ano,
                    Descripcion = item.Descripcion,
                    idDirectorCurso = item.idDirectorCurso,
                    finalizado = item.finalizado
                };

                Person directorCurso = lsTeacher.Where(c => c.id.Equals(item.idDirectorCurso)).First();

                itemVmCourse.directorCurso = directorCurso.primerNombre + " " + (string.IsNullOrEmpty(directorCurso.segundoNombre) ? "" : (directorCurso.segundoNombre + " ")) +
                    directorCurso.primerApellido + (string.IsNullOrEmpty(directorCurso.segundoApellido) ? "" : (" " + directorCurso.segundoApellido));

                itemVmCourse.grado = lsGrade.Where(c => c.id.Equals(item.idGrado)).Select(c => c.NombreGrado).First();

                switch (item.idJornada)
                {
                    case 1:
                        itemVmCourse.jornada = "Mañana";
                        break;
                    case 2:
                        itemVmCourse.jornada = "Tarde";
                        break;
                    case 3:
                        itemVmCourse.jornada = "Noche";
                        break;
                    case 4:
                        itemVmCourse.jornada = "Única";
                        break;
                    default:
                        break;
                }

                itemVmCourse.sede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();


                lstVmCourse.Add(itemVmCourse);
            }

            return View(lstVmCourse);
        }


        //
        // GET: /Course/Create
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

            PersonBiz personBiz = new PersonBiz();
            PlaceBiz placeBiz = new PlaceBiz();
            GradeBiz gradeBiz = new GradeBiz();

            List<Person> lsPerson = personBiz.GetPersonList(school, 4).OrderBy(x => x.primerNombre).ToList();

            vmCourse newVmCourse = new vmCourse() {};

            foreach (Person item in lsPerson)
            {
                vmPerson itemVmPerson = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto = item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : (item.segundoNombre + " ")) +
                      item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : (" " + item.segundoApellido))
                };

                newVmCourse.lsTeacher.Add(itemVmPerson);
            }

            newVmCourse.lsGrade = gradeBiz.GetGradeList(school);
            newVmCourse.lsPlace = placeBiz.GetPlaceList(school);

            return View(newVmCourse);
        }

        //
        // POST: /Course/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Course course)
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

                course.idColegio = school.id;
                courseBiz.SaveCourse(course);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Course/Edit/5
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

            PersonBiz personBiz = new PersonBiz();
            PlaceBiz placeBiz = new PlaceBiz();
            GradeBiz gradeBiz = new GradeBiz();

            List<Person> lsPerson = personBiz.GetPersonList(school, 4).OrderBy(x => x.primerNombre).ToList();

            Course course = courseBiz.GetCoursebyKey(new Course() { id = id });

            vmCourse currentVmCourse = new vmCourse()
            {
                id = course.id,
                idColegio = course.idColegio,
                idGrado = course.idGrado,
                ano = course.ano,
                Descripcion = course.Descripcion,
                idDirectorCurso = course.idDirectorCurso,
                idSede = course.idSede,
                idJornada = course.idJornada,
                finalizado = course.finalizado 
            };

            foreach (Person item in lsPerson)
            {
                vmPerson itemVmPerson = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto = item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : (item.segundoNombre + " ")) +
                      item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : (" " + item.segundoApellido))
                };

                currentVmCourse.lsTeacher.Add(itemVmPerson);
            }

            currentVmCourse.lsGrade = gradeBiz.GetGradeList(school);
            currentVmCourse.lsPlace = placeBiz.GetPlaceList(school);

            return View(currentVmCourse);
        }

        //
        // POST: /Course/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Course course)
        {
            try
            {
                // TODO: Add update logic here
                course.id = id;
                courseBiz.SaveCourse(course);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Course/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            courseBiz.DeleteCourse(new Course() { id = id });

            return RedirectToAction("Index");
        }

    }
}
