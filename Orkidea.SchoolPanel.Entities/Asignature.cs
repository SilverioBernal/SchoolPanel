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
    
    public partial class Asignature
    {
        public Asignature()
        {
            this.CourseAsignatures = new HashSet<CourseAsignature>();
        }
    
        public int id { get; set; }
        public int idAreaConocimiento { get; set; }
        public int idGrado { get; set; }
        public string Descripcion { get; set; }
        public int intensidadHoraria { get; set; }
        public Nullable<bool> ignorarEnPromedio { get; set; }
    
        public virtual Grade Grade { get; set; }
        public virtual KnowledgeArea KnowledgeArea { get; set; }
        public virtual ICollection<CourseAsignature> CourseAsignatures { get; set; }
    }
}
