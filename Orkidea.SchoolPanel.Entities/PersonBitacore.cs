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
    
    public partial class PersonBitacore
    {
        public int id { get; set; }
        public int idPersona { get; set; }
        public System.DateTime fecha { get; set; }
        public string observaciones { get; set; }
        public int idAutor { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
