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
    
    public partial class StudentAbsence
    {
        public int id { get; set; }
        public int idEstudiante { get; set; }
        public int idAsignatura { get; set; }
        public int idPeriodoAcademico { get; set; }
        public System.DateTime fecha { get; set; }
    
        public virtual AcademicPeriod AcademicPeriod { get; set; }
        public virtual CourseAsignature CourseAsignature { get; set; }
        public virtual CourseStudent CourseStudent { get; set; }
    }
}
