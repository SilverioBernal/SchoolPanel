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
    public class TaskBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<Orkidea.SchoolPanel.Entities.Task> GetTaskList()
        {

            List<Orkidea.SchoolPanel.Entities.Task> lstTask = new List<Orkidea.SchoolPanel.Entities.Task>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstTask = ctx.Tasks.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstTask;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="taskTarget"></param>
        /// <returns></returns>
        public Orkidea.SchoolPanel.Entities.Task GetTaskByKey(Orkidea.SchoolPanel.Entities.Task taskTarget)
        {
            Orkidea.SchoolPanel.Entities.Task oTask = new Orkidea.SchoolPanel.Entities.Task();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oTask = ctx.Tasks.Where(x => x.id.Equals(taskTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oTask;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="taskTarget"></param>
        public void SaveTask(Orkidea.SchoolPanel.Entities.Task taskTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Orkidea.SchoolPanel.Entities.Task oTask = GetTaskByKey(taskTarget);

                    if (oTask != null)
                    {
                        // if exists then edit 
                        ctx.Tasks.Attach(oTask);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oTask, taskTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Tasks.Add(taskTarget);
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
        /// <param name="taskTarget"></param>
        public void DeleteTask(Orkidea.SchoolPanel.Entities.Task taskTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Orkidea.SchoolPanel.Entities.Task oTask = GetTaskByKey(taskTarget);

                    if (oTask != null)
                    {
                        // if exists then edit 
                        ctx.Tasks.Attach(oTask);
                        ctx.Tasks.Remove(oTask);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<Orkidea.SchoolPanel.Entities.Task> GetTaskList(CourseAsignature courseAsignature)
        {

            List<Orkidea.SchoolPanel.Entities.Task> lsTask = new List<Orkidea.SchoolPanel.Entities.Task>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    lsTask = ctx.Tasks.Where(x => x.idAsignatura.Equals(courseAsignature.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsTask;
        }
    }
}
