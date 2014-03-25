using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmCourseStudent
    {
        public int id { get; set; }
        public int idCurso { get; set; }
        public int idEstudiante { get; set; }


        public string estudiante { get; set; }
        public string documento { get; set; }

        public List<vmCourseStudent> lsVmStudent { get; set; }

        public vmCourseStudent()
        {
            lsVmStudent = new List<vmCourseStudent>();
        }
    }
}