using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmReporting
    {
        public int idCurso { get; set; }
        public int año { get; set; }
        public int idPeriodoAcademico { get; set; }

        public List<Course> lsCurso { get; set; }
        public List<AcademicPeriod> lsAdademicPeriod { get; set; }
        //public List<Person> lsEstudiante { get; set; }
        public SortedDictionary<int, int> lsAño { get; set; }

        public vmReporting()
        {
            lsCurso = new List<Course>();
            lsAdademicPeriod = new List<AcademicPeriod>();
            //lsEstudiante = new List<Person>();
            lsAño = new SortedDictionary<int, int>();
        }
    }
}