using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Utilities;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Orkidea.SchoolPanel.Business
{
    public class AsignatureBiz
    {
        /*CRUD Asignature*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<Asignature> GetAsignatureList()
        {
            List<KnowledgeArea> lstKnowledgeArea = new List<KnowledgeArea>();
            List<Asignature> lstAsignature = new List<Asignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAsignature = ctx.Asignatures.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAsignature;
        }

        /// <summary>
        /// Retrieve assignature list by school
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>


        /// <summary>
        /// Retrieve Asignature information based in the primary key
        /// </summary>
        /// <param name="assignatureTarget"></param>
        /// <returns></returns>
        public Asignature GetAsignaturebyKey(Asignature assignatureTarget)
        {
            Asignature oAssignature = new Asignature();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oAssignature =
                        ctx.Asignatures.Where(x => x.id.Equals(assignatureTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oAssignature;
        }

        /// <summary>
        /// Create or update an Asignature
        /// </summary>
        /// <param name="assignatureTarget"></param>
        public void SaveAsignature(Asignature assignatureTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the grade exists
                    Asignature oAsignature = GetAsignaturebyKey(assignatureTarget);

                    if (oAsignature != null)
                    {
                        // if exists then edit 
                        ctx.Asignatures.Attach(oAsignature);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oAsignature, assignatureTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Asignatures.Add(assignatureTarget);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete an Asignature
        /// </summary>
        /// <param name="assignatureTarget"></param>
        public void DeleteAsignature(Asignature assignatureTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    Asignature oAsignature = GetAsignaturebyKey(assignatureTarget);

                    if (oAsignature != null)
                    {
                        // if exists then edit 
                        ctx.Asignatures.Attach(oAsignature);
                        ctx.Asignatures.Remove(oAsignature);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar esta asignatura porque existe información asociada a esta.");
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<Asignature> GetAsignatureList(School schoolTarget)
        {
            List<KnowledgeArea> lstKnowledgeArea = new List<KnowledgeArea>();
            List<Asignature> lstAssignature = new List<Asignature>();
            List<Asignature> lstAssignatureReturn = new List<Asignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    KnowledgeAreaBiz kaBiz = new KnowledgeAreaBiz();

                    lstKnowledgeArea = kaBiz.GetKnowledgeAreaList(schoolTarget);

                    foreach (KnowledgeArea item in lstKnowledgeArea)
                    {
                        lstAssignature = GetAsignatureList(item);

                        foreach (Asignature asignatureItem in lstAssignature)
                        {
                            lstAssignatureReturn.Add(asignatureItem);
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAssignatureReturn;
        }

        public List<Asignature> GetAsignatureList(KnowledgeArea knowledgeAreaTarget)
        {

            List<Asignature> lstAssignature = new List<Asignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAssignature = ctx.Asignatures.Where(x => x.idAreaConocimiento.Equals(knowledgeAreaTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAssignature;
        }

        public List<Asignature> GetAsignatureList(Grade gradeTarget)
        {

            List<Asignature> lstAssignature = new List<Asignature>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstAssignature = ctx.Asignatures.Where(x => x.idGrado.Equals(gradeTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstAssignature;
        }
    }
}
