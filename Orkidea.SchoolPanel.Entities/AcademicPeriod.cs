//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Orkidea.SchoolPanel.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AcademicPeriod
    {
        public AcademicPeriod()
        {
            this.StudentAbsences = new HashSet<StudentAbsence>();
            this.Evaluations = new HashSet<Evaluation>();
        }
    
        public int id { get; set; }
        public int idColegio { get; set; }
        public string Descripcion { get; set; }
        public decimal valorPorcentual { get; set; }
    
        public virtual ICollection<StudentAbsence> StudentAbsences { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual School School { get; set; }
    }
}
