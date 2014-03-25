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
    public class KnowledgeAreaBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<KnowledgeArea> GetKnowledgeAreaList(School schoolTarget)
        {

            List<KnowledgeArea> lstKnowledgeArea = new List<KnowledgeArea>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstKnowledgeArea = ctx.KnowledgeAreas.Where(x=> x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstKnowledgeArea;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="KnowledgeAreaTarget"></param>
        /// <returns></returns>
        public KnowledgeArea GetKnowledgeAreaByKey(KnowledgeArea KnowledgeAreaTarget)
        {
            KnowledgeArea oKnowledgeArea = new KnowledgeArea();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oKnowledgeArea = ctx.KnowledgeAreas.Where(x => x.id.Equals(KnowledgeAreaTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oKnowledgeArea;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="KnowledgeAreaTarget"></param>
        public void SaveKnowledgeArea(KnowledgeArea KnowledgeAreaTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    KnowledgeArea oKnowledgeArea = GetKnowledgeAreaByKey(KnowledgeAreaTarget);

                    if (oKnowledgeArea != null)
                    {
                        // if exists then edit 
                        ctx.KnowledgeAreas.Attach(oKnowledgeArea);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oKnowledgeArea, KnowledgeAreaTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.KnowledgeAreas.Add(KnowledgeAreaTarget);
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
        /// <param name="KnowledgeAreaTarget"></param>
        public void DeleteKnowledgeArea(KnowledgeArea KnowledgeAreaTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    KnowledgeArea oKnowledgeArea = GetKnowledgeAreaByKey(KnowledgeAreaTarget);

                    if (oKnowledgeArea != null)
                    {
                        // if exists then edit 
                        ctx.KnowledgeAreas.Attach(oKnowledgeArea);
                        ctx.KnowledgeAreas.Remove(oKnowledgeArea);
                        ctx.SaveChanges();
                    }
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
        }
    }
}
