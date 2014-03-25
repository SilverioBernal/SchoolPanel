using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmCourse
    {
        public int id { get; set; }
        public int idColegio { get; set; }
        public int idJornada { get; set; }
        public int idSede { get; set; }
        public int idGrado { get; set; }
        public int ano { get; set; }
        public string Descripcion { get; set; }
        public int idDirectorCurso { get; set; }
        public bool finalizado { get; set; }

        public string grado { get; set; }
        public string directorCurso { get; set; }
        public string sede { get; set; }
        public string jornada { get; set; }

        public List<Grade> lsGrade { get; set; }
        public List<Asignature> lsAsignature { get; set; }
        public List<vmPerson> lsTeacher { get; set; }
        public List<vmPerson> lsStudent { get; set; }
        public List<Place> lsPlace { get; set; }
        public SortedDictionary<int, string> lsJornada { get; set; }
        //public List<vmCourse> ls { get; set; }

        //public List<vmCourseStudent> lstStudentsNoCourse { get; set; }
        //public List<vmCourseTeacher> lstCourseTeachers { get; set; }
        //public List<vmCourseTeacher> lstTeachersNoCourse { get; set; }

        public vmCourse()
        {
            lsGrade = new List<Grade>();
            lsAsignature = new List<Asignature>();
            lsTeacher = new List<vmPerson>();
            lsStudent = new List<vmPerson>();

            lsPlace = new List<Place>();

            lsJornada = new SortedDictionary<int, string>();

            lsJornada.Add(1, "Mañana");
            lsJornada.Add(2, "Tarde");
            lsJornada.Add(3, "Noche");
            lsJornada.Add(4, "Única");

            //    lstStudentsNoCourse = new List<vmCourseStudent>();
            //    lstCourseTeachers = new List<vmCourseTeacher>();
            //    lstTeachersNoCourse = new List<vmCourseTeacher>();
            //    lstPreviusCourses = new List<vmCourse>();
        }
    }
}