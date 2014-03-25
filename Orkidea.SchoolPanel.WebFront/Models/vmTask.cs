using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmTask:Task
    {
        public string descActivo { get; set; }
        public List<TaskAttachment> lsTaskAttach { get; set; }

        public vmTask()
        {
            lsTaskAttach = new List<TaskAttachment>();
        }
    }
}