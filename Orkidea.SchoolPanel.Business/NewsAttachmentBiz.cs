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
    public class NewsAttachmentBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<NewsAttachment> GetNewsAttachmentList()
        {

            List<NewsAttachment> lstNewsAttachment = new List<NewsAttachment>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstNewsAttachment = ctx.NewsAttachments.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstNewsAttachment;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="newsAttachmentTarget"></param>
        /// <returns></returns>
        public NewsAttachment GetNewsAttachmentByKey(NewsAttachment newsAttachmentTarget)
        {
            NewsAttachment oNewsAttachment = new NewsAttachment();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oNewsAttachment = ctx.NewsAttachments.Where(x => x.id.Equals(newsAttachmentTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oNewsAttachment;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="newsAttachmentTarget"></param>
        public void SaveNewsAttachment(NewsAttachment newsAttachmentTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    NewsAttachment oNewsAttachment = GetNewsAttachmentByKey(newsAttachmentTarget);

                    if (oNewsAttachment != null)
                    {
                        // if exists then edit 
                        ctx.NewsAttachments.Attach(oNewsAttachment);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oNewsAttachment, newsAttachmentTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.NewsAttachments.Add(newsAttachmentTarget);
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
        /// <param name="newsAttachmentTarget"></param>
        public void DeleteNewsAttachment(NewsAttachment newsAttachmentTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    NewsAttachment oNewsAttachment = GetNewsAttachmentByKey(newsAttachmentTarget);

                    if (oNewsAttachment != null)
                    {
                        // if exists then edit 
                        ctx.NewsAttachments.Attach(oNewsAttachment);
                        ctx.NewsAttachments.Remove(oNewsAttachment);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        public List<NewsAttachment> GetNewsAttachmentList(School school)
        {

            List<NewsAttachment> lsNewsAttachment = new List<NewsAttachment>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    NewsPaperBiz newsPaperBiz = new NewsPaperBiz();
                    List<int> lsNoticias = newsPaperBiz.GetNewsPaperList(school).Select(x => x.id).ToList();

                    lsNewsAttachment = ctx.NewsAttachments.Where(x => lsNoticias.Contains(x.idNoticia)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsNewsAttachment;
        }

        public List<NewsAttachment> GetNewsAttachmentList(NewsPaper newsPaper)
        {

            List<NewsAttachment> lsNewsAttachment = new List<NewsAttachment>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    lsNewsAttachment = ctx.NewsAttachments.Where(x => x.idNoticia.Equals(newsPaper.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lsNewsAttachment;
        }
    }
}
