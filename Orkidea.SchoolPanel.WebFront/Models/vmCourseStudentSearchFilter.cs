using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmCourseStudentSearchFilter
    {
        public int idSede { get; set; }
        public int idJornada { get; set; }
        public int ano { get; set; }
        public int idGrado { get; set; }

        public List<Place> lsSede { get; set; }
        public SortedDictionary<int, string> lsAnos { get; set; }
        public List<Grade> lsGrado { get; set; }
        public SortedDictionary<int, string> lsJornada { get; set; }

        public vmCourseStudentSearchFilter()
        {
            lsAnos = new SortedDictionary<int, string>();
            lsGrado = new List<Grade>();
            lsJornada = new SortedDictionary<int, string>();
            lsSede = new List<Place>();

            lsJornada.Add(1, "Mañana");
            lsJornada.Add(2, "Tarde");
            lsJornada.Add(3, "Noche");
            lsJornada.Add(4, "Única");
        }
    }
}