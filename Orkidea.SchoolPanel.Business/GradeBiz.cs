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
    public class GradeBiz
    {
        /*CRUD Grades*/

        /// <summary>
        /// Retrieve grades list without parameters
        /// </summary>
        /// <returns></returns>
        public List<Grade> GetGradeList()
        {

            List<Grade> lstGrade = new List<Grade>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstGrade = ctx.Grades.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstGrade;
        }

        /// <summary>
        /// Retrieve grade information based in the primary key
        /// </summary>
        /// <param name="gradeTarget"></param>
        /// <returns></returns>
        public Grade GetGradebyKey(Grade gradeTarget)
        {
            Grade oGrade = new Grade();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oGrade = ctx.Grades.Where(x => x.id.Equals(gradeTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oGrade;
        }

        /// <summary>
        /// Create or update a grade
        /// </summary>
        /// <param name="gradeTarget"></param>
        public void SaveGrade(Grade gradeTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the grade exists
                    Grade oGrade = GetGradebyKey(gradeTarget);

                    if (oGrade != null)
                    {
                        // if exists then edit 
                        ctx.Grades.Attach(oGrade);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oGrade, gradeTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Grades.Add(gradeTarget);
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
        /// Delete a grade
        /// </summary>
        /// <param name="gradeTarget"></param>
        public void DeleteGrade(Grade gradeTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    Grade oGrade = GetGradebyKey(gradeTarget);

                    if (oGrade != null)
                    {
                        // if exists then edit 
                        ctx.Grades.Attach(oGrade);
                        ctx.Grades.Remove(oGrade);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar este grado porque existe información asociada a este.");
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<Grade> GetGradeList(School school)
        {

            List<Grade> lstGrade = new List<Grade>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstGrade = ctx.Grades.Where(X => X.idColegio.Equals(school.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstGrade;
        }
    }
}
