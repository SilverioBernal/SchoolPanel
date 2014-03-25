using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    [Serializable]
    public class vmEvaluation
    {
        public string idColegio { get; set; }
        public string desColegio { get; set; }
        public string idCurso { get; set; }
        public string desCurso { get; set; }
        public string idGrado { get; set; }        
        public string desEstudiante { get; set; }
        public string idProfesor { get; set; }        
        public string desAsignatura { get; set; }        
        public string desPeriodoAcademico { get; set; }

        public int idPeriodoAcademico { get; set; }
        public int idAsignatura { get; set; }
        public int idEstudiante { get; set; }
        public decimal Nota { get; set; }
        public Nullable<int> numeroFallas { get; set; }
        public Nullable<int> comentario1 { get; set; }
        public Nullable<int> comentario2 { get; set; }
        public string observaciones { get; set; }

        public string ape1 { get; set; }
        public string ape2 { get; set; }

        
        public string ano { get; set; }

        public List<School> lstColegio { get; set; }
        public List<Person> lstProfesor { get; set; }
        public List<Course> lstCurso { get; set; }
        public List<Asignature> lstAsignatura { get; set; }
        public Hashtable lstNotas { get; set; }
        public List<EvaluationNote> lsEvaluationNotes { get; set; }

        public vmEvaluation()
        {
            lstColegio = new List<School>();
            lstProfesor = new List<Person>();
            lstColegio = new List<School>();
            lstAsignatura = new List<Asignature>();
            lstNotas = new Hashtable();
            lsEvaluationNotes = new List<EvaluationNote>();
        }
    }
}