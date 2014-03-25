using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmAssingnature
    {
        public int id { get; set; }
        public int idColegio { get; set; }        
        public int idGrado { get; set; }
        public string DesGrado { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "Nota minima")]
        public decimal notaMinima { get; set; }
        [Display(Name = "Intensidad horaria")]
        public int intensidadHoraria { get; set; }
        public List<Grade> lstGrade { get; set; }

        public vmAssingnature()
        {
            lstGrade = new List<Grade>();
        }
    }
}