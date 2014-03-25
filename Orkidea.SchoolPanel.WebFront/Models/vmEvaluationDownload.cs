using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    [Serializable]
    public class vmEvaluationDownload
    {
        public int idEstudiante { get; set; }
        public string desEstudiante { get; set; }        
        public decimal Nota { get; set; }
        public Nullable<int> comentario1 { get; set; }
        public Nullable<int> comentario2 { get; set; }
        public Nullable<int> numeroFallas { get; set; }
        public string observaciones { get; set; }
        public vmEvaluationDownload()
        {
        
        }
    }
}