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
    public class CourseAsignatureBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<CourseAsignature> GetCourseAsignatureList()
        {

            List<CourseAsignature> lsCourseAsignature = new List<CourseAsignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsCourseAsignature = ctx.CourseAsignatures.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsCourseAsignature;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="courseAsignature"></param>
        /// <returns></returns>
        public CourseAsignature GetCourseAsignatureByKey(CourseAsignature courseAsignature)
        {
            CourseAsignature currentCourseAsignature = new CourseAsignature();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    currentCourseAsignature = ctx.CourseAsignatures.Where(x => x.id.Equals(courseAsignature.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return currentCourseAsignature;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="courseAsignature"></param>
        public void SaveCourseAsignature(CourseAsignature courseAsignature)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    CourseAsignature currentCourseAsignature = GetCourseAsignatureByKey(courseAsignature);

                    if (currentCourseAsignature != null)
                    {
                        // if exists then edit 
                        ctx.CourseAsignatures.Attach(currentCourseAsignature);
                        _GenericEntityValidation.EnumeratePropertyDifferences(currentCourseAsignature, courseAsignature);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create                        
                        ctx.CourseAsignatures.Add(courseAsignature);
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
        /// <param name="courseAsignature"></param>
        public void DeleteCourseAsignature(CourseAsignature courseAsignature)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    CourseAsignature currentCourseAsignature = GetCourseAsignatureByKey(courseAsignature);

                    if (currentCourseAsignature != null)
                    {
                        // if exists then edit 
                        ctx.CourseAsignatures.Attach(currentCourseAsignature);
                        ctx.CourseAsignatures.Remove(currentCourseAsignature);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<CourseAsignature> GetCourseAsignatureList(Course course)
        {

            List<CourseAsignature> lsCourseAsignature = new List<CourseAsignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    
                        lsCourseAsignature = ctx.CourseAsignatures.Where(c => c.idCurso.Equals(course.id)).ToList();

                }
            }
            catch (Exception ex) { throw ex; }

            return lsCourseAsignature;
        }

        public List<CourseAsignature> GetCourseAsignatureList(Person profesor)
        {

            List<CourseAsignature> lsCourseAsignature = new List<CourseAsignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsCourseAsignature = ctx.CourseAsignatures.Where(c => c.idProfesor == profesor.id).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsCourseAsignature;
        }

        /// <summary>
        /// Retrieve information based in fileds differents to primary key
        /// </summary>
        /// <param name="courseAsignature"></param>
        /// <returns></returns>
        public CourseAsignature GetCourseAsignatureByOthers(CourseAsignature courseAsignature)
        {
            CourseAsignature currentCourseAsignature = new CourseAsignature();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    currentCourseAsignature = ctx.CourseAsignatures.Where(x => x.idCurso.Equals(courseAsignature.idCurso) && x.idAsignatura.Equals(courseAsignature.idAsignatura)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return currentCourseAsignature;
        }
    }
}
