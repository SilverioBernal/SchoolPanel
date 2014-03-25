using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.WebFront.Models
{
    public class vmUser
    {
        public string usuario { get; set; }
        public string nombre { get; set; }

        //[Required(ErrorMessage = "Required")]
        //[RegularExpression(@"\A[^/]{8,50}\Z", ErrorMessage = "El nombre de usuario ")]
        public string contraseña { get; set; }
        public int Rol { get; set; }
        public int idTabla { get; set; }
        public bool activo { get; set; }
        public int idColegio { get; set; }
        public List<vmUser> lstUser { get; set; }

        public vmUser()
        {
            lstUser = new List<vmUser>();
        }

        public vmUser(string Usuario, string Contrasena)
        {
            lstUser = new List<vmUser>();

            Cryptography oCrypto = new Cryptography();

            usuario = Usuario;
            contraseña = oCrypto.Encrypt(Contrasena);
        }
    }
}