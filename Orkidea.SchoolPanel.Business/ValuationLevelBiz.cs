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
    public class ValuationLevelBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<ValuationLevel> GetValuationLevelList()
        {

            List<ValuationLevel> lstValuationLevel = new List<ValuationLevel>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstValuationLevel = ctx.ValuationLevels.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstValuationLevel;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="ValuationLevelTarget"></param>
        /// <returns></returns>
        public ValuationLevel GetValuationLevelByKey(ValuationLevel ValuationLevelTarget)
        {
            ValuationLevel oValuationLevel = new ValuationLevel();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oValuationLevel = ctx.ValuationLevels.Where(x => x.id.Equals(ValuationLevelTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oValuationLevel;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="ValuationLevelTarget"></param>
        public void SaveValuationLevel(ValuationLevel ValuationLevelTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    ValuationLevel oValuationLevel = GetValuationLevelByKey(ValuationLevelTarget);

                    if (oValuationLevel != null)
                    {
                        // if exists then edit 
                        ctx.ValuationLevels.Attach(oValuationLevel);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oValuationLevel, ValuationLevelTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.ValuationLevels.Add(ValuationLevelTarget);
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
        /// <param name="ValuationLevelTarget"></param>
        public void DeleteValuationLevel(ValuationLevel ValuationLevelTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    ValuationLevel oValuationLevel = GetValuationLevelByKey(ValuationLevelTarget);

                    if (oValuationLevel != null)
                    {
                        // if exists then edit 
                        ctx.ValuationLevels.Attach(oValuationLevel);
                        ctx.ValuationLevels.Remove(oValuationLevel);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/
        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<ValuationLevel> GetValuationLevelList(School school)
        {

            List<ValuationLevel> lstValuationLevel = new List<ValuationLevel>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstValuationLevel = ctx.ValuationLevels.Where(x=> x.idColegio.Equals(school.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstValuationLevel;
        }
    }
}
