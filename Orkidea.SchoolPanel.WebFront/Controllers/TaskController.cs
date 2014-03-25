using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class TaskController : Controller
    {
        TaskBiz taskBiz = new TaskBiz();
        //
        // GET: /Task/
        [Authorize]
        public ActionResult Index(int id)
        {
            TaskAttachmentBiz taskAttachmentBiz = new TaskAttachmentBiz();
            List<TaskAttachment> lsTaskAttachment = taskAttachmentBiz.GetTaskAttachmentList();

            List<Orkidea.SchoolPanel.Entities.Task> lsTask = taskBiz.GetTaskList(new CourseAsignature() { id = id });
            ViewBag.id = id;

            List<vmTask> lsCurrentTasks = new List<vmTask>();

            foreach (Task item in lsTask)
            {
                lsCurrentTasks.Add(new vmTask()
                {
                    id = item.id,
                    idAsignatura = item.idAsignatura,
                    activo = item.activo,
                    descripcion = item.descripcion,
                    fechaCreacion = item.fechaCreacion,
                    fechaLimite = item.fechaLimite,
                    titulo = item.titulo,
                    descActivo = item.activo ? "Activa" : "Inactiva",
                    lsTaskAttach = lsTaskAttachment.Where(x => x.idTarea.Equals(item.id)).ToList()
                });
            }
            return View(lsCurrentTasks);
        }

        //
        // GET: /Task/Create
        [Authorize]
        public ActionResult Create(int id)
        {
            ViewBag.id = id;
            return View();
        }

        //
        // POST: /Task/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(int id, Task task)
        {
            try
            {
                Task newTask = new Task()
                {
                    idAsignatura = id,
                    fechaCreacion = DateTime.Now,
                    activo = true,
                    descripcion = task.descripcion,
                    fechaLimite = task.fechaLimite,
                    titulo = task.titulo
                };
                taskBiz.SaveTask(newTask);

                return RedirectToAction("Index", new { id = id });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Task/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Task task = taskBiz.GetTaskByKey(new Task() { id = id });
            vmTask currentTask = new vmTask()
            {
                id = id,
                idAsignatura = task.idAsignatura,
                activo = task.activo,
                descripcion = task.descripcion,
                fechaCreacion = task.fechaCreacion,
                fechaLimite = task.fechaLimite,
                titulo = task.titulo
            };
            ViewBag.id = id;
            return View(currentTask);
        }

        //
        // POST: /Task/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, Task task)
        {
            try
            {
                task.id = id;

                taskBiz.SaveTask(task);

                return RedirectToAction("Index", new { id = task.idAsignatura });
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Task/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            Task task = taskBiz.GetTaskByKey(new Task() { id = id });
            taskBiz.DeleteTask(task);
            return RedirectToAction("Index", new { id = task.idAsignatura });
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
                string physicalPath = HttpContext.Server.MapPath("~") + "\\" + "UploadedFiles" + "\\";
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Request.Files[0].FileName);

                Request.Files[0].SaveAs(physicalPath + fileName);

                Task task = taskBiz.GetTaskByKey(new Task() { id = id });

                TaskAttachmentBiz taskAttachmentBiz = new TaskAttachmentBiz();

                taskAttachmentBiz.SaveTaskAttachment(new TaskAttachment() { idTarea = id, rutaAdjunto = fileName });

                return RedirectToAction("Index", new { id = task.idAsignatura });
            }
            else
            {
                Task task = taskBiz.GetTaskByKey(new Task() { id = id });
                return RedirectToAction("Index", new { id = task.idAsignatura });
            }
        }

        [Authorize]
        public ActionResult DetachFile(int id)
        {
            TaskAttachmentBiz taskAttachmentBiz = new TaskAttachmentBiz();
            TaskAttachment taskAttachment = taskAttachmentBiz.GetTaskAttachmentByKey(new TaskAttachment() { id = id });
            taskAttachmentBiz.DeleteTaskAttachment(new TaskAttachment() { id = id });

            Task task = taskBiz.GetTaskByKey(new Task() { id = taskAttachment.idTarea });
            return RedirectToAction("Index", new { id = task.idAsignatura });
        }
    }
}
