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
    
    public partial class EvaluationNote
    {
        public EvaluationNote()
        {
            this.Evaluations = new HashSet<Evaluation>();
            this.Evaluations1 = new HashSet<Evaluation>();
        }
    
        public int id { get; set; }
        public int idColegio { get; set; }
        public string Comentario { get; set; }
    
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Evaluation> Evaluations1 { get; set; }
        public virtual School School { get; set; }
    }
}
