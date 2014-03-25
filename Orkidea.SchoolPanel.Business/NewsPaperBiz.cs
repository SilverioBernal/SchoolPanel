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
    public class NewsPaperBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<NewsPaper> GetNewsPaperList()
        {

            List<NewsPaper> lstNewsPaper = new List<NewsPaper>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstNewsPaper = ctx.NewsPapers.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNewsPaper;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="newsPaperTarget"></param>
        /// <returns></returns>
        public NewsPaper GetNewsPaperByKey(NewsPaper newsPaperTarget)
        {
            NewsPaper oNewsPaper = new NewsPaper();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oNewsPaper = ctx.NewsPapers.Where(x => x.id.Equals(newsPaperTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oNewsPaper;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="newsPaperTarget"></param>
        public void SaveNewsPaper(NewsPaper newsPaperTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    NewsPaper oNewsPaper = GetNewsPaperByKey(newsPaperTarget);

                    if (oNewsPaper != null)
                    {
                        // if exists then edit 
                        ctx.NewsPapers.Attach(oNewsPaper);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oNewsPaper, newsPaperTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.NewsPapers.Add(newsPaperTarget);
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
        /// <param name="newsPaperTarget"></param>
        public void DeleteNewsPaper(NewsPaper newsPaperTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    NewsPaper oNewsPaper = GetNewsPaperByKey(newsPaperTarget);

                    if (oNewsPaper != null)
                    {
                        // if exists then edit 
                        ctx.NewsPapers.Attach(oNewsPaper);
                        ctx.NewsPapers.Remove(oNewsPaper);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<NewsPaper> GetNewsPaperList(School school)
        {

            List<NewsPaper> lsNewsPaper = new List<NewsPaper>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    lsNewsPaper = ctx.NewsPapers.Where(x => x.idColegio.Equals(school.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsNewsPaper;
        }
    }
}
