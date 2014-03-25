using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class NewsPaperController : Controller
    {
        NewsPaperBiz newsPaperBiz = new NewsPaperBiz();
        //
        // GET: /NewsPaper/
        [Authorize]
        public ActionResult Index()
        {
            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            NewsAttachmentBiz newsAttachmentBiz = new NewsAttachmentBiz();

            List<NewsPaper> lsNews = newsPaperBiz.GetNewsPaperList(school);

            foreach (NewsPaper item in lsNews)
            {
                item.NewsAttachments = newsAttachmentBiz.GetNewsAttachmentList(item);
            }

            return View(lsNews);
        }

        //
        // GET: /NewsPaper/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NewsPaper/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(NewsPaper newsPaper)
        {
            try
            {
                #region School identification
                System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                int idColegio = 0;

                if (context.IsAuthenticated)
                {
                    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                    string[] userRole = ci.Ticket.UserData.Split('|');
                    idColegio = int.Parse(userRole[2]);
                }

                School school = new School() { id = idColegio };
                #endregion

                newsPaper.fecha = DateTime.Now;
                newsPaper.idColegio = school.id;

                newsPaperBiz.SaveNewsPaper(newsPaper);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NewsPaper/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            NewsPaper newsPaper = newsPaperBiz.GetNewsPaperByKey(new NewsPaper() { id = id });

            return View(newsPaper);
        }

        //
        // POST: /NewsPaper/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, NewsPaper newsPaper)
        {
            try
            {

                newsPaper.id = id;
                newsPaperBiz.SaveNewsPaper(newsPaper);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NewsPaper/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            NewsPaper newsPaper = newsPaperBiz.GetNewsPaperByKey(new NewsPaper() { id = id });

            newsPaperBiz.DeleteNewsPaper(newsPaper);

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult AttachFile(int id)
        {

            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachFile(int id, HttpPostedFileBase uploadFile)
        {
            int files = Request.Files.Count;

            if (files > 0)
            {
                NewsAttachmentBiz newsAttachmentBiz = new NewsAttachmentBiz();

                string physicalPath = HttpContext.Server.MapPath("~") + "\\" + "UploadedFiles" + "\\";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Request.Files[0].FileName);

                Request.Files[0].SaveAs(physicalPath + fileName);

                NewsPaper newsPaper = newsPaperBiz.GetNewsPaperByKey(new NewsPaper() { id = id });

                NewsAttachment newsAttachment = new NewsAttachment() { };

                newsAttachmentBiz.SaveNewsAttachment(new NewsAttachment() { idNoticia = id, rutaAdjunto = fileName });

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public ActionResult DetachFile(int id)
        {
            NewsAttachmentBiz newsAttachmentBiz = new NewsAttachmentBiz();
            newsAttachmentBiz.DeleteNewsAttachment(new NewsAttachment() { id = id });

            return RedirectToAction("Index");
        }
    }
}
