using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            SchoolBiz schoolBiz = new SchoolBiz();
            TaskBiz taskBiz = new TaskBiz();

            NewsPaperBiz newsBiz = new NewsPaperBiz();
            NewsAttachmentBiz newsAttachmentBiz = new NewsAttachmentBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;
            int idRol = 0;
            int usuario = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                usuario = int.Parse(userRole[0]);
                idRol = int.Parse(userRole[1]);
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            #region Noticias
            List<NewsPaper> lsNews = newsBiz.GetNewsPaperList(school);
            List<NewsAttachment> lsNewsFiles = newsAttachmentBiz.GetNewsAttachmentList(school);

            foreach (NewsPaper item in lsNews)
                item.NewsAttachments = lsNewsFiles.Where(x => x.idNoticia.Equals(item.id)).ToList();

            vmHome appHome = new vmHome()
            {
                school = schoolBiz.GetSchoolbyKey(school),
                lsNews = lsNews
            };
            #endregion

            if (idRol.Equals(5))
            {
                CourseBiz courseBiz = new CourseBiz();
                Course course = courseBiz.GetCourseList(usuario).First();

                CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();
                List<int> asignaturas = courseAsignatureBiz.GetCourseAsignatureList(course).Select(x => x.id).Distinct().ToList();

                List<Task> lstask = taskBiz.GetTaskList().Where(x => asignaturas.Contains(x.idAsignatura) && x.fechaLimite >= DateTime.Now).ToList();

                List<int> lsIdTask = lstask.Select(x => x.id).ToList();

                TaskAttachmentBiz taskAttachmentBiz = new TaskAttachmentBiz();
                List<TaskAttachment> lsTaskAttach = taskAttachmentBiz.GetTaskAttachmentList().Where(x => lsIdTask.Contains(x.idTarea)).ToList();

                foreach (Task item in lstask)
                {
                    item.TaskAttachments = lsTaskAttach.Where(x => x.idTarea.Equals(item.id)).ToList();
                }
                appHome.lsTask = lstask;    
            } 
            
            

            return View(appHome);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
