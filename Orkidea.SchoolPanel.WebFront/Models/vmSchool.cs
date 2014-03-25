using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmSchool
    {
        public int id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        public string nombreColegio { get; set; }

        public string nit { get; set; }
        public string especialidad { get; set; }
        public string resolucion { get; set; }
        public string registroEducativo { get; set; }
        public string registroDANE { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string departamento { get; set; }
        public string logo { get; set; }
        public string nombreRector { get; set; }
        public string nombreSecretario { get; set; }
    }
}