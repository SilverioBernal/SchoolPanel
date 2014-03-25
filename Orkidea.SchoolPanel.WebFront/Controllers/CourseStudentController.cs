using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class CourseStudentController : Controller
    {
        CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
        [Authorize]
        public ActionResult IndexCourseStudent(int id)
        {
            CourseBiz courseBiz = new CourseBiz();
            PersonBiz personBiz = new PersonBiz();
            vmCourseStudent currentVmCourseStudent = new vmCourseStudent();

            Course course = courseBiz.GetCoursebyKey(new Course() { id = id });

            currentVmCourseStudent.idCurso = course.id;

            List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(course);
            List<Person> lsEstudiante = personBiz.GetPersonList(new School() { id = course.idColegio }, 5);

            PlaceBiz placeBiz = new PlaceBiz();
            Place place = placeBiz.GetPlaceByKey(new Place() { id = course.idSede });

            foreach (CourseStudent item in lsCourseStudent)
            {
                Person person = lsEstudiante.Where(x => x.id.Equals(item.idEstudiante)).First();

                currentVmCourseStudent.lsVmStudent.Add(new vmCourseStudent()
                {
                    id = item.id,
                    idCurso = course.id,
                    idEstudiante = person.id,
                    estudiante = person.primerNombre + " " + (string.IsNullOrEmpty(person.segundoNombre) ? "" : person.segundoNombre + " ") +
                    person.primerApellido + (string.IsNullOrEmpty(person.segundoApellido) ? "" : " " + person.segundoApellido)
                });
            }

            ViewBag.nombreCurso = course.Descripcion + " - Sede: '" + place.descripcion + "'";

            return View(currentVmCourseStudent);
        }

        [Authorize]
        public ActionResult SearchForStudents(int id)
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

            PlaceBiz placeBiz = new PlaceBiz();
            GradeBiz gradeBiz = new GradeBiz();
            CourseBiz courseBiz = new CourseBiz();

            vmCourseStudentSearchFilter currentVmStudentSeachFilter = new vmCourseStudentSearchFilter();
            currentVmStudentSeachFilter.lsSede = placeBiz.GetPlaceList(school);
            currentVmStudentSeachFilter.lsGrado = gradeBiz.GetGradeList(school);

            List<Course> lsCurso = courseBiz.GetCourseList(school);
            Course cursoActual = lsCurso.Where(x => x.id.Equals(id)).FirstOrDefault();

            List<int> año = lsCurso.Where(x => x.idColegio.Equals(school.id) && x.ano < cursoActual.ano).Select(x => x.ano).Distinct().ToList();

            foreach (int item in año)
            {
                currentVmStudentSeachFilter.lsAnos.Add(item, item.ToString());
            }

            ViewBag.id = id;

            return View(currentVmStudentSeachFilter);
        }

        /*ajustar cuando ya tengamos estudiantes*/
        [Authorize]
        [HttpPost]
        public ActionResult SearchForStudents(int id, Course course)
        {
            PersonBiz personBiz = new PersonBiz();

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

            List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(course);
            vmCourseStudent currentVmCourseStudent = new vmCourseStudent();
            List<Person> lsPerson = personBiz.GetPersonList(school, 5);

            foreach (CourseStudent item in lsCourseStudent)
            {
                Person person = lsPerson.Where(x => x.id.Equals(item.idEstudiante)).First();

                currentVmCourseStudent.lsVmStudent.Add(new vmCourseStudent()
                {
                    id = item.id,
                    idCurso = course.id,
                    idEstudiante = person.id,
                    estudiante = person.primerNombre + " " + (string.IsNullOrEmpty(person.segundoNombre) ? "" : person.segundoNombre + " ") +
                    person.primerApellido + (string.IsNullOrEmpty(person.segundoApellido) ? "" : " " + person.segundoApellido)
                });
            }

            return View("JoinStudent", currentVmCourseStudent);

            /*
             * 1. terminar filtro de estudiantes
             * 2. Los resultados deben mostrar los cursos que coindican don esa busqueda
             *    las opciones deben ser: 
             *    - seleccionar estudiantes: debe llevar a la pantalla donde se muestren los estudiantes acordes a los resultados de la busqueda
             *      uno por uno se agregan
             *    - promover curso: se selecciona el curso entero y se agrega al curso <<ESTA OPCION SE DEJARA DE ULTIMAS HASTA QUE NO TENGAMOS PRUEBAS DE NOTAS>>
             * 3. no se debe olvidar que esta pantalla es para añadir estudiantes a un curso. por lo tanto en los parametros de entrada debe ir el curso.
             *
             *    
             */

        }

        [Authorize]
        public ActionResult AddOlderStudents(string id)
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
            CourseBiz courseBiz = new CourseBiz();

            List<CourseStudent> lsCourseStudent = new List<CourseStudent>();
            List<Person> lsEstudiante = new List<Person>();
            vmCourseStudent currentVmCourseStudent = new vmCourseStudent();

            try
            {
                string[] queryString = id.Split('|');

                Course courseTo = courseBiz.GetCoursebyKey(new Course() { id = int.Parse(queryString[0]) });
                Course courseFrom = new Course() { id = int.Parse(queryString[1]) };
                List<vmCourseStudent> lsVmCourseStudent = new List<vmCourseStudent>();
                
                ViewBag.idCurso = courseTo.id;
                ViewBag.nombreCurso = courseTo.Descripcion;

                lsCourseStudent.AddRange(courseStudentBiz.GetCourseStudentList(courseFrom));
                lsEstudiante.AddRange(personBiz.GetPersonList(new School() { id = school.id }, 5));

                // estudiantes matriculados en cursos del mismo año para ser excluidos 
                List<int> lsCurrentCourseStudent = courseStudentBiz.GetCourseStudentList(new Course() { idColegio = school.id, ano = courseTo.ano }).Select(x => x.idEstudiante).ToList();

                /* estudiantes que pueden matricularse */
                List<int> lsStudent = lsCourseStudent.Where(x => !(lsCurrentCourseStudent.Contains(x.idEstudiante))).Select(x => x.idEstudiante).ToList();

                List<Person> lsPerson = personBiz.GetPersonList(school, 5).Where(x =>
                    x.retirado == false &&
                     (lsStudent.Contains(x.id)))
                    .ToList();


                foreach (Person item in lsPerson)
                {
                    vmCourseStudent person = new vmCourseStudent()
                    {
                        idEstudiante = item.id,
                        estudiante =
                        item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido + " ") +
                                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " "),
                        documento = item.documento
                    };

                    currentVmCourseStudent.lsVmStudent.Add(person);
                }

                //foreach (CourseStudent item in lsCourseStudent)
                //{
                //    Person person = lsEstudiante.Where(x => x.id.Equals(item.idEstudiante)).First();

                //    lsVmCourseStudent.Add(new vmCourseStudent()
                //    {
                //        id = item.id,
                //        idCurso = courseFrom.id,
                //        idEstudiante = person.id,
                //        estudiante = person.primerApellido + (string.IsNullOrEmpty(person.segundoApellido) ? "" : " " + person.segundoApellido + " ") +
                //        person.primerNombre + " " + (string.IsNullOrEmpty(person.segundoNombre) ? "" : person.segundoNombre + " ") 
                //    });
                //}

                currentVmCourseStudent.lsVmStudent.AddRange(lsVmCourseStudent.OrderBy(x => x.estudiante).ToList());
            }
            catch (Exception ex)
            {

            }

            return View(currentVmCourseStudent);
        }

        public ActionResult JoinOldStudentToCourse(string id)
        {
            string res = "";

            try
            {
                string[] joinStudents = id.Split('|');

                int idCurso = int.Parse(joinStudents[0]);

                for (int i = 1; i < joinStudents.Length - 1; i++)
                {
                    courseStudentBiz.SaveCourseStudent(new CourseStudent() { idCurso = idCurso, idEstudiante = int.Parse(joinStudents[i]) });
                }

                res = "OK";
            }
            catch (Exception ex)
            {
                res = "Se presento un error";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CourseByFilters(string filters)
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

            CourseBiz courseBiz = new CourseBiz();

            List<Course> lstCourse = new List<Course>();

            try
            {
                string[] filtersArray = filters.Split('|');

                Place sede = new Place() { id = int.Parse(filtersArray[0]) };
                int jornada = int.Parse(filtersArray[1]);
                int ano = int.Parse(filtersArray[2]);
                Grade grado = new Grade() { id = int.Parse(filtersArray[3]) };

                lstCourse.AddRange(courseBiz.GetCourseList(school, sede, grado, jornada, ano));
            }
            catch (Exception ex)
            {

            }

            return Json(lstCourse, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult JoinStudent(int id)
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

            CourseBiz courseBiz = new CourseBiz();
            Course course = courseBiz.GetCoursebyKey(new Course() { id = id });

            PlaceBiz placeBiz = new PlaceBiz();
            Place place = placeBiz.GetPlaceByKey(new Place() { id = course.idSede });

            PersonBiz personBiz = new PersonBiz();

            // estudiantes matriculados en cursos del mismo año para ser excluidos 
            List<int> lsCourseStudent = courseStudentBiz.GetCourseStudentList(new Course() { idColegio = school.id, ano = course.ano }).Select(x => x.idEstudiante).ToList();

            vmCourseStudent currentVmCourseStudent = new vmCourseStudent();
            currentVmCourseStudent.idCurso = id;

            /* estudiantes que pueden matricularse */
            List<Person> lsPerson = personBiz.GetPersonList(school, 5).Where(x => x.retirado == false && !(lsCourseStudent.Contains(x.id)) && x.idSede.Equals(course.idSede)).ToList();

            foreach (Person item in lsPerson)
            {
                vmCourseStudent person = new vmCourseStudent()
                {
                    idEstudiante = item.id,
                    estudiante =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento
                };

                currentVmCourseStudent.lsVmStudent.Add(person);
            }

            ViewBag.nombreCurso = course.Descripcion + " - Sede: '" + place.descripcion + "'";

            return View(currentVmCourseStudent);
        }

        [Authorize]
        [HttpPost]
        public ActionResult JoinStudent(int id, vmCourseStudent courseStudent)
        {
            string[] estudiantes = courseStudent.estudiante.Split('|');

            foreach (string item in estudiantes)
            {
                if (!string.IsNullOrEmpty(item))
                    courseStudentBiz.SaveCourseStudent(new CourseStudent() { idCurso = id, idEstudiante = int.Parse(item) });
            }

            return RedirectToAction("IndexCourseStudent", "CourseStudent", new { id = id });
        }

        [Authorize]
        public ActionResult removeStudent(int id)
        {
            CourseStudent courseStudent = courseStudentBiz.GetCourseStudentbyKey(new CourseStudent() { id = id });
            courseStudentBiz.DeleteCourseStudent(courseStudent);

            return RedirectToAction("IndexCourseStudent", "CourseStudent", new { id = courseStudent.idCurso });
        }

        /* Reporting */
        [Authorize]
        public ActionResult Constancia()
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

            CourseBiz courseBiz = new CourseBiz();
            PersonBiz personBiz = new PersonBiz();

            List<int> lsCursos = courseBiz.GetCourseList(school).Where(x => !x.finalizado).Select(x => x.id).Distinct().ToList();
            List<int> lsCourseStudent = courseStudentBiz.GetCourseStudentList().Where(x => lsCursos.Contains(x.idCurso)).Select(x => x.idEstudiante).Distinct().ToList();

            List<Person> lsPerson = personBiz.GetPersonList(school).Where(x => lsCourseStudent.Contains(x.id)).ToList();
            List<vmPerson> lsVmPerson = new List<vmPerson>();

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Retirado" : "Activo"
                };

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson);
        }

        public ActionResult ConstanciaDestino(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public FileContentResult ConstanciaRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int idEstudiante = int.Parse(parametros[0]);
                string destino = string.IsNullOrEmpty(parametros[1]) ? "" : parametros[1];

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/Constancia.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue idEstudianteDiscreteValue = new ParameterDiscreteValue();
                idEstudianteDiscreteValue.Value = idEstudiante;
                rpt.SetParameterValue("idEstudiante", idEstudianteDiscreteValue);

                ParameterDiscreteValue destinoDiscreteValue = new ParameterDiscreteValue();
                destinoDiscreteValue.Value = destino;
                rpt.SetParameterValue("destino", destinoDiscreteValue);

                CrystalDecisions.Shared.ConnectionInfo connectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                connectionInfo.DatabaseName = oConnBuilder.InitialCatalog;
                connectionInfo.UserID = oConnBuilder.UserID;
                connectionInfo.Password = oConnBuilder.Password;
                connectionInfo.ServerName = oConnBuilder.DataSource;

                Tables tables = rpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
                {
                    CrystalDecisions.Shared.TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                }

                for (int i = 0; i < rpt.DataSourceConnections.Count; i++)
                {
                    rpt.DataSourceConnections[i].SetConnection(oConnBuilder.DataSource, oConnBuilder.InitialCatalog, oConnBuilder.UserID, oConnBuilder.Password);
                }

                rpt.SetDatabaseLogon(oConnBuilder.UserID, oConnBuilder.Password, oConnBuilder.DataSource, oConnBuilder.InitialCatalog);


                System.IO.MemoryStream strMemory = (System.IO.MemoryStream)rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                response = new byte[strMemory.Length];

                strMemory.Read(response, 0, (int)strMemory.Length);

                return new FileContentResult(response, "application/pdf");
            }
            catch (Exception ex)
            {
                string error = ex.Message + " :::---::: " + ex.StackTrace;

                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
                byte[] response = codificador.GetBytes(error);

                return new FileContentResult(response, "text/plain");
            }
        }

        //CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
        ////
        //// GET: /CourseStudent/5

        //[Authorize]
        //public ActionResult Index(int id)
        //{

        //    StudentBiz studentBiz = new StudentBiz();
        //    CourseBiz courseBiz = new CourseBiz();
        //    GradeBiz gradeBiz = new GradeBiz();
        //    vmCourse oCourse = new vmCourse();
        //    Course course = courseBiz.GetCoursebyKey(new Course() { id = id });            

        //    oCourse.id = course.id;
        //    oCourse.Descripcion = course.Descripcion;
        //    oCourse.ano = course.ano.ToString();

        //    int añoAnterior = course.ano - 1;
        //    List<Course> lstCourse = courseBiz.GetCourseListByYear(new Course() { idColegio = course.idColegio, ano = añoAnterior });

        //    foreach (Course item in lstCourse)
        //    {
        //        Grade oGrade = gradeBiz.GetGradebyKey(new Grade() { id = item.idGrado });

        //        oCourse.lstPreviusCourses.Add(new vmCourse() { 
        //            id = item.id,
        //            Descripcion = item.Descripcion,
        //            ano = item.ano.ToString(),
        //            desGrado = oGrade.NombreGrado
        //        });
        //    }


        //    List<CourseStudent> courseStudent = courseStudentBiz.GetCourseStudentListByCourse(course);

        //    foreach (CourseStudent item in courseStudent)
        //    {
        //        Student student = studentBiz.GetStudentbyKey(new Student() { id = item.idEstudiante, idColegio = item.idColegio });


        //        vmCourseStudent oVmCourseStudent = new vmCourseStudent();

        //        oVmCourseStudent.idColegio = item.idColegio;
        //        oVmCourseStudent.idCurso = item.idCurso;
        //        oVmCourseStudent.idEstudiante = item.idEstudiante;
        //        oVmCourseStudent.desCurso = course.Descripcion;
        //        oVmCourseStudent.desEstudiante =
        //            student.primerNombre + " " + (string.IsNullOrEmpty(student.segundoNombre) ? "" : (student.segundoNombre + " ")) +
        //            student.primerApellido + (string.IsNullOrEmpty(student.segundoApellido) ? "" : (" " + student.segundoApellido));

        //        oCourse.lstCourseStudents.Add(oVmCourseStudent);
        //    }
        //    return View(oCourse);
        //}

        //[Authorize]
        //public ActionResult Create(int id)
        //{
        //    CourseBiz courseBiz = new CourseBiz();

        //    Course course = courseBiz.GetCoursebyKey(new Course() { id = id });
        //    vmCourse oCourse = new vmCourse();

        //    oCourse.id = course.id;
        //    oCourse.Descripcion = course.Descripcion;
        //    oCourse.ano = course.ano.ToString();

        //    List<Student> student = courseStudentBiz.GetStudentNoCourseList(course.ano, course.idColegio);

        //    foreach (Student item in student)
        //    {
        //        vmCourseStudent oStudent = new vmCourseStudent();
        //        oStudent.idEstudiante = item.id;
        //        oStudent.idColegio = item.idColegio;
        //        oStudent.desEstudiante =
        //           item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : (item.segundoNombre + " ")) +
        //           item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : (" " + item.segundoApellido));

        //        oCourse.lstStudentsNoCourse.Add(oStudent);
        //    }

        //    return View(oCourse);
        //}

        //[Authorize]
        //public ActionResult IncludeStudent(int idCourse, int idStudent)
        //{
        //    CourseBiz courseBiz = new CourseBiz();
        //    Course course = courseBiz.GetCoursebyKey(new Course() { id = idCourse });
        //    try
        //    {
        //        courseStudentBiz.SaveCourseStudent(new CourseStudent()
        //        {
        //            idColegio = course.idColegio,
        //            idCurso = course.id,
        //            idEstudiante = idStudent
        //        });
        //        return RedirectToAction("Index", new { id = idCourse});
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", new { id = idCourse });
        //    }
        //}

        //[Authorize]
        //public ActionResult Delete(int idCourse, int idStudent)
        //{
        //    CourseBiz courseBiz = new CourseBiz();
        //    Course course = courseBiz.GetCoursebyKey(new Course() { id = idCourse });
        //    try
        //    {
        //        courseStudentBiz.DeleteCourseStudent(new CourseStudent()
        //        {
        //            idColegio = course.idColegio,
        //            idCurso = course.id,
        //            idEstudiante = idStudent
        //        });
        //        return RedirectToAction("Index", new { id = idCourse });
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", new { id = idCourse });
        //    }
        //}

        //[Authorize]
        //public ActionResult CourseByStudentIndex()
        //{
        //    #region School identification
        //    System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //    int idColegio = 0;
        //    int usuario = 0;

        //    if (context.IsAuthenticated)
        //    {
        //        System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //        string[] userRole = ci.Ticket.UserData.Split('|');
        //        usuario = int.Parse(userRole[0]);
        //        idColegio = int.Parse(userRole[2]);
        //    }

        //    School school = new School() { id = idColegio };
        //    #endregion

        //    CourseBiz courseBiz = new CourseBiz();
        //    AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

        //    List<vmCourseStudent> courseStudent = new List<vmCourseStudent>();
        //    List<CourseStudent> lstCourseStudentDraft = courseStudentBiz.GetCourseStudentbyStudent(new CourseStudent() { idEstudiante = usuario, idColegio = idColegio });
        //    List<Course> lstCourses = courseBiz.GetCourseList(new School() { id = idColegio });
        //    List<AcademicPeriod> lstAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

        //    foreach (CourseStudent item in lstCourseStudentDraft)
        //    {
        //        Course course = (from x in lstCourses where x.id == item.idCurso select x).FirstOrDefault();

        //        if (course != null)
        //        {
        //            courseStudent.Add(new vmCourseStudent()
        //            {
        //                idColegio = item.idColegio,
        //                idCurso = item.idCurso,
        //                ano = course.ano,
        //                desCurso = course.Descripcion,
        //                lstAcademicPeriod = lstAcademicPeriod
        //            });
        //        }
        //    }

        //    ViewBag.idEstudiante = usuario;

        //    return View(courseStudent);
        //}

        //[Authorize]
        //public ActionResult coursePromotion(int idCourseFrom, int idCourseTo)
        //{
        //    CourseBiz courseBiz = new CourseBiz();

        //    try
        //    {
        //        Course course = courseBiz.GetCoursebyKey(new Course() { id = idCourseFrom });
        //        List<CourseStudent> courseStudent = courseStudentBiz.GetCourseStudentListByCourse(course);


        //        foreach (CourseStudent item in courseStudent)
        //        {
        //            courseStudentBiz.SaveCourseStudent(new CourseStudent()
        //            {
        //                idColegio = item.idColegio,
        //                idCurso = idCourseTo,
        //                idEstudiante = item.idEstudiante
        //            });    
        //        }

        //        return RedirectToAction("Index", new { id = idCourseTo });
        //    }
        //    catch
        //    {
        //        return RedirectToAction("Index", new { id = idCourseTo });
        //    }

        //}
    }
}
