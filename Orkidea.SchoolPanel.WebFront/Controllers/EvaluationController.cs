using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ExporterObjects;
using System.Data.OleDb;
using System.Data;
using Orkidea.SchoolPanel.WebFront.DsReporting;

//using iTextSharp.text;
//using DocumentFormat.OpenXml.Office2010.ExcelAc;

//using System.Web.UI;
//using System.Web.UI.WebControls;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class EvaluationController : Controller
    {
        EvaluationBiz evaluationBiz = new EvaluationBiz();

        /****** Entrada Masiva Profesor********/
        [Authorize]
        public FileResult downloadExcel(string id)
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

            string[] evaluationKey = id.Split('|');

            EvaluationNoteBiz evaluationNoteBiz = new EvaluationNoteBiz();
            List<EvaluationNote> lsEvaluationNote = evaluationNoteBiz.GetEvaluationNoteList(school);

            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
            List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(new Course() { id = courseAsignature.idCurso });

            PersonBiz personBiz = new PersonBiz();
            List<Person> lsPerson = personBiz.GetPersonList(new Course() { id = courseAsignature.idCurso }, 5);

            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            List<AcademicPeriod> lsAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            List<Evaluation> lsEvaluation = evaluationBiz.GetEvaluationList(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            bool reevaluar = false;

            //ingreso de boletines en cero cuando el alumno no tenga registro en la tabla evaluation
            foreach (AcademicPeriod currentAcademicPeriod in lsAcademicPeriod)
            {
                int alumnosEvaluados = lsEvaluation.Where(x => x.idPeriodoAcademico.Equals(currentAcademicPeriod.id)).Count();

                if (alumnosEvaluados < lsCourseStudent.Count())
                {
                    reevaluar = true;
                    foreach (CourseStudent currentStudent in lsCourseStudent)
                    {
                        int alumnoEvaluado = lsEvaluation.Where(x => x.idEstudiante.Equals(currentStudent.id) && x.idPeriodoAcademico.Equals(currentAcademicPeriod.id)).Count();

                        if (alumnoEvaluado == 0)
                            evaluationBiz.SaveEvaluation(new Evaluation() { idPeriodoAcademico = currentAcademicPeriod.id, idEstudiante = currentStudent.id, idAsignatura = courseAsignature.id, Nota = 0, numeroFallas = 0 });
                    }
                }
            }

            if (reevaluar)
                lsEvaluation = evaluationBiz.GetEvaluationList(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            List<vmEvaluationDownload> lsStudentEvaluation = new List<vmEvaluationDownload>();

            foreach (Evaluation item in lsEvaluation.Where(x => x.idPeriodoAcademico.Equals(int.Parse(evaluationKey[1]))).ToList())
            {
                int idEstudiante = lsCourseStudent.Where(x => x.id.Equals(item.idEstudiante)).Select(x => x.idEstudiante).First();
                Person currentStudent = lsPerson.Where(x => x.id.Equals(idEstudiante)).First();

                string nombreAlumno = currentStudent.primerApellido + " " + (string.IsNullOrEmpty(currentStudent.segundoApellido) ? "" : currentStudent.segundoApellido + " ") +
                    currentStudent.primerNombre + (string.IsNullOrEmpty(currentStudent.segundoNombre) ? "" : " " + currentStudent.segundoNombre);

                vmEvaluationDownload currenEvaluation = new vmEvaluationDownload()
                {
                    idEstudiante = item.idEstudiante,
                    desEstudiante = nombreAlumno,
                };

                lsStudentEvaluation.Add(currenEvaluation);
            }


            ExportList<vmEvaluationDownload> exp = new ExportList<vmEvaluationDownload>();
            exp.PathTemplateFolder = Server.MapPath("~/exports");
            string filePathExport = Server.MapPath("~/exports/planilla.xlsx");
            exp.ExportTo(lsStudentEvaluation, ExportToFormat.Excel2007, filePathExport);
            return this.File(filePathExport, "application/octet-stream", System.IO.Path.GetFileName(filePathExport));
        }

        [Authorize]
        public ActionResult UploadNotes(string id)
        {
            ViewBag.id = id;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadNotes(string id, HttpPostedFileBase uploadFile)
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

            EvaluationNoteBiz evaluationNoteBiz = new Business.EvaluationNoteBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
            PersonBiz personBiz = new PersonBiz();

            List<EvaluationNote> lsLogros = evaluationNoteBiz.GetEvaluationNoteList(school);
            List<AcademicPeriod> lsAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            int files = Request.Files.Count;

            if (files > 0)
            {
                string[] evaluationKey = id.Split('|');

                int asignaturaCurso = int.Parse(evaluationKey[0]);
                int periodo = int.Parse(evaluationKey[1]);

                #region Upload del archivo
                string physicalPath = HttpContext.Server.MapPath("~") + "\\UploadedFiles\\";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Request.Files[0].FileName); //".xlsx";

                Request.Files[0].SaveAs(physicalPath + fileName);

                //Create connection string to Excel work book
                string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + physicalPath + fileName + ";Extended Properties=Excel 12.0;Persist Security Info=False";
                //Create Connection to Excel work book
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                //Create OleDbCommand to fetch data from Excel
                OleDbCommand cmd = new OleDbCommand("Select [idEstudiante],[Nota1],[Nota2],[Nota3],[Nota4],[Comportamiento],[comentario1],[comentario2],[numeroFallas],[observaciones] from [Sheet1$]", excelConnection);

                excelConnection.Open();
                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();
                #endregion

                #region almacenamiento de las notas
                #region busqueda de estudiantes por curso

                CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = asignaturaCurso });
                List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(new Course() { id = courseAsignature.idCurso });
                //List<Person> lsPerson = personBiz.GetPersonList(new Course() { id = courseAsignature.idCurso }, 5);

                #endregion

                foreach (IDataRecord record in GetFromReader(dReader))
                {
                    int logro1Valido, logro2Valido;

                    if (!string.IsNullOrEmpty(record["idEstudiante"].ToString()))
                    {
                        int estudianteCurso = lsCourseStudent.Where(x => x.idEstudiante.Equals(int.Parse(record["idEstudiante"].ToString()))).Select(x => x.id).FirstOrDefault();

                        /*promedio de las 4 notas subidas */

                        decimal parcial1 = string.IsNullOrEmpty(record["Nota1"].ToString()) ? 0 : decimal.Parse(record["Nota1"].ToString());
                        decimal parcial2 = string.IsNullOrEmpty(record["Nota2"].ToString()) ? 0 : decimal.Parse(record["Nota2"].ToString());
                        decimal parcial3 = string.IsNullOrEmpty(record["Nota3"].ToString()) ? 0 : decimal.Parse(record["Nota3"].ToString());
                        decimal parcial4 = string.IsNullOrEmpty(record["Nota4"].ToString()) ? 0 : decimal.Parse(record["Nota4"].ToString());

                        Evaluation nota = new Evaluation()
                        {
                            idEstudiante = estudianteCurso,
                            idAsignatura = asignaturaCurso,
                            idPeriodoAcademico = periodo,
                            Nota = (parcial1 + parcial2 + parcial3 + parcial4) / 4,
                            numeroFallas = string.IsNullOrEmpty(record["numeroFallas"].ToString()) ? 0 : int.Parse(record["numeroFallas"].ToString()),
                            observaciones = record["observaciones"].ToString()
                        };

                        if (!string.IsNullOrEmpty(record["comentario1"].ToString()))
                        {
                            logro1Valido = lsLogros.Where(x => x.id.Equals(int.Parse(record["comentario1"].ToString()))).Count();

                            if (logro1Valido > 0)
                                nota.comentario1 = int.Parse(record["comentario1"].ToString());
                        }

                        if (!string.IsNullOrEmpty(record["comentario2"].ToString()))
                        {
                            logro2Valido = lsLogros.Where(x => x.id.Equals(int.Parse(record["comentario2"].ToString()))).Count();

                            if (logro2Valido > 0)
                                nota.comentario2 = int.Parse(record["comentario2"].ToString());
                        }

                        evaluationBiz.SaveEvaluation(nota);

                        /* evaluacion de diciplina */

                        DiciplineEvaluation diciplineEvaluation = new DiciplineEvaluation()
                        {
                            idEstudiante = estudianteCurso,
                            idAsignatura = asignaturaCurso,
                            idPeriodoAcademico = periodo,
                            Nota = string.IsNullOrEmpty(record["Comportamiento"].ToString()) ? 0 : decimal.Parse(record["Comportamiento"].ToString())
                        };

                        evaluationBiz.SaveDiciplineEvaluation(diciplineEvaluation);
                    }
                }
                #endregion
            }
            return RedirectToAction("EnterResult", new { id = id });
        }

        private IEnumerable<IDataRecord> GetFromReader(IDataReader reader)
        {
            while (reader.Read()) yield return reader;
        }



        /****** Entrada Masiva Admin ********/
        [Authorize]
        public ActionResult IndexUpload()
        {
            SchoolBiz schoolBiz = new SchoolBiz();

            vmEvaluation oEvaluation = new vmEvaluation();
            oEvaluation.lstColegio = schoolBiz.GetSchoolList();

            return View(oEvaluation);
        }

        [Authorize]
        public ActionResult YearsBySchool(int idColegio)
        {
            List<int> anos = new List<int>();
            CourseBiz courseBiz = new CourseBiz();

            List<Course> lstCourse = courseBiz.GetCourseList(new School() { id = idColegio });

            anos = (from x in lstCourse select x.ano).Distinct().ToList();

            return Json(anos, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CourseByYear(int idColegio, int ano)
        {
            CourseBiz courseBiz = new CourseBiz();
            List<Course> lstCourse = courseBiz.GetCourseList(new School() { id = idColegio }, ano);

            return Json(lstCourse, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AcademicPeriodBySchool(int idColegio)
        {
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            List<AcademicPeriod> lstAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(new School() { id = idColegio });

            return Json(lstAcademicPeriod, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult AsignatureByCourse(int idCurso)
        {
            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            AsignatureBiz asignatureBiz = new AsignatureBiz();

            List<CourseAsignature> courseTeacher = courseAsignatureBiz.GetCourseAsignatureList(new Course() { id = idCurso });
            List<Asignature> assingnature = new List<Asignature>();

            foreach (CourseAsignature item in courseTeacher)
            {
                assingnature.Add(asignatureBiz.GetAsignaturebyKey(new Asignature() { id = item.idAsignatura }));
            }

            return Json(assingnature, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourseAsignatureID(string courseAsignature)
        {
            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();

            string res = "";

            try
            {
                string[] id = courseAsignature.Split('|');

                int curso = int.Parse(id[0]);
                int asignatura = int.Parse(id[1]);

                CourseAsignature ca = courseAsignatureBiz.GetCourseAsignatureByOthers(new CourseAsignature() { idCurso = curso, idAsignatura = asignatura });

                res = ca.id.ToString();
            }
            catch (Exception)
            {
                res = "";
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        //[Authorize]
        //[HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CreateUpload(vmEvaluation evaluation, HttpPostedFileBase uploadFile)
        //{
        //    EvaluationBiz evaluationBiz = new EvaluationBiz();

        //    SchoolBiz schoolBiz = new SchoolBiz();
        //    School school = schoolBiz.GetSchoolbyKey(new School() { id = int.Parse(evaluation.idColegio) });

        //    CourseBiz courseBiz = new CourseBiz();
        //    Course course = courseBiz.GetCoursebyKey(new Course() { id = int.Parse(evaluation.idCurso) });

        //    PersonBiz personBiz = new PersonBiz();
        //    List<Person> lsStudent = personBiz.GetPersonList(course, 5);

        //    AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
        //    AcademicPeriod academicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = evaluation.idPeriodoAcademico });

        //    AsignatureBiz asignatureBiz = new AsignatureBiz();
        //    Asignature asignature = asignatureBiz.GetAsignaturebyKey(new Asignature() { id = evaluation.idAsignatura });

        //    CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
        //    CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByOthers(new CourseAsignature() { idAsignatura = evaluation.idAsignatura, idCurso = int.Parse(evaluation.idCurso) });

        //    CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
        //    List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(course);



        //    evaluation.desColegio = school.nombreColegio;
        //    evaluation.desCurso = course.Descripcion;
        //    evaluation.ano = course.ano.ToString();
        //    evaluation.desPeriodoAcademico = academicPeriod.Descripcion;

        //    int files = Request.Files.Count;

        //    if (files > 0)
        //    {
        //        string physicalPath = HttpContext.Server.MapPath("~") + "UploadedFiles" + "\\";
        //        string fileName = Guid.NewGuid().ToString() + ".csv";

        //        Request.Files[0].SaveAs(physicalPath + fileName);

        //        StreamReader reader = new StreamReader(physicalPath + fileName);

        //        while (reader.Peek() >= 0)
        //        {
        //            string[] lineaNotas = reader.ReadLine().Split(',');
        //            CourseStudent courseStudent = new CourseStudent();
        //            Person student = new Person();
        //            string estudiante = "";

        //            try
        //            {
        //                courseStudent = lsCourseStudent.Where(x => x.idEstudiante.Equals(int.Parse(lineaNotas[0]))).FirstOrDefault();
        //                student = lsStudent.Where(x => x.id.Equals(courseStudent.idEstudiante)).FirstOrDefault();
        //            }
        //            catch (Exception)
        //            {
        //                student.id = int.Parse(lineaNotas[0]);
        //                student.primerNombre = "No";
        //                student.primerApellido = "Existe";
        //            }

        //            try
        //            {
        //                evaluationBiz.SaveEvaluation(new Evaluation()
        //                {
        //                    idPeriodoAcademico = academicPeriod.id,
        //                    idAsignatura = courseAsignature.id,
        //                    idEstudiante = courseStudent.id,
        //                    Nota = decimal.Parse(lineaNotas[1]),
        //                    comentario1 = int.Parse(lineaNotas[2]),
        //                    comentario2 = int.Parse(lineaNotas[3]),
        //                    observaciones = lineaNotas[4],
        //                    numeroFallas = int.Parse(lineaNotas[5])
        //                });

        //                estudiante = student.primerNombre + " " + (string.IsNullOrEmpty(student.segundoNombre) ? "" : (student.segundoNombre + " ")) +
        //                    student.primerApellido + (string.IsNullOrEmpty(student.segundoApellido) ? "" : (" " + student.segundoApellido));

        //                evaluation.lstNotas.Add(lineaNotas[0] + " - " + estudiante, lineaNotas[1]);
        //            }
        //            catch (Exception ex)
        //            {
        //                evaluation.lstNotas.Add("*** " + lineaNotas[0] + " - " + estudiante, lineaNotas[1] + "Error: " + ex.Message);
        //            }
        //        }


        //        reader.Close();

        //        try
        //        {
        //            if (System.IO.File.Exists(physicalPath + fileName))
        //                System.IO.File.Delete(physicalPath + fileName);
        //        }
        //        catch (Exception) { }
        //    }

        //    return View(evaluation);
        //}


        /****** Entrada Manual ********/
        [Authorize]
        public ActionResult EnterResult(string id)
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

            string[] evaluationKey = id.Split('|');

            EvaluationNoteBiz evaluationNoteBiz = new EvaluationNoteBiz();
            List<EvaluationNote> lsEvaluationNote = evaluationNoteBiz.GetEvaluationNoteList(school);

            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            CourseBiz courseBiz = new CourseBiz();
            Course course = courseBiz.GetCoursebyKey(new Course() { id = courseAsignature.idCurso });

            CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
            List<CourseStudent> lsCourseStudent = courseStudentBiz.GetCourseStudentList(new Course() { id = courseAsignature.idCurso });

            PersonBiz personBiz = new PersonBiz();
            List<Person> lsPerson = personBiz.GetPersonList(new Course() { id = courseAsignature.idCurso }, 5);

            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            List<AcademicPeriod> lsAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);
            AcademicPeriod academicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = int.Parse(evaluationKey[1]) });

            AsignatureBiz asignatureBiz = new AsignatureBiz();
            Asignature asignature = asignatureBiz.GetAsignaturebyKey(new Asignature() { id = courseAsignature.idAsignatura });

            List<Evaluation> lsEvaluation = evaluationBiz.GetEvaluationList(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            ViewBag.id = id + "|CURSO_" + course.Descripcion + "_ANO_" + course.ano + "_PERIODO_" + academicPeriod.Descripcion + "_MATERIA_" + asignature.Descripcion;
            ViewBag.idUpload = id;
            bool reevaluar = false;

            //ingreso de boletines en cero cuando el alumno no tenga registro en la tabla evaluation
            foreach (AcademicPeriod currentAcademicPeriod in lsAcademicPeriod)
            {
                int alumnosEvaluados = lsEvaluation.Where(x => x.idPeriodoAcademico.Equals(currentAcademicPeriod.id)).Count();

                if (alumnosEvaluados < lsCourseStudent.Count())
                {
                    reevaluar = true;
                    foreach (CourseStudent currentStudent in lsCourseStudent)
                    {
                        int alumnoEvaluado = lsEvaluation.Where(x => x.idEstudiante.Equals(currentStudent.id) && x.idPeriodoAcademico.Equals(currentAcademicPeriod.id)).Count();

                        if (alumnoEvaluado == 0)
                            evaluationBiz.SaveEvaluation(new Evaluation() { idPeriodoAcademico = currentAcademicPeriod.id, idEstudiante = currentStudent.id, idAsignatura = courseAsignature.id, Nota = 0, numeroFallas = 0 });
                    }
                }
            }

            if (reevaluar)
                lsEvaluation = evaluationBiz.GetEvaluationList(new CourseAsignature() { id = int.Parse(evaluationKey[0]) });

            List<vmEvaluation> lsStudentEvaluation = new List<vmEvaluation>();

            foreach (Evaluation item in lsEvaluation.Where(x => x.idPeriodoAcademico.Equals(int.Parse(evaluationKey[1]))).ToList())
            {
                int idEstudiante = lsCourseStudent.Where(x => x.id.Equals(item.idEstudiante)).Select(x => x.idEstudiante).First();
                Person currentStudent = lsPerson.Where(x => x.id.Equals(idEstudiante)).First();

                if (!currentStudent.retirado)
                {
                    string nombreAlumno = currentStudent.primerApellido + " " + (string.IsNullOrEmpty(currentStudent.segundoApellido) ? "" : currentStudent.segundoApellido + " ") +
                        currentStudent.primerNombre + (string.IsNullOrEmpty(currentStudent.segundoNombre) ? "" : " " + currentStudent.segundoNombre);

                    vmEvaluation currenEvaluation = new vmEvaluation()
                    {
                        idPeriodoAcademico = item.idPeriodoAcademico,
                        idAsignatura = item.idAsignatura,
                        idEstudiante = item.idEstudiante,
                        Nota = item.Nota,
                        desEstudiante = nombreAlumno,
                        lsEvaluationNotes = lsEvaluationNote,
                        numeroFallas = item.numeroFallas,
                        comentario1 = item.comentario1,
                        comentario2 = item.comentario2,
                        observaciones = item.observaciones,
                        ape1 = currentStudent.primerApellido,
                        ape2 = (string.IsNullOrEmpty(currentStudent.segundoApellido) ? "" : currentStudent.segundoApellido + " ")
                    };

                    lsStudentEvaluation.Add(currenEvaluation);
                }
            }

            return View(lsStudentEvaluation.OrderBy(x => x.ape1).ThenBy(x => x.ape2).ToList());
        }

        [Authorize]
        public JsonResult saveEvaluationResult(Evaluation evaluation)
        {
            string res = "";

            try
            {
                evaluationBiz.SaveEvaluation(evaluation);

                res = "OK";
            }
            catch (Exception)
            {
                res = "Error, verificar informacion";
            }


            return Json(res, JsonRequestBehavior.AllowGet);
        }


        /******Reporting*******/
        [Authorize]
        public ActionResult PuestosXInstitucion()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            foreach (int item in reporting.lsCurso.Select(x => x.ano).Distinct().ToList())
                reporting.lsAño.Add(item, item);

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult PuestosXInstitucionRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int año = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/PuestosXInstitucion.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue AñoDiscreteValue = new ParameterDiscreteValue();
                AñoDiscreteValue.Value = año;
                rpt.SetParameterValue("año", AñoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("idPeriodo", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult PuestosXCurso()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult PuestosXCursoRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int idCurso = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/PuestosXGrupo.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue CursoDiscreteValue = new ParameterDiscreteValue();
                CursoDiscreteValue.Value = idCurso;
                rpt.SetParameterValue("idCurso", CursoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("idPeriodo", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult EscalafonCursos()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            foreach (int item in reporting.lsCurso.Select(x => x.ano).Distinct().ToList())
                reporting.lsAño.Add(item, item);

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult EscalafonCursosRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int año = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/EscalafonCursos.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue AñoDiscreteValue = new ParameterDiscreteValue();
                AñoDiscreteValue.Value = año;
                rpt.SetParameterValue("año", AñoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("idPeriodo", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult DocentesCursoArea()
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

            CourseBiz courseBiz = new CourseBiz();


            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            foreach (int item in reporting.lsCurso.Select(x => x.ano).Distinct().ToList())
                reporting.lsAño.Add(item, item);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult DocentesCursoAreaRep(int id)
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

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/AsignacionDocentesGrupoArea.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue AñoDiscreteValue = new ParameterDiscreteValue();
                AñoDiscreteValue.Value = id;
                rpt.SetParameterValue("ano", AñoDiscreteValue);

                ParameterDiscreteValue colegioDiscreteValue = new ParameterDiscreteValue();
                colegioDiscreteValue.Value = idColegio;
                rpt.SetParameterValue("idColegio", colegioDiscreteValue);

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

        [Authorize]
        public ActionResult ProfesoresMayorMortalidad()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            foreach (int item in reporting.lsCurso.Select(x => x.ano).Distinct().ToList())
                reporting.lsAño.Add(item, item);

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        public FileContentResult ProfesoresMayorMortalidadRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int año = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/DocentesMayorMortalidad.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue AñoDiscreteValue = new ParameterDiscreteValue();
                AñoDiscreteValue.Value = año;
                rpt.SetParameterValue("año", AñoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("idPeriodo", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult imprimirBoletin()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(usuario).ToList();

            foreach (int item in reporting.lsCurso.Select(x => x.ano).Distinct().ToList())
                reporting.lsAño.Add(item, item);

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult imprimirBoletinRep(string id)
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

                string[] parametros = id.Split('|');

                int curso = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/evaluation.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);



                ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
                cursoDiscreteValue.Value = curso;
                rpt.SetParameterValue("curso", cursoDiscreteValue);

                ParameterDiscreteValue estudianteDiscreteValue = new ParameterDiscreteValue();
                estudianteDiscreteValue.Value = usuario;
                rpt.SetParameterValue("estudiante", estudianteDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("periodo", peirodoDiscreteValue);

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

        [Authorize]
        public FileContentResult DownloadNotasExcelRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int idAsignatura = int.Parse(parametros[0]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/PlanillaNotasBaja.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue idAsignaturaDiscreteValue = new ParameterDiscreteValue();
                idAsignaturaDiscreteValue.Value = idAsignatura;
                rpt.SetParameterValue("idAsignatura", idAsignaturaDiscreteValue);

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


                System.IO.MemoryStream strMemory = (System.IO.MemoryStream)rpt.ExportToStream(ExportFormatType.Excel);
                response = new byte[strMemory.Length];

                strMemory.Read(response, 0, (int)strMemory.Length);

                return new FileContentResult(response, "application/vnd.ms-excel");
            }
            catch (Exception ex)
            {
                string error = ex.Message + " :::---::: " + ex.StackTrace;

                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
                byte[] response = codificador.GetBytes(error);

                return new FileContentResult(response, "text/plain");
            }
        }


        [Authorize]
        public ActionResult NotasXCurso()
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

            CourseBiz courseBiz = new CourseBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = courseBiz.GetCourseList(school).Where(x => !x.finalizado).ToList();

            reporting.lsAdademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            return View(reporting);
        }

        [Authorize]
        public FileContentResult NotasXCursoRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int curso = int.Parse(parametros[0]);
                int idPeriodo = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/GlobalEvaluation.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
                cursoDiscreteValue.Value = curso;
                rpt.SetParameterValue("curso", cursoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idPeriodo;
                rpt.SetParameterValue("periodo", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult Certification()
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

            PlaceBiz placeBiz = new PlaceBiz();
            PersonBiz personBiz = new PersonBiz();

            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<vmPerson> lsVmPerson = new List<vmPerson>();
            List<Person> lsPerson = personBiz.GetPersonList(school, 5);

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto = item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido) +
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " "),

                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Retirado" : "Activo"
                };

                switch (item.idJornada)
                {
                    case 1:
                        person.nombreJornada = "Mañana";
                        break;
                    case 2:
                        person.nombreJornada = "Tarde";
                        break;
                    case 3:
                        person.nombreJornada = "Noche";
                        break;
                    case 4:
                        person.nombreJornada = "Única";
                        break;
                    default:
                        break;
                }

                person.nombreSede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson);
        }

        [Authorize]
        public ActionResult CertificationLookUp(int id)
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

            ViewBag.idEstudiante = id;

            CourseBiz courseBiz = new CourseBiz();
            List<Course> lsCursosColegios = courseBiz.GetCourseList(id).Where(x => !x.finalizado).ToList();

            vmReporting reporting = new vmReporting();
            reporting.lsCurso = lsCursosColegios.OrderBy(x => x.ano).ToList();

            return View(reporting);
        }

        [Authorize]
        public FileContentResult CertificactionRep(string id)
        {
            try
            {
                string[] parametros = id.Split('|');

                int idEstudiante = int.Parse(parametros[0]);
                int idCurso = int.Parse(parametros[1]);

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/Certificacion.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
                cursoDiscreteValue.Value = idCurso;
                rpt.SetParameterValue("idCurso", cursoDiscreteValue);

                ParameterDiscreteValue peirodoDiscreteValue = new ParameterDiscreteValue();
                peirodoDiscreteValue.Value = idEstudiante;
                rpt.SetParameterValue("idEstudiante", peirodoDiscreteValue);

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

        [Authorize]
        public ActionResult InsuficiencesAdminIndex()
        {
            SchoolBiz schoolBiz = new SchoolBiz();

            vmEvaluation oEvaluation = new vmEvaluation();
            oEvaluation.lstColegio = schoolBiz.GetSchoolList();

            return View(oEvaluation);
        }

        [Authorize]
        public ActionResult InsuficiencesSchoolIndex()
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

            ViewBag.idColegio = school.id;
            vmEvaluation oEvaluation = new vmEvaluation();

            return View(oEvaluation);
        }

        [Authorize]
        public ActionResult InsuficiencesRep(string id)
        {
            DsCrossReports ds = calculoReporteInsuficiencias(id);

            //string[] parametros = id.Split('|');

            //int idEstudiante = int.Parse(parametros[0]);
            //int idCurso = int.Parse(parametros[1]);

            //string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
            string rutaRpt = Server.MapPath("~/Reporting/Insuficiencias.rpt");

            ReportDocument rpt;

            byte[] response = null;


            rpt = new ReportDocument();
            rpt.Load(rutaRpt);
            rpt.SetDataSource(ds.Tables[0]);





            System.IO.MemoryStream strMemory = (System.IO.MemoryStream)rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            response = new byte[strMemory.Length];

            strMemory.Read(response, 0, (int)strMemory.Length);

            return new FileContentResult(response, "application/pdf");
        }

        private DsCrossReports calculoReporteInsuficiencias(string id)
        {

            CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
            ValuationLevelBiz valuationLevelBiz = new ValuationLevelBiz();
            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            CourseStudentBiz courseStudentBiz = new CourseStudentBiz();
            AsignatureBiz asignatureBiz = new AsignatureBiz();
            CourseBiz courseBiz = new CourseBiz();
            PersonBiz personBiz = new PersonBiz();
            SchoolBiz schoolBiz = new SchoolBiz();
            PlaceBiz placeBiz = new PlaceBiz();

            try
            {
                string[] entrada = id.Split('|');
                int idCurso = int.Parse(entrada[0]);
                int idPeriodo = int.Parse(entrada[1]);
                int idReporte = int.Parse(entrada[2]);

                AcademicPeriod academicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { id = idPeriodo });
                Course course = courseBiz.GetCoursebyKey(new Course() { id = idCurso });
                School school = schoolBiz.GetSchoolbyKey(new School() { id = course.idColegio });
                Place place = placeBiz.GetPlaceByKey(new Place() { id = course.idSede });
                Person dirCurso = personBiz.GetPersonByKey(new Person() { id = course.idDirectorCurso });

                string jornada = course.idJornada == 1 ? "Mañana" : (course.idJornada == 2 ? "Tarde" : (course.idJornada == 3 ? "Noche" : (course.idJornada == 4 ? "Única" : "mañana")));

                string directorCurso =
                        dirCurso.primerNombre + " " + (string.IsNullOrEmpty(dirCurso.segundoNombre) ? "" : dirCurso.segundoNombre + " ") +
                        dirCurso.primerApellido + (string.IsNullOrEmpty(dirCurso.segundoApellido) ? "" : " " + dirCurso.segundoApellido);

                List<Evaluation> lsEvaluation = new List<Evaluation>();
                List<Asignature> lsAsignature = asignatureBiz.GetAsignatureList(school);
                List<CourseAsignature> lsCourseAsignature = courseAsignatureBiz.GetCourseAsignatureList(course);

                List<Person> lsStudents = personBiz.GetPersonList(course, 5).OrderBy(x => x.primerApellido).ThenBy(x => x.segundoApellido).ToList();
                List<CourseStudent> lsCourseSTudent = courseStudentBiz.GetCourseStudentList(course);
                List<ValuationLevel> lsValuationLevel = valuationLevelBiz.GetValuationLevelList(school);

                lsEvaluation = evaluationBiz.GetEvaluationList(course).Where(x => x.idPeriodoAcademico.Equals(idPeriodo)).ToList();

                List<int> lsCourseAsignatureCode = lsEvaluation.Select(x => x.idAsignatura).Distinct().ToList();
                List<int> lsCourseStudentCode = new List<int>();//lsEvaluation.Select(x => x.idEstudiante).Distinct().ToList();

                foreach (Person item in lsStudents)
                    lsCourseStudentCode.Add(lsCourseSTudent.Where(x => x.idEstudiante.Equals(item.id)).Select(x => x.id).FirstOrDefault());

                DsCrossReports dsCrossReports = new DsCrossReports();
                DsCrossReports dsTmpCrossReports = new DsCrossReports();

                #region Titulo
                DsCrossReports.RptTableRow titleRow = dsCrossReports.RptTable.NewRptTableRow();
                DsCrossReports.RptTableRow teacherRow = dsCrossReports.RptTable.NewRptTableRow();

                titleRow.num = "Num";
                titleRow.alumno = "Alumno";
                titleRow.total = "TAP";
                titleRow.colegio = school.nombreColegio;
                titleRow.sede = place.descripcion;
                titleRow.periodo = academicPeriod.Descripcion;
                titleRow.curso = course.Descripcion + " - " + course.ano;
                titleRow.profesor = directorCurso;
                titleRow.jornada = jornada;
                titleRow.tipoReporte = idReporte.ToString();


                #region descripcion por materia

                for (int i = 0; i < lsCourseAsignatureCode.Count; i++)
                {
                    CourseAsignature courseAsignature = lsCourseAsignature.Where(x => x.id.Equals(lsCourseAsignatureCode[i])).FirstOrDefault();
                    Asignature asignature = asignatureBiz.GetAsignaturebyKey(new Asignature() { id = courseAsignature.idAsignatura });

                    Person person = personBiz.GetPersonByKey(new Person() { id = (int)courseAsignature.idProfesor });
                    string profesor =
                        person.primerNombre + " " + (string.IsNullOrEmpty(person.segundoNombre) ? "" : person.segundoNombre + " ") +
                        person.primerApellido + (string.IsNullOrEmpty(person.segundoApellido) ? "" : " " + person.segundoApellido);

                    switch (i)
                    {
                        case 0:
                            titleRow.mat1 = asignature.Descripcion;
                            teacherRow.mat1 = profesor;
                            break;
                        case 1:
                            titleRow.mat2 = asignature.Descripcion;
                            teacherRow.mat2 = profesor;
                            break;
                        case 2:
                            titleRow.mat3 = asignature.Descripcion;
                            teacherRow.mat3 = profesor;
                            break;
                        case 3:
                            titleRow.mat4 = asignature.Descripcion;
                            teacherRow.mat4 = profesor;
                            break;
                        case 4:
                            titleRow.mat5 = asignature.Descripcion;
                            teacherRow.mat5 = profesor;
                            break;
                        case 5:
                            titleRow.mat6 = asignature.Descripcion;
                            teacherRow.mat6 = profesor;
                            break;
                        case 6:
                            titleRow.mat7 = asignature.Descripcion;
                            teacherRow.mat7 = profesor;
                            break;
                        case 7:
                            titleRow.mat8 = asignature.Descripcion;
                            teacherRow.mat8 = profesor;
                            break;
                        case 8:
                            titleRow.mat9 = asignature.Descripcion;
                            teacherRow.mat9 = profesor;
                            break;
                        case 9:
                            titleRow.mat10 = asignature.Descripcion;
                            teacherRow.mat10 = profesor;
                            break;
                        case 10:
                            titleRow.mat11 = asignature.Descripcion;
                            teacherRow.mat11 = profesor;
                            break;
                        case 11:
                            titleRow.mat12 = asignature.Descripcion;
                            teacherRow.mat12 = profesor;
                            break;
                        case 12:
                            titleRow.mat13 = asignature.Descripcion;
                            teacherRow.mat13 = profesor;
                            break;
                        case 13:
                            titleRow.mat14 = asignature.Descripcion;
                            teacherRow.mat14 = profesor;
                            break;
                        case 14:
                            titleRow.mat15 = asignature.Descripcion;
                            teacherRow.mat15 = profesor;
                            break;
                        case 15:
                            titleRow.mat16 = asignature.Descripcion;
                            teacherRow.mat16 = profesor;
                            break;
                        case 16:
                            titleRow.mat17 = asignature.Descripcion;
                            teacherRow.mat17 = profesor;
                            break;
                        case 17:
                            titleRow.mat18 = asignature.Descripcion;
                            teacherRow.mat18 = profesor;
                            break;
                        case 18:
                            titleRow.mat19 = asignature.Descripcion;
                            teacherRow.mat19 = profesor;
                            break;
                        case 19:
                            titleRow.mat20 = asignature.Descripcion;
                            teacherRow.mat20 = profesor;
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                dsCrossReports.RptTable.AddRptTableRow(titleRow);
                #endregion

                #region detalle bruto

                DsCrossReports.RptTableRow insufCountRow = dsCrossReports.RptTable.NewRptTableRow();
                for (int i = 0; i < 28; i++)
                    insufCountRow[i] = i < 7 ? "" : ((i >= 7 && i < 27) ? "0" : "");

                foreach (int item in lsCourseStudentCode)
                {
                    CourseStudent courseStudent = lsCourseSTudent.Where(x => x.id.Equals(item)).FirstOrDefault();
                    Person student = lsStudents.Where(x => x.id.Equals(courseStudent.idEstudiante)).FirstOrDefault();

                    DsCrossReports.RptTableRow detailRow = dsTmpCrossReports.RptTable.NewRptTableRow();

                    //titleRow.num = "-1";
                    detailRow.alumno =
                        student.primerApellido + (string.IsNullOrEmpty(student.segundoApellido) ? "" : " " + student.segundoApellido) +
                        student.primerNombre + " " + (string.IsNullOrEmpty(student.segundoNombre) ? "" : student.segundoNombre + " ");

                    detailRow.colegio = school.nombreColegio;
                    detailRow.sede = place.descripcion;
                    detailRow.periodo = academicPeriod.Descripcion;
                    detailRow.curso = course.Descripcion;
                    detailRow.profesor = directorCurso;
                    detailRow.jornada = jornada;
                    detailRow.tipoReporte = idReporte.ToString();

                    int TAP = 0;

                    for (int i = 0; i < lsCourseAsignatureCode.Count; i++)
                    {
                        #region Valor de nota segun reporte
                        Evaluation evaluation = lsEvaluation.Where(x => x.idAsignatura.Equals(lsCourseAsignatureCode[i]) && x.idEstudiante.Equals(item)).FirstOrDefault();
                        string nota = "";

                        bool pierde = false;

                        if (idReporte == 1)
                        {
                            pierde = lsValuationLevel.Where(x => evaluation.Nota >= x.minimo && evaluation.Nota <= x.maximo).Select(x => x.noSupera).FirstOrDefault();

                            if (pierde)
                            {
                                nota = "[" + evaluation.Nota.ToString() + "]";
                                TAP++;
                            }
                            else
                                nota = evaluation.Nota != null ? evaluation.Nota.ToString() : "[  ]";
                        }
                        else if (idReporte > 1)
                        {
                            pierde = lsValuationLevel.Where(x => evaluation.Nota >= x.minimo && evaluation.Nota <= x.maximo).Select(x => x.noSupera).FirstOrDefault();

                            if (pierde)
                            {
                                nota = evaluation.Nota.ToString();
                                TAP++;
                            }
                            else
                                nota = "";
                        }

                        #endregion

                        #region Nota por materia
                        switch (i)
                        {
                            case 0:
                                detailRow.mat1 = nota;
                                insufCountRow.mat1 = pierde ? (decimal.Parse(insufCountRow.mat1) + 1).ToString() : insufCountRow.mat1;
                                break;
                            case 1:
                                detailRow.mat2 = nota;
                                insufCountRow.mat2 = pierde ? (decimal.Parse(insufCountRow.mat2) + 1).ToString() : insufCountRow.mat2;
                                break;
                            case 2:
                                detailRow.mat3 = nota;
                                insufCountRow.mat3 = pierde ? (decimal.Parse(insufCountRow.mat3) + 1).ToString() : insufCountRow.mat3;
                                break;
                            case 3:
                                detailRow.mat4 = nota;
                                insufCountRow.mat4 = pierde ? (decimal.Parse(insufCountRow.mat4) + 1).ToString() : insufCountRow.mat4;
                                break;
                            case 4:
                                detailRow.mat5 = nota;
                                insufCountRow.mat5 = pierde ? (decimal.Parse(insufCountRow.mat5) + 1).ToString() : insufCountRow.mat5;
                                break;
                            case 5:
                                detailRow.mat6 = nota;
                                insufCountRow.mat6 = pierde ? (decimal.Parse(insufCountRow.mat6) + 1).ToString() : insufCountRow.mat6;
                                break;
                            case 6:
                                detailRow.mat7 = nota;
                                insufCountRow.mat7 = pierde ? (decimal.Parse(insufCountRow.mat7) + 1).ToString() : insufCountRow.mat7;
                                break;
                            case 7:
                                detailRow.mat8 = nota;
                                insufCountRow.mat8 = pierde ? (decimal.Parse(insufCountRow.mat8) + 1).ToString() : insufCountRow.mat8;
                                break;
                            case 8:
                                detailRow.mat9 = nota;
                                insufCountRow.mat9 = pierde ? (decimal.Parse(insufCountRow.mat9) + 1).ToString() : insufCountRow.mat9;
                                break;
                            case 9:
                                detailRow.mat10 = nota;
                                insufCountRow.mat10 = pierde ? (decimal.Parse(insufCountRow.mat10) + 1).ToString() : insufCountRow.mat10;
                                break;
                            case 10:
                                detailRow.mat11 = nota;
                                insufCountRow.mat11 = pierde ? (decimal.Parse(insufCountRow.mat11) + 1).ToString() : insufCountRow.mat11;
                                break;
                            case 11:
                                detailRow.mat12 = nota;
                                insufCountRow.mat12 = pierde ? (decimal.Parse(insufCountRow.mat12) + 1).ToString() : insufCountRow.mat12;
                                break;
                            case 12:
                                detailRow.mat13 = nota;
                                insufCountRow.mat13 = pierde ? (decimal.Parse(insufCountRow.mat13) + 1).ToString() : insufCountRow.mat13;
                                break;
                            case 13:
                                detailRow.mat14 = nota;
                                insufCountRow.mat14 = pierde ? (decimal.Parse(insufCountRow.mat14) + 1).ToString() : insufCountRow.mat14;
                                break;
                            case 14:
                                detailRow.mat15 = nota;
                                insufCountRow.mat15 = pierde ? (decimal.Parse(insufCountRow.mat15) + 1).ToString() : insufCountRow.mat15;
                                break;
                            case 15:
                                detailRow.mat16 = nota;
                                insufCountRow.mat16 = pierde ? (decimal.Parse(insufCountRow.mat16) + 1).ToString() : insufCountRow.mat16;
                                break;
                            case 16:
                                detailRow.mat17 = nota;
                                insufCountRow.mat17 = pierde ? (decimal.Parse(insufCountRow.mat17) + 1).ToString() : insufCountRow.mat17;
                                break;
                            case 17:
                                detailRow.mat18 = nota;
                                insufCountRow.mat18 = pierde ? (decimal.Parse(insufCountRow.mat18) + 1).ToString() : insufCountRow.mat18;
                                break;
                            case 18:
                                detailRow.mat19 = nota;
                                insufCountRow.mat19 = pierde ? (decimal.Parse(insufCountRow.mat19) + 1).ToString() : insufCountRow.mat19;
                                break;
                            case 19:
                                detailRow.mat20 = nota;
                                insufCountRow.mat20 = pierde ? (decimal.Parse(insufCountRow.mat20) + 1).ToString() : insufCountRow.mat20;
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }

                    int numMateriasActivas = lsCourseAsignatureCode.Count;
                    int numMateriasInactivas = 20 - numMateriasActivas;

                    for (int i = ((28 - numMateriasInactivas) - 1); i < 28; i++)
                    {
                        detailRow[i] = idReporte == 1 ? "[  ]" : ""; ;
                    }

                    detailRow.total = TAP.ToString();
                    detailRow.num = (dsTmpCrossReports.RptTable.Rows.Count + 1).ToString();

                    dsTmpCrossReports.RptTable.AddRptTableRow(detailRow);
                }
                #endregion

                #region Detalle formateado y ordenado
                List<DsCrossReports.RptTableRow> filas = new List<DsCrossReports.RptTableRow>(); ;

                if (idReporte == 1 || idReporte == 3)
                    filas = dsTmpCrossReports.RptTable.ToList();
                else if (idReporte == 2)
                    filas = dsTmpCrossReports.RptTable.OrderBy(x => x.total).ToList();

                int num = 1;

                foreach (DsCrossReports.RptTableRow item in filas)
                {
                    DsCrossReports.RptTableRow detailRow = dsCrossReports.RptTable.NewRptTableRow();

                    if (idReporte == 1 || idReporte == 3)
                        detailRow.num = item.num;
                    else if (idReporte == 2)
                        detailRow.num = num.ToString();

                    detailRow.sede = item.sede;
                    detailRow.periodo = item.periodo;
                    detailRow.profesor = item.profesor;
                    detailRow.alumno = item.alumno;
                    detailRow.colegio = item.colegio;
                    detailRow.curso = item.curso;
                    detailRow.mat1 = item.mat1;
                    detailRow.mat2 = item.mat2;
                    detailRow.mat3 = item.mat3;
                    detailRow.mat4 = item.mat4;
                    detailRow.mat5 = item.mat5;
                    detailRow.mat6 = item.mat6;
                    detailRow.mat7 = item.mat7;
                    detailRow.mat8 = item.mat8;
                    detailRow.mat9 = item.mat9;
                    detailRow.mat10 = item.mat10;
                    detailRow.mat11 = item.mat11;
                    detailRow.mat12 = item.mat12;
                    detailRow.mat13 = item.mat13;
                    detailRow.mat14 = item.mat14;
                    detailRow.mat15 = item.mat15;
                    detailRow.mat16 = item.mat16;
                    detailRow.mat17 = item.mat17;
                    detailRow.mat18 = item.mat18;
                    detailRow.mat19 = item.mat19;
                    detailRow.mat20 = item.mat20;

                    detailRow.total = item.total;
                    detailRow.jornada = item.jornada;
                    detailRow.tipoReporte = item.tipoReporte;
                    dsCrossReports.RptTable.AddRptTableRow(detailRow);
                    num++;
                }

                #endregion

                teacherRow.num = "999";
                insufCountRow.alumno = "TOTALES      Total grupo = " + lsCourseStudentCode.Count.ToString() + "      Total insuficiencias:";
                teacherRow.alumno = "Profesores";

                dsCrossReports.RptTable.AddRptTableRow(insufCountRow);
                dsCrossReports.RptTable.AddRptTableRow(teacherRow);

                //int y = 0;

                return dsCrossReports;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //        [Authorize]
        //        public ReportResult ShowResultsPdf(string id)
        //        {
        //            #region School identification
        //            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //            int idColegio = 0;
        //            int usuario = 0;

        //            if (context.IsAuthenticated)
        //            {
        //                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //                string[] userRole = ci.Ticket.UserData.Split('|');
        //                usuario = int.Parse(userRole[0]);
        //                idColegio = int.Parse(userRole[2]);
        //            }

        //            School school = new School() { id = idColegio };
        //            #endregion

        //            EvaluationBiz evaluationBiz = new EvaluationBiz();
        //            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
        //            StudentBiz studentBiz = new StudentBiz();
        //            TeacherBiz teacherBiz = new TeacherBiz();
        //            AssingatureBiz assignatureBiz = new AssingatureBiz();
        //            CourseBiz courseBiz = new CourseBiz();
        //            SchoolBiz schoolBiz = new SchoolBiz();

        //            string[] parametros = id.Split('-');

        //            int idCurso = int.Parse(parametros[0]);
        //            int idPeriodoAcademico = int.Parse(parametros[1]);

        //            List<vmEvaluation> lsEvaluationResult = new List<vmEvaluation>();

        //            Course course = courseBiz.GetCoursebyKey(new Course() { id = idCurso });
        //            AcademicPeriod AcademicPeriod = academicPeriodBiz.GetAcademicPeriodbyKey(new AcademicPeriod() { idColegio = school.id, id = idPeriodoAcademico });
        //            Student student = studentBiz.GetStudentbyKey(new Student() { idColegio = school.id, id = usuario });
        //            Teacher teacher = teacherBiz.GetTeacherbyKey(new Teacher() { id = course.idDirectorCurso, idColegio = school.id });
        //            school = schoolBiz.GetSchoolbyKey(school);

        //            Evaluation evaluation = new Evaluation()
        //            {
        //                idColegio = school.id,
        //                idCurso = course.id,
        //                idPeriodoAcademico = idPeriodoAcademico,
        //                idEstudiante = usuario
        //            };

        //            List<Evaluation> lsEvaluation = evaluationBiz.GetEvaluationResult(evaluation);

        //            foreach (Evaluation item in lsEvaluation)
        //            {
        //                Assingnature assignature = assignatureBiz.GetAssingnaturebyKey(new Assingnature()
        //                {
        //                    id = item.idAsignatura,
        //                    idColegio = school.id,
        //                    idGrado = course.idGrado
        //                });

        //                lsEvaluationResult.Add(new vmEvaluation()
        //                {
        //                    desAsignatura = assignature.Descripcion,
        //                    Nota = item.Nota
        //                });
        //            }

        //            student.primerNombre = student.primerNombre + " " + (string.IsNullOrEmpty(student.segundoNombre) ? "" : (student.segundoNombre + " ")) +
        //                                student.primerApellido + (string.IsNullOrEmpty(student.segundoApellido) ? "" : (" " + student.segundoApellido));

        //            teacher.primerNombre = teacher.primerNombre + " " + (string.IsNullOrEmpty(teacher.segundoNombre) ? "" : (teacher.segundoNombre + " ")) +
        //                                teacher.primerApellido + (string.IsNullOrEmpty(teacher.segundoApellido) ? "" : (" " + teacher.segundoApellido));

        //            var report = new Report(lsEvaluationResult.ToReportSource());

        //            report.TextFields.Title = school.NombreColegio.ToUpper();
        //            report.TextFields.SubTitle = "BOLETÍN DE NOTAS";


        //            report.TextFields.Header = string.Format(@"
        //________________________________________________________________________________________
        //
        //Periodo: {0}
        //Curso: {1}
        //Director de grupo: {2}
        //Alumno: {3:c}
        //________________________________________________________________________________________"
        //                , AcademicPeriod.Descripcion.ToUpper(), course.Descripcion, teacher.primerNombre, student.primerNombre);

        //            report.RenderHints.BooleanCheckboxes = true;

        //            report.DataFields["idColegio"].Hidden = true;
        //            report.DataFields["desColegio"].Hidden = true;
        //            report.DataFields["idCurso"].Hidden = true;
        //            report.DataFields["desCurso"].Hidden = true;
        //            report.DataFields["idGrado"].Hidden = true;
        //            report.DataFields["idEstudiante"].Hidden = true;
        //            report.DataFields["desEstudiante"].Hidden = true;
        //            report.DataFields["idProfesor"].Hidden = true;
        //            report.DataFields["idAsignatura"].Hidden = true;
        //            report.DataFields["idPeriodoAcademico"].Hidden = true;
        //            report.DataFields["desPeriodoAcademico"].Hidden = true;
        //            report.DataFields["lstColegio"].Hidden = true;
        //            report.DataFields["lstProfesor"].Hidden = true;
        //            report.DataFields["ano"].Hidden = true;
        //            report.DataFields["lstCurso"].Hidden = true;
        //            report.DataFields["lstAsignatura"].Hidden = true;
        //            report.DataFields["lstNotas"].Hidden = true;
        //            report.DataFields["desAsignatura"].HeaderText = "Asignatrura";

        //            return new ReportResult(report);
        //        }

        //        [Authorize]
        //        public ActionResult ShowResults(int curso)
        //        {
        //            #region School identification
        //            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //            int idColegio = 0;
        //            int usuario = 0;

        //            if (context.IsAuthenticated)
        //            {
        //                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //                string[] userRole = ci.Ticket.UserData.Split('|');
        //                usuario = int.Parse(userRole[0]);
        //                idColegio = int.Parse(userRole[2]);
        //            }

        //            School school = new School() { id = idColegio };
        //            #endregion

        //            CourseStudent courseStudent = new CourseStudent() { idColegio = idColegio, idCurso = curso, idEstudiante = usuario };

        //            return View(courseStudent);
        //        }

        //        [Authorize]
        //        public FileContentResult getEvaluationResults(int curso)
        //        {

        //            #region School identification
        //            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //            int idColegio = 0;
        //            int usuario = 0;

        //            if (context.IsAuthenticated)
        //            {
        //                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //                string[] userRole = ci.Ticket.UserData.Split('|');
        //                usuario = int.Parse(userRole[0]);
        //                idColegio = int.Parse(userRole[2]);
        //            }

        //            School school = new School() { id = idColegio };
        //            #endregion

        //            EvaluationBiz evaluationBiz = new EvaluationBiz();

        //            string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
        //            try
        //            {
        //                byte[] response = boletin(curso, usuario, Server.MapPath("~/Reporting/Evaluation.rpt"), oConnStr);
        //                return new FileContentResult(response, "application/pdf");
        //            }
        //            catch (Exception ex)
        //            {
        //                string error = ex.Message + " :::---::: " + oConnStr + " :::---::: " + ex.StackTrace;

        //                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
        //                byte[] response = codificador.GetBytes(error);

        //                return new FileContentResult(response, "text/plain");
        //            }


        //        }


        //        public byte[] boletin(int curso, int estudiante, string rutaRpt, string oConnStr)
        //        {
        //            ReportDocument rpt;
        //            byte[] response = null;

        //            System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

        //            rpt = new ReportDocument();

        //            rpt.Load(rutaRpt);

        //            //rpt.SetParameterValue("curso", curso);
        //            //rpt.SetParameterValue("estudiante", estudiante);

        //            ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
        //            cursoDiscreteValue.Value = curso;
        //            rpt.SetParameterValue("curso", cursoDiscreteValue);

        //            ParameterDiscreteValue estudianteDiscreteValue = new ParameterDiscreteValue();
        //            estudianteDiscreteValue.Value = estudiante;
        //            rpt.SetParameterValue("estudiante", estudianteDiscreteValue);


        //            CrystalDecisions.Shared.ConnectionInfo connectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
        //            connectionInfo.DatabaseName = oConnBuilder.InitialCatalog;
        //            connectionInfo.UserID = oConnBuilder.UserID;
        //            connectionInfo.Password = oConnBuilder.Password;
        //            connectionInfo.ServerName = oConnBuilder.DataSource;

        //            Tables tables = rpt.Database.Tables;
        //            foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
        //            {
        //                CrystalDecisions.Shared.TableLogOnInfo tableLogonInfo = table.LogOnInfo;
        //                tableLogonInfo.ConnectionInfo = connectionInfo;
        //                table.ApplyLogOnInfo(tableLogonInfo);
        //            }

        //            for (int i = 0; i < rpt.DataSourceConnections.Count; i++)
        //            {
        //                rpt.DataSourceConnections[i].SetConnection(oConnBuilder.DataSource, oConnBuilder.InitialCatalog, oConnBuilder.UserID, oConnBuilder.Password);
        //            }

        //            rpt.SetDatabaseLogon(oConnBuilder.UserID, oConnBuilder.Password, oConnBuilder.DataSource, oConnBuilder.InitialCatalog);


        //            System.IO.MemoryStream strMemory = (System.IO.MemoryStream)rpt.ExportToStream(ExportFormatType.PortableDocFormat); 
        //            response = new byte[strMemory.Length];

        //            strMemory.Read(response, 0, (int)strMemory.Length);
        //            return response;
        //        }
    }
}
