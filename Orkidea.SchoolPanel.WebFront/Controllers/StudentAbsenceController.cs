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
    public class StudentAbsenceController : Controller
    {
        StudentAbsenceBiz studentAbsenceBiz = new StudentAbsenceBiz();
        //
        // GET: /StudentAbsence/
        [Authorize]
        public ActionResult Index(int id)
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

            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = id });
            List<StudentAbsence> lsStudentAbsence = studentAbsenceBiz.GetStudentAbsenceList(courseAsignature);


            CourseBiz courseBiz = new CourseBiz();
            Course course = courseBiz.GetCoursebyKey(new Course() { id = courseAsignature.idCurso });

            CourseStudentBiz courseStudentBiz = new Business.CourseStudentBiz();
            List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(course);

            PersonBiz personBiz = new PersonBiz();
            List<Person> lsPerson = personBiz.GetPersonList(new Course() { id = courseAsignature.idCurso }, 5);

            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            List<AcademicPeriod> academicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            vmStudentAbsence studenAbsence = new vmStudentAbsence() { idAsignatura = id, lsPeriodoAcademico = academicPeriod };

            foreach (CourseStudent item in lsCourseStudent)
            {
                Person estudiante = lsPerson.Where(x => x.id.Equals(item.idEstudiante)).First();
                vmStudentAbsence registroEstudiante = new vmStudentAbsence()
                {
                    idEstudiante = item.id,
                    idAsignatura = id,
                    estudiante = estudiante.primerNombre + " " + (string.IsNullOrEmpty(estudiante.segundoNombre) ? "" : estudiante.segundoNombre + " ") +
                    estudiante.primerApellido + (string.IsNullOrEmpty(estudiante.segundoApellido) ? "" : " " + estudiante.segundoApellido)
                };

                registroEstudiante.lsFallasEstudiante = lsStudentAbsence.Where(x => x.idAsignatura.Equals(id) && x.idEstudiante.Equals(item.id)).ToList();

                studenAbsence.lsStudentAbsence.Add(registroEstudiante);
            }

            return View(studenAbsence);
        }

        //
        // GET: /StudentAbsence/Create
        [Authorize]
        public ActionResult Create(string id)
        {
            string[] parametros = id.Split('|');

            int idEstudiante = int.Parse(parametros[0]);
            int idAsignatura = int.Parse(parametros[1]);
            int idPeriodoAcademico = int.Parse(parametros[2]);
            DateTime fecha = DateTime.Parse(parametros[3]);

            StudentAbsence absence = new StudentAbsence()
            {
                idEstudiante = idEstudiante,
                idAsignatura = idAsignatura,
                idPeriodoAcademico = idPeriodoAcademico,
                fecha = fecha
            };

            studentAbsenceBiz.SaveStudentAbsence(absence);

            return RedirectToAction("Index", new { id = idAsignatura });
        }        
    }
}
