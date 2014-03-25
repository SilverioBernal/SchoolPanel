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
    public class CourseAsignatureController : Controller
    {
        CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();

        //
        // GET: /CourseAsignature/

        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult IndexCourseAsignature(int id)
        {
            vmCourseAsignature currentVmCourseAsignature = new vmCourseAsignature();

            CourseBiz courseBiz = new CourseBiz();
            Course course = courseBiz.GetCoursebyKey(new Course() { id = id });

            PlaceBiz placeBiz = new PlaceBiz();
            Place place = placeBiz.GetPlaceByKey(new Place() { id = course.idSede });

            GradeBiz gradeBiz = new GradeBiz();
            Grade grade = gradeBiz.GetGradebyKey(new Grade() { id = course.idGrado });

            AsignatureBiz asignatureBiz = new AsignatureBiz();
            List<Asignature> lsAsignature = asignatureBiz.GetAsignatureList(grade);

            PersonBiz personBiz = new PersonBiz();
            List<Person> lsProfesor = personBiz.GetPersonList(new School() { id = grade.idColegio }).Where(x => x.idRol.Equals(4) && !x.retirado).ToList();

            List<CourseAsignature> lsCourseAsignature = courseAsignatureBiz.GetCourseAsignatureList(new Course() { id = id });

            if (lsCourseAsignature.Count() < lsAsignature.Count())
            {
                foreach (Asignature item in lsAsignature)
                {
                    if (lsCourseAsignature.Where(c => c.idAsignatura.Equals(item.id)).Count() == 0)
                    {
                        CourseAsignature courseAsignature = new CourseAsignature()
                        {
                            idAsignatura = item.id,
                            idCurso = id
                        };

                        courseAsignatureBiz.SaveCourseAsignature(courseAsignature);
                    }
                }

                lsCourseAsignature = courseAsignatureBiz.GetCourseAsignatureList(new Course() { id = id });
            }

            List<vmCourseAsignature> lsVmCourseAsignature = new List<vmCourseAsignature>();

            foreach (CourseAsignature itemCourseAsignature in lsCourseAsignature)
            {
                vmCourseAsignature itemVmCourseAsignature = new vmCourseAsignature()
                {
                    id = itemCourseAsignature.id,
                    idAsignatura = itemCourseAsignature.idAsignatura,
                    idCurso = itemCourseAsignature.idCurso,
                    //lsProfesor = lsVmPerson
                };

                itemVmCourseAsignature.asignatura = lsAsignature.Where(c => c.id.Equals(itemCourseAsignature.idAsignatura)).Select(c => c.Descripcion).First();

                if (itemCourseAsignature.idProfesor != null)
                {
                    Person profesor = lsProfesor.Where(c => c.id.Equals(itemCourseAsignature.idProfesor)).First();
                    itemVmCourseAsignature.idProfesor = itemCourseAsignature.idProfesor;
                    itemVmCourseAsignature.profesor = profesor.primerNombre + " " + (string.IsNullOrEmpty(profesor.segundoNombre) ? "" : profesor.segundoNombre + " ") +
                        profesor.primerApellido + (string.IsNullOrEmpty(profesor.segundoApellido) ? "" : " " + profesor.segundoApellido);
                }

                lsVmCourseAsignature.Add(itemVmCourseAsignature);
            }

            currentVmCourseAsignature.lsCourseAsignature = lsVmCourseAsignature;

            foreach (Person item in lsProfesor.OrderBy(X => X.primerNombre).ThenBy(x => x.segundoNombre).ToList())
            {
                //if (item.idSede.Equals(course.idSede) && item.idJornada.Equals(course.idJornada) && item.usuarioActivo && item.idRol.Equals(4))
                //if (item.usuarioActivo && item.idRol.Equals(4))
                //{
                currentVmCourseAsignature.lsProfesor.Add(new vmPerson()
                {
                    id = item.id,
                    nombreCompleto = item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido)
                });
                //}
            }

            ViewBag.nombreCurso = course.Descripcion + " - Sede: '" + place.descripcion + "'";

            return View(currentVmCourseAsignature);
        }

        //
        // GET: /CourseAsignature/Create
        public ActionResult Edit(string id)
        {
            string[] parametros = id.Split('|');

            CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = int.Parse(parametros[0]) });

            if (!string.IsNullOrEmpty(parametros[1]))
                courseAsignature.idProfesor = int.Parse(parametros[1]);
            else
                courseAsignature.idProfesor = null;

            courseAsignatureBiz.SaveCourseAsignature(courseAsignature);

            return RedirectToAction("IndexCourseAsignature", new { id = courseAsignature.idCurso });
        }
    }
}
