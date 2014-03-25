
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class TaskAttachmentBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<TaskAttachment> GetTaskAttachmentList()
        {

            List<TaskAttachment> lstTaskAttachment = new List<TaskAttachment>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstTaskAttachment = ctx.TaskAttachments.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstTaskAttachment;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="taskAttachmentTarget"></param>
        /// <returns></returns>
        public TaskAttachment GetTaskAttachmentByKey(TaskAttachment taskAttachmentTarget)
        {
            TaskAttachment oTaskAttachment = new TaskAttachment();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oTaskAttachment = ctx.TaskAttachments.Where(x => x.id.Equals(taskAttachmentTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oTaskAttachment;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="taskAttachmentTarget"></param>
        public void SaveTaskAttachment(TaskAttachment taskAttachmentTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    TaskAttachment oTaskAttachment = GetTaskAttachmentByKey(taskAttachmentTarget);

                    if (oTaskAttachment != null)
                    {
                        // if exists then edit 
                        ctx.TaskAttachments.Attach(oTaskAttachment);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oTaskAttachment, taskAttachmentTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.TaskAttachments.Add(taskAttachmentTarget);
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
        /// <param name="taskAttachmentTarget"></param>
        public void DeleteTaskAttachment(TaskAttachment taskAttachmentTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    TaskAttachment oTaskAttachment = GetTaskAttachmentByKey(taskAttachmentTarget);

                    if (oTaskAttachment != null)
                    {
                        // if exists then edit 
                        ctx.TaskAttachments.Attach(oTaskAttachment);
                        ctx.TaskAttachments.Remove(oTaskAttachment);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<TaskAttachment> GetTaskAttachmentList(TaskAttachment taskAttachmentTarget)
        {

            List<TaskAttachment> lsTaskAttachment = new List<TaskAttachment>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    lsTaskAttachment = ctx.TaskAttachments.Where(x => x.idTarea.Equals(taskAttachmentTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsTaskAttachment;
        }

    }
}
