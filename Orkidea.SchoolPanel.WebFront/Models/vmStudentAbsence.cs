using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmStudentAbsence : StudentAbsence
    {
        public string estudiante { get; set; }

        public List<AcademicPeriod> lsPeriodoAcademico { get; set; }
        public List<StudentAbsence> lsFallasEstudiante { get; set; }
        public List<vmStudentAbsence> lsStudentAbsence { get; set; }

        public vmStudentAbsence()
        {
            lsPeriodoAcademico = new List<AcademicPeriod>();
            lsStudentAbsence = new List<vmStudentAbsence>();
            lsFallasEstudiante = new List<StudentAbsence>();
        }
    }
}