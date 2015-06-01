using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmAsignature
    {
        public int id { get; set; }
        public int idAreaConocimiento { get; set; }
        public int idGrado { get; set; }
        public string Descripcion { get; set; }
        public int intensidadHoraria { get; set; }
        public bool ignorarEnPromedio { get; set; }
        public Nullable<decimal> pesoPorcentualAreaConocimiento { get; set; }

        public string desAreaConocimiento { get; set; }
        public string desGrado { get; set; }

        public List<KnowledgeArea> lsKnowledgeArea { get; set; }
        public List<Grade> lsGrade { get; set; }

        public vmAsignature()
        {
            lsKnowledgeArea = new List<KnowledgeArea>();
            lsGrade = new List<Grade>();
        }
    }
}