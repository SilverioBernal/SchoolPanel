
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmCourseAsignature
    {
        public int id { get; set; }
        public int idCurso { get; set; }
        public Nullable<int> idProfesor { get; set; }
        public int idAsignatura { get; set; }

        public string curso{ get; set; }
        public string profesor { get; set; }
        public string asignatura { get; set; }

        public List<vmPerson> lsProfesor { get; set; }
        public List<Asignature> lsAsignatura { get; set; }
        public List<vmCourseAsignature> lsCourseAsignature { get; set; }
        public List<AcademicPeriod> lsAcademicPeriod { get; set; }

        public vmCourseAsignature()
        {
            lsProfesor = new List<vmPerson>();
            lsAsignatura = new List<Asignature>();
            lsCourseAsignature = new List<vmCourseAsignature>();
            lsAcademicPeriod = new List<AcademicPeriod>();            
        }
    }
}