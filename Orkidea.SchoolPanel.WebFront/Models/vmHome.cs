using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmHome
    {
        public School school { get; set; }
        public List<Task> lsTask{ get; set; }
        public List<NewsPaper> lsNews { get; set; }

        public vmHome()
        {
            lsTask = new List<Task>();
            lsNews = new List<NewsPaper>();
        }
    }
}