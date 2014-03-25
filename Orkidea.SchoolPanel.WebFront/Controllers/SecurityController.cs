using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;
using Orkidea.SchoolPanel.Utilities;
using System.Web.Security;
using System.Security.Principal;
using Orkidea.SchoolPanel.Business;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class SecurityController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            Cryptography oCrypto = new Cryptography();
            if (!String.IsNullOrEmpty(model.UserName) && !String.IsNullOrEmpty(model.Password))
            {
                //UserBiz userBiz = new UserBiz();
                //User userTarget = userBiz.GetUserbyKey(new User() { usuario = model.UserName });

                PersonBiz personBiz = new PersonBiz();
                Person personTarget = personBiz.GetPersonByUserName(new Person { usuario = model.UserName });


                if (personTarget == null)
                    return View(model);

                //TeacherBiz teacherBiz = new TeacherBiz();
                //StudentBiz studentBiz = new StudentBiz();

                string contraseñaDesencriptada = oCrypto.Decrypt(personTarget.password);


                if (model.Password.Equals(contraseñaDesencriptada))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);

                    int id = personTarget.id;
                    int idRole = personTarget.idRol;
                    int idColegio = personTarget.idColegio;

                    //if (userTarget.Rol > 1 && userTarget.Rol < 4)
                    //{
                    //    Teacher teacher = teacherBiz.GetTeacherbyId(new Teacher() { id = id });
                    //    idColegio = teacher.idColegio;
                    //}

                    //if (userTarget.Rol == 4)
                    //{
                    //    Student student = studentBiz.GetStudentbyId(new Student() { id = id });
                    //    idColegio = student.idColegio;
                    //}

                    string userData = id.ToString().Trim() + "|" + idRole.ToString().Trim() + "|" + idColegio.ToString().Trim();
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, userData);

                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    HttpContext.Response.Cookies.Add(faCookie);

                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        CustomIdentity identity = new CustomIdentity(authTicket.Name, userData);
                        GenericPrincipal newUser = new GenericPrincipal(identity, new string[] { });
                        HttpContext.User = newUser;
                    }

                    return RedirectToLocal(returnUrl);
                }
            }

            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                //return RedirectToAction("Index", "Home");
                return RedirectToAction
                ("Login");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //[Authorize]
        //public ActionResult SchoolAdminUserIndex(int id)
        //{
            PersonBiz personBiz = new PersonBiz();
            //UserBiz userBiz = new UserBiz();
            //TeacherBiz teacherBiz = new TeacherBiz();

            //List<User> lstUser = userBiz.GetUserListByRol(new User() { Rol = 2 });
            //List<Teacher> lstTeacher = teacherBiz.GetTeacherList(new School() { id = id });

            //List<User> lstUser = userBiz.GetUserListByRol(new User() { Rol = 2 });

            //vmUser adminUser = new vmUser();
            //adminUser.idColegio = id;


            //foreach (User item in lstUser)
            //{
            //    Teacher teacher = (from x in lstTeacher where x.id == item.idTabla select x).FirstOrDefault();

            //    if (teacher != null)
            //    {
            //        adminUser.lstUser.Add(new vmUser()
            //        {
            //            usuario = item.usuario,
            //            activo = item.activo,
            //            contraseña = item.contraseña,
            //            idTabla = item.idTabla,
            //            Rol = item.Rol,
            //            nombre =
            //            teacher.primerNombre + " " + (string.IsNullOrEmpty(teacher.segundoNombre) ? "" : (teacher.segundoNombre + " ")) +
            //            teacher.primerApellido + (string.IsNullOrEmpty(teacher.segundoApellido) ? "" : (" " + teacher.segundoApellido))
            //        });
            //    }
            //}

        //    ViewBag.id = id;

        //    return View(adminUser);
        //}

        //[Authorize]
        //public ActionResult TeacherUserIndex(int id)
        //{
        //    #region School identification
        //    System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //    int idColegio = 0;

        //    if (context.IsAuthenticated)
        //    {
        //        System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //        string[] userRole = ci.Ticket.UserData.Split('|');
        //        idColegio = int.Parse(userRole[2]);
        //    }

        //    School school = new School() { id = idColegio };
        //    #endregion

        //    UserBiz userBiz = new UserBiz();
        //    TeacherBiz teacherBiz = new TeacherBiz();

        //    List<User> lstUser = userBiz.GetUserList();//ByRol(new User() { Rol = 3 });
        //    List<Teacher> lstTeacher = teacherBiz.GetTeacherList(new School() { id = idColegio });
        //    List<vmUser> lstTeacherUser = new List<vmUser>();

        //    vmUser adminUser = new vmUser();
        //    adminUser.idColegio = id;

        //    foreach (User item in lstUser)
        //    {
        //        Teacher teacher = (from x in lstTeacher where x.id == item.idTabla select x).FirstOrDefault();

        //        if (teacher != null)
        //        {
        //            adminUser.lstUser.Add(new vmUser()
        //            {
        //                usuario = item.usuario,
        //                activo = item.activo,
        //                contraseña = item.contraseña,
        //                idTabla = item.idTabla,
        //                Rol = item.Rol,
        //                nombre =
        //                teacher.primerNombre + " " + (string.IsNullOrEmpty(teacher.segundoNombre) ? "" : (teacher.segundoNombre + " ")) +
        //                teacher.primerApellido + (string.IsNullOrEmpty(teacher.segundoApellido) ? "" : (" " + teacher.segundoApellido))
        //            });
        //        }
        //    }

        //    return View(adminUser);
        //}

        //[Authorize]
        //public ActionResult TeacherNoUserIndex(int id)
        //{
        //    UserBiz userBiz = new UserBiz();
        //    TeacherBiz teacherBiz = new TeacherBiz();

        //    List<User> lstUserTeacher = userBiz.GetUserListByRol(new User() { Rol = 3 });
        //    List<User> lstUserAdminSchool = userBiz.GetUserListByRol(new User() { Rol = 2 });

        //    List<Teacher> lstTeacher = teacherBiz.GetTeacherList(new School() { id = id });
        //    List<vmUser> lstTeacherUser = new List<vmUser>();

        //    vmUser adminUser = new vmUser();
        //    adminUser.idColegio = id;

        //    foreach (Teacher item in lstTeacher)
        //    {
        //        int hit = (from x in lstUserTeacher where x.idTabla == item.id select x).Count() + (from x in lstUserAdminSchool where x.idTabla == item.id select x).Count();

        //        if (hit == 0)
        //        {
        //            adminUser.lstUser.Add(new vmUser()
        //            {

        //                idTabla = item.id,
        //                nombre =
        //                item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : (item.segundoNombre + " ")) +
        //                item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : (" " + item.segundoApellido))
        //            });
        //        }
        //    }

        //    return View(adminUser);
        //}

        //[Authorize]
        //public ActionResult StudentUserIndex()
        //{
        //    #region School identification
        //    System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //    int idColegio = 0;

        //    if (context.IsAuthenticated)
        //    {
        //        System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //        string[] userRole = ci.Ticket.UserData.Split('|');
        //        idColegio = int.Parse(userRole[2]);
        //    }

        //    School school = new School() { id = idColegio };
        //    #endregion

        //    UserBiz userBiz = new UserBiz();
        //    StudentBiz studentBiz = new StudentBiz();

        //    List<User> lstUser = userBiz.GetUserListByRol(new User() { Rol = 4 });
        //    List<Student> lstStudent = studentBiz.GetStudentList(new School() { id = school.id });
        //    vmUser studentUser = new vmUser();

        //    foreach (User item in lstUser)
        //    {
        //        Student student = (from x in lstStudent where x.id == item.idTabla select x).FirstOrDefault();

        //        if (student != null)
        //        {
        //            studentUser.lstUser.Add(new vmUser()
        //            {
        //                usuario = item.usuario,
        //                activo = item.activo,
        //                contraseña = item.contraseña,
        //                idTabla = item.idTabla,
        //                Rol = item.Rol,
        //                nombre =
        //                student.primerNombre + " " + (string.IsNullOrEmpty(student.segundoNombre) ? "" : (student.segundoNombre + " ")) +
        //                student.primerApellido + (string.IsNullOrEmpty(student.segundoApellido) ? "" : (" " + student.segundoApellido))
        //            });
        //        }
        //    }

        //    return View(studentUser);
        //}

        //[Authorize]
        //public ActionResult StudentNoUserIndex()
        //{
        //    #region School identification
        //    System.Security.Principal.IIdentity context = HttpContext.User.Identity;
        //    int idColegio = 0;

        //    if (context.IsAuthenticated)
        //    {
        //        System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
        //        string[] userRole = ci.Ticket.UserData.Split('|');
        //        idColegio = int.Parse(userRole[2]);
        //    }

        //    School school = new School() { id = idColegio };
        //    #endregion

        //    UserBiz userBiz = new UserBiz();
        //    StudentBiz studentBiz = new StudentBiz();

        //    List<User> lstUser = userBiz.GetUserListByRol(new User(){ Rol = 4 });
        //    List<Student> lstStudent = studentBiz.GetStudentList(new School() { id = school.id });
        //    List<vmUser> lstStudentUser = new List<vmUser>();

        //    vmUser adminUser = new vmUser();
        //    adminUser.idColegio = school.id;

        //    foreach (Student item in lstStudent)
        //    {
        //        int hit = (from x in lstUser where x.idTabla == item.id select x).Count();

        //        if (hit == 0)
        //        {
        //            adminUser.lstUser.Add(new vmUser()
        //            {

        //                idTabla = item.id,
        //                nombre =
        //                item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : (item.segundoNombre + " ")) +
        //                item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : (" " + item.segundoApellido))
        //            });
        //        }
        //    }

        //    return View(adminUser);
        //}

        //[Authorize]
        //public ActionResult NewTeacher(int id)
        //{
        //    Teacher oTeacher = new Teacher() { idColegio = id };

        //    return View(oTeacher);
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult NewTeacher(int id, Teacher teacher)
        //{
        //    TeacherBiz teacherBiz = new TeacherBiz();
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        Teacher newTeacher = new Teacher()
        //        { 
        //            idColegio = id, 
        //            cedula= teacher.cedula,
        //            primerNombre = teacher.primerNombre,
        //            segundoNombre= teacher.segundoNombre,
        //            primerApellido = teacher.primerApellido,
        //            segundoApellido = teacher.segundoApellido                    
        //        };

        //        teacherBiz.SaveTeacher(newTeacher);
        //        return RedirectToAction("TeacherNoUserIndex", new { id = teacher.idColegio });
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        [Authorize]
        public ActionResult SaveUser(Person person)
        {
            //UserBiz userBiz = new UserBiz();
            try
            {
                Cryptography oCrypto = new Cryptography();

                if (string.IsNullOrEmpty(person.password))
                    person.password = oCrypto.Encrypt(person.usuario.Trim() + DateTime.Now.Year.ToString());

                personBiz.SavePerson(person);

                if (person.idRol == 3 || person.idRol == 4)
                {
                    //TeacherBiz teacherBiz = new TeacherBiz();
                    //Teacher teacher = teacherBiz.GetTeacherbyId(new Teacher() { id = person.idTabla });

                    return RedirectToAction("TeacherUserIndex", new { id = person.idColegio });
                }

                if (person.idRol == 5)
                {
                    //StudentBiz studentBiz = new StudentBiz();
                    //Student student = studentBiz.GetStudentbyId(new Student() { id = person.idTabla });

                    return RedirectToAction("StudentUserIndex", new { id = person.idColegio });
                }

            }
            catch
            {
                return View();
            }

            return View();
        }

        //[Authorize]
        //public ActionResult DeleteUser(User user)
        //{
        //    UserBiz userBiz = new UserBiz();
        //    try
        //    {
        //        userBiz.DeleteUser(user);

        //        if (user.Rol > 1 && user.Rol < 4)
        //        {
        //            TeacherBiz teacherBiz = new TeacherBiz();
        //            Teacher teacher = teacherBiz.GetTeacherbyId(new Teacher() { id = user.idTabla });

        //            return RedirectToAction("TeacherUserIndex", new { id = teacher.idColegio });
        //        }

        //        if (user.Rol == 4)
        //        {
        //            StudentBiz studentBiz = new StudentBiz();
        //            Student student = studentBiz.GetStudentbyId(new Student() { id = user.idTabla });

        //            return RedirectToAction("StudentUserIndex", new { id = student.idColegio });
        //        }

        //    }
        //    catch
        //    {
        //        return View();
        //    }

        //    return View();
        //}

        [Authorize]
        public ActionResult ChangePassword()
        {
            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;
            int usuario = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                usuario = int.Parse(userRole[0]);
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            //UserBiz userBiz = new UserBiz();

            Person person = personBiz.GetPersonByUserName(new Person() { usuario = HttpContext.User.Identity.Name });

            //User user = userBiz.GetUserbyKey(new User() { usuario = HttpContext.User.Identity.Name });
            //user.contraseña = "";
            return View(person);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(Person personTarget)
        {
            Cryptography oCrypto = new Cryptography();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;
            int usuario = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                usuario = int.Parse(userRole[0]);
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            //UserBiz userBiz = new UserBiz();

            Person person = personBiz.GetPersonByUserName(new Person() { usuario = HttpContext.User.Identity.Name });
            
            person.password = oCrypto.Encrypt(personTarget.password);

            SaveUser(person);
            return RedirectToAction("Index","Home");
        }

        [AllowAnonymous]
        public JsonResult ResetPassword(string usuario)
        {
            string resultado = "No se pudo resetear el usuario.";
            try
            {
                Cryptography oCrypto = new Cryptography();

                Person person = personBiz.GetPersonByUserName(new Person() { usuario = usuario });
                person.password = oCrypto.Encrypt(person.documento);

                SaveUser(person);

                resultado = "Clave reseteada con éxito";
            }
            catch (Exception)
            {
                
            }

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

    }
}
