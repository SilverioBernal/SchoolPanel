using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class StudentLogBiz
    {
        /*CRUD StudentLogs*/

        /// <summary>
        /// Retrieve StudentLogs list without parameters
        /// </summary>
        /// <returns></returns>
        public List<StudentLog> GetStudentLogList()
        {

            List<StudentLog> lstStudentLog = new List<StudentLog>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstStudentLog = ctx.StudentLogs.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentLog;
        }

        /// <summary>
        /// Retrieve StudentLog information based in the primary key
        /// </summary>
        /// <param name="StudentLogTarget"></param>
        /// <returns></returns>
        public StudentLog GetStudentLogbyKey(StudentLog StudentLogTarget)
        {
            StudentLog oStudentLog = new StudentLog();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oStudentLog = ctx.StudentLogs.Where(x => x.id.Equals(StudentLogTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oStudentLog;
        }

        /// <summary>
        /// Create or update a StudentLog
        /// </summary>
        /// <param name="StudentLogTarget"></param>
        public void SaveStudentLog(StudentLog StudentLogTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the StudentLog exists
                    StudentLog oStudentLog = GetStudentLogbyKey(StudentLogTarget);

                    if (oStudentLog != null)
                    {
                        // if exists then edit 
                        ctx.StudentLogs.Attach(oStudentLog);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oStudentLog, StudentLogTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.StudentLogs.Add(StudentLogTarget);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (DbEntityValidationException e)
            {
                StringBuilder oError = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    oError.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        oError.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                string msg = oError.ToString();
                throw new Exception(msg);
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a StudentLog
        /// </summary>
        /// <param name="StudentLogTarget"></param>
        public void DeleteStudentLog(StudentLog StudentLogTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    StudentLog oStudentLog = GetStudentLogbyKey(StudentLogTarget);

                    if (oStudentLog != null)
                    {
                        // if exists then edit 
                        ctx.StudentLogs.Attach(oStudentLog);
                        ctx.StudentLogs.Remove(oStudentLog);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar este registro del observador porque existe información asociada a este.");
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<StudentLog> GetStudentLogList(Person estudiante)
        {

            List<StudentLog> lstStudentLog = new List<StudentLog>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstStudentLog = ctx.StudentLogs.Where(X => X.idEstudiante.Equals(estudiante.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentLog;
        }
    }
}
