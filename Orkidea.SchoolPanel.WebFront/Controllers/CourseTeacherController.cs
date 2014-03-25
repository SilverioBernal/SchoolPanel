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
    public class CourseTeacherController : Controller
    {
        CourseAsignatureBiz courseTeacherBiz = new CourseAsignatureBiz();

        //
        // GET: /CourseTeacher/
        [Authorize]
        public ActionResult Index()
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

            PersonBiz personBiz = new PersonBiz();

            AsignatureBiz asignatureBiz = new AsignatureBiz();
            List<Asignature> lsAsignatura = asignatureBiz.GetAsignatureList(school);

            CourseBiz courseBiz = new CourseBiz();
            List<Course> lsCourse = courseBiz.GetCourseList(new Person() { id = usuario }).Where(x => !x.finalizado).OrderBy(x => x.id).ToList();            
            List<int> lsIdCurso = lsCourse.Select(x => x.id).ToList();

            List<CourseAsignature> lsCourseAsignature = courseTeacherBiz.GetCourseAsignatureList(new Person() { id = usuario }).Where(x => lsIdCurso.Contains(x.idCurso)).ToList();
            List<vmCourseAsignature> lsAsignaturas = new List<vmCourseAsignature>();

            AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
            List<AcademicPeriod> lsAcademicPeriod = academicPeriodBiz.GetAcademicPeriodList(school);

            foreach (CourseAsignature item in lsCourseAsignature)
            {
                vmCourseAsignature currentAsignature = new vmCourseAsignature()
                {
                    id = item.id,
                    idCurso = item.idCurso,
                    idAsignatura = item.idAsignatura,
                    idProfesor = item.idProfesor,
                    asignatura = lsAsignatura.Where(x => x.id.Equals(item.idAsignatura)).Select(x => x.Descripcion).First(),
                    curso = lsCourse.Where(x => x.id.Equals(item.idCurso)).Select(x => x.Descripcion).First(),
                    lsAcademicPeriod = lsAcademicPeriod
                };

                lsAsignaturas.Add(currentAsignature);
            }

            return View(lsAsignaturas);
        }


        /* Reportes*/

        [Authorize]
        public FileContentResult PlanillaAuxiliar(int id)
        {
            try
            {
                CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = id });

                int idCurso = courseAsignature.idCurso;
                int idProfesor = (int)courseAsignature.idProfesor;

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/PlanillaAuxiliarCalificaciones.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
                cursoDiscreteValue.Value = idCurso;
                rpt.SetParameterValue("idCurso", cursoDiscreteValue);

                ParameterDiscreteValue profesorDiscreteValue = new ParameterDiscreteValue();
                profesorDiscreteValue.Value = idProfesor;
                rpt.SetParameterValue("idProfesor", profesorDiscreteValue);

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
        public FileContentResult PlanillaResumen(int id)
        {
            try
            {
                CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                CourseAsignature courseAsignature = courseAsignatureBiz.GetCourseAsignatureByKey(new CourseAsignature() { id = id });

                //int idCurso = courseAsignature.idCurso;
                //int idProfesor = (int)courseAsignature.idProfesor;

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/PlanillaResumen.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue cursoDiscreteValue = new ParameterDiscreteValue();
                cursoDiscreteValue.Value = id;
                rpt.SetParameterValue("idAsignaturaCurso", cursoDiscreteValue);              

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
    }
}
