using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Utilities;
using System.Data.Entity.Infrastructure;

namespace Orkidea.SchoolPanel.Business
{
    public class AcademicPeriodBiz
    {
        /*CRUD Academic period*/

        /// <summary>
        /// Retrieve student list by school
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<AcademicPeriod> GetAcademicPeriodList(School schoolTarget)
        {

            List<AcademicPeriod> lstAcademicPeriod = new List<AcademicPeriod>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAcademicPeriod = ctx.AcademicPeriods.Where(x => x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAcademicPeriod;
        }

        /// <summary>
        /// Retrieve student information based in the primary key
        /// </summary>
        /// <param name="academicPeriodTarget"></param>
        /// <returns></returns>
        public AcademicPeriod GetAcademicPeriodbyKey(AcademicPeriod academicPeriodTarget)
        {
            AcademicPeriod oAcademicPeriod = new AcademicPeriod();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oAcademicPeriod = ctx.AcademicPeriods.Where(x => x.id.Equals(academicPeriodTarget.id)).FirstOrDefault();
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar esta área de conocimiento porque existe información asociada a esta.");
                }
            }
            catch (Exception ex) { throw ex; }

            return oAcademicPeriod;
        }

        /// <summary>
        /// Create or update a student
        /// </summary>
        /// <param name="academicPeriod"></param>
        public void SaveAcademicPeriod(AcademicPeriod academicPeriod)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the student exists
                    AcademicPeriod oAcademicPeriod = GetAcademicPeriodbyKey(academicPeriod);

                    if (oAcademicPeriod!= null )
                    {
                        // if exists then edit 
                        ctx.AcademicPeriods.Attach(oAcademicPeriod);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oAcademicPeriod, academicPeriod);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.AcademicPeriods.Add(academicPeriod);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a grade
        /// </summary>
        /// <param name="academicPeriodTarget"></param>
        public void DeleteAcademicPeriod(AcademicPeriod academicPeriodTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    AcademicPeriod oAcademicPeriod = GetAcademicPeriodbyKey(academicPeriodTarget);

                    if (oAcademicPeriod != null)
                    {
                        // if exists then edit 
                        ctx.AcademicPeriods.Attach(oAcademicPeriod);
                        ctx.AcademicPeriods.Remove(oAcademicPeriod);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
