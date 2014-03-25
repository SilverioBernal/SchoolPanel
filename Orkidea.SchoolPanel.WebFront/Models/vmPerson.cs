using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Entities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmPerson
    {
        public int id { get; set; }
        public int idColegio { get; set; }
        public int idSede { get; set; }
        public int idJornada { get; set; }
        public int idRol { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public bool usuarioActivo { get; set; }
        public string primerNombre { get; set; }
        public string segundoNombre { get; set; }
        public string primerApellido { get; set; }
        public string segundoApellido { get; set; }
        public string tipoDocumento { get; set; }
        public string documento { get; set; }
        public bool sexo { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public string ciudad { get; set; }
        public string padre { get; set; }
        public string telefonoPadre { get; set; }
        public string madre { get; set; }
        public string telefonoMadre { get; set; }
        public string nombreOtroContacto { get; set; }
        public string telefonoOtroContacto { get; set; }
        public bool retirado { get; set; }

        public string nombreCompleto { get; set; }
        public string nombreSede { get; set; }
        public string nombreJornada { get; set; }
        public string desRetirado { get; set; }
        public string desActivo { get; set; }

        public List<Place> lsPlace{ get; set; }
        public SortedDictionary<int, string> lsJornada { get; set; }
        public SortedDictionary<string, string> lsTipoDocumento { get; set; }

        public vmPerson()
        {
            lsPlace = new List<Place>();
            
            lsJornada = new SortedDictionary<int, string>();

            lsJornada.Add(1, "Mañana");
            lsJornada.Add(2, "Tarde");
            lsJornada.Add(3, "Noche");
            lsJornada.Add(4, "Única");

            lsTipoDocumento = new SortedDictionary<string, string>();
            lsTipoDocumento.Add("RC", "RC");
            lsTipoDocumento.Add("TI", "TI");
            lsTipoDocumento.Add("CC", "CC");            
        }
    }
}