//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Orkidea.SchoolPanel.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class CourseAsignature
    {
        public CourseAsignature()
        {
            this.StudentAbsences = new HashSet<StudentAbsence>();
            this.Tasks = new HashSet<Task>();
            this.Evaluations = new HashSet<Evaluation>();
        }
    
        public int id { get; set; }
        public int idCurso { get; set; }
        public Nullable<int> idProfesor { get; set; }
        public int idAsignatura { get; set; }
    
        public virtual ICollection<StudentAbsence> StudentAbsences { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual Asignature Asignature { get; set; }
    }
}