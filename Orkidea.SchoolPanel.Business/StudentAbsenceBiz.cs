
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class StudentAbsenceBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<StudentAbsence> GetStudentAbsenceList()
        {

            List<StudentAbsence> lstStudentAbsence = new List<StudentAbsence>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstStudentAbsence = ctx.StudentAbsences.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentAbsence;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="studentAbsenceTarget"></param>
        /// <returns></returns>
        public StudentAbsence GetStudentAbsenceByKey(StudentAbsence studentAbsenceTarget)
        {
            StudentAbsence oStudentAbsence = new StudentAbsence();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oStudentAbsence = ctx.StudentAbsences.Where(x => x.id.Equals(studentAbsenceTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oStudentAbsence;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="studentAbsenceTarget"></param>
        public void SaveStudentAbsence(StudentAbsence studentAbsenceTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    StudentAbsence oStudentAbsence = GetStudentAbsenceByKey(studentAbsenceTarget);

                    if (oStudentAbsence != null)
                    {
                        // if exists then edit 
                        ctx.StudentAbsences.Attach(oStudentAbsence);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oStudentAbsence, studentAbsenceTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.StudentAbsences.Add(studentAbsenceTarget);
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
        /// Delete a record
        /// </summary>
        /// <param name="studentAbsenceTarget"></param>
        public void DeleteStudentAbsence(StudentAbsence studentAbsenceTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    StudentAbsence oStudentAbsence = GetStudentAbsenceByKey(studentAbsenceTarget);

                    if (oStudentAbsence != null)
                    {
                        // if exists then edit 
                        ctx.StudentAbsences.Attach(oStudentAbsence);
                        ctx.StudentAbsences.Remove(oStudentAbsence);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }


        /*Complementary business methods*/

        public List<StudentAbsence> GetStudentAbsenceList(School schoolTarget)
        {

            List<StudentAbsence> lstStudentAbsence = new List<StudentAbsence>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    CourseBiz courseBiz = new CourseBiz();
                    List<int> lsCourse = courseBiz.GetCourseList(schoolTarget).Select(x => x.id).Distinct().ToList();

                    CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                    List<int> lsCurseAsignature = courseAsignatureBiz.GetCourseAsignatureList().Where(x => lsCourse.Contains(x.idCurso)).Select(x => x.id).Distinct().ToList();

                    lstStudentAbsence = ctx.StudentAbsences.Where(x => lsCurseAsignature.Contains(x.idAsignatura)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentAbsence;
        }

        public List<StudentAbsence> GetStudentAbsenceList(Grade grade)
        {

            List<StudentAbsence> lstStudentAbsence = new List<StudentAbsence>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    CourseBiz courseBiz = new CourseBiz();
                    List<int> lsCourse = courseBiz.GetCourseList(grade).Select(x => x.id).Distinct().ToList();

                    CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                    List<int> lsCurseAsignature = courseAsignatureBiz.GetCourseAsignatureList().Where(x => lsCourse.Contains(x.idCurso)).Select(x => x.id).Distinct().ToList();

                    lstStudentAbsence = ctx.StudentAbsences.Where(x => lsCurseAsignature.Contains(x.idAsignatura)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentAbsence;
        }

        public List<StudentAbsence> GetStudentAbsenceList(Course course)
        {

            List<StudentAbsence> lstStudentAbsence = new List<StudentAbsence>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    
                    CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                    List<int> lsCurseAsignature = courseAsignatureBiz.GetCourseAsignatureList().Where(x => x.idCurso.Equals(course.id)).Select(x => x.id).Distinct().ToList();

                    lstStudentAbsence = ctx.StudentAbsences.Where(x => lsCurseAsignature.Contains(x.idAsignatura)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentAbsence;
        }

        public List<StudentAbsence> GetStudentAbsenceList(CourseAsignature courseAsignature)
        {

            List<StudentAbsence> lstStudentAbsence = new List<StudentAbsence>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    
                    lstStudentAbsence = ctx.StudentAbsences.Where(x => x.idAsignatura.Equals(courseAsignature.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstStudentAbsence;
        }
    }
}
