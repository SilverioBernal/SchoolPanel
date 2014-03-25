using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Orkidea.SchoolPanel.Business;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.WebFront.Models;

namespace Orkidea.SchoolPanel.WebFront.Controllers
{
    public class PersonController : Controller
    {
        /*Estudiantes*/
        PersonBiz personBiz = new PersonBiz();
        //
        // GET: /Person/
        [Authorize]
        public ActionResult IndexAllSchoolStudent()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<vmPerson> lsVmPerson = new List<vmPerson>();
            List<Person> lsPerson = personBiz.GetPersonList(school, 5);

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Retirado" : "Activo"
                };

                switch (item.idJornada)
                {
                    case 1:
                        person.nombreJornada = "Mañana";
                        break;
                    case 2:
                        person.nombreJornada = "Tarde";
                        break;
                    case 3:
                        person.nombreJornada = "Noche";
                        break;
                    case 4:
                        person.nombreJornada = "Única";
                        break;
                    default:
                        break;
                }

                person.nombreSede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson);
        }

        //
        // GET: /Person/Create
        [Authorize]
        public ActionResult CreateStudent()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            vmPerson newPerson = new vmPerson();
            newPerson.lsPlace = placeBiz.GetPlaceList(school);
            return View(newPerson);
        }

        //
        // POST: /Person/Create
        [Authorize]
        [HttpPost]
        public ActionResult CreateStudent(vmPerson newPerson)
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            try
            {
                // TODO: Add insert logic here
                Person person = new Person()
                {
                    tipoDocumento = newPerson.tipoDocumento,
                    documento = newPerson.documento,
                    primerNombre = newPerson.primerNombre,
                    segundoNombre = newPerson.segundoNombre,
                    primerApellido = newPerson.primerApellido,
                    segundoApellido = newPerson.segundoApellido,
                    sexo = newPerson.sexo,
                    retirado = false,
                    usuarioActivo = true,
                    idRol = 5,

                    ciudad = newPerson.ciudad,
                    direccion = newPerson.direccion,
                    telefono = newPerson.telefono,
                    email = newPerson.email,

                    madre = newPerson.madre,
                    telefonoMadre = newPerson.telefonoMadre,

                    padre = newPerson.padre,
                    telefonoPadre = newPerson.telefonoPadre,
                    nombreOtroContacto = newPerson.nombreOtroContacto,
                    telefonoOtroContacto = newPerson.telefonoOtroContacto,

                    idColegio = school.id,
                    idSede = newPerson.idSede,
                    idJornada = newPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexAllSchoolStudent");
            }
            catch
            {
                newPerson.lsPlace = placeBiz.GetPlaceList(school);
                return View(newPerson);
            }
        }

        //
        // GET: /Person/Edit/5
        [Authorize]
        public ActionResult EditStudent(int id)
        {
            PlaceBiz placeBiz = new PlaceBiz();
            Person person = personBiz.GetPersonByKey(new Person() { id = id });
            vmPerson editPerson = new vmPerson() 
            {
                tipoDocumento = person.tipoDocumento,
                documento = person.documento,
                primerNombre = person.primerNombre,
                segundoNombre = person.segundoNombre,
                primerApellido = person.primerApellido,
                segundoApellido = person.segundoApellido,
                sexo = person.sexo,
                retirado = person.retirado,
                usuarioActivo = person.usuarioActivo,
                usuario = person.usuario,                
                idRol = 5,

                ciudad = person.ciudad,
                direccion = person.direccion,
                telefono = person.telefono,
                email = person.email,

                madre = person.madre,
                telefonoMadre = person.telefonoMadre,

                padre = person.padre,
                telefonoPadre = person.telefonoPadre,
                nombreOtroContacto = person.nombreOtroContacto,
                telefonoOtroContacto = person.telefonoOtroContacto,

                idColegio = person.idColegio,
                idSede = person.idSede,
                idJornada = person.idJornada
            };

            editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = person.idColegio});

            return View(editPerson);
        }

        //
        // POST: /Person/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult EditStudent(int id, vmPerson editPerson)
        {            
            try
            {
                Person storedPerson = personBiz.GetPersonByKey(new Person() { id = id });

                // TODO: Add insert logic here
                Person person = new Person()
                {
                    id = id,
                    tipoDocumento = editPerson.tipoDocumento,
                    documento = editPerson.documento,
                    primerNombre = editPerson.primerNombre,
                    segundoNombre = editPerson.segundoNombre,
                    primerApellido = editPerson.primerApellido,
                    segundoApellido = editPerson.segundoApellido,
                    sexo = editPerson.sexo,
                    retirado = editPerson.retirado,
                    usuarioActivo = editPerson.usuarioActivo,
                    usuario = editPerson.usuario,
                    password = storedPerson.password,
                    idRol = 5,

                    ciudad = editPerson.ciudad,
                    direccion = editPerson.direccion,
                    telefono = editPerson.telefono,
                    email = editPerson.email,

                    madre = editPerson.madre,
                    telefonoMadre = editPerson.telefonoMadre,

                    padre = editPerson.padre,
                    telefonoPadre = editPerson.telefonoPadre,
                    nombreOtroContacto = editPerson.nombreOtroContacto,
                    telefonoOtroContacto = editPerson.telefonoOtroContacto,

                    idColegio = editPerson.idColegio ,
                    idSede = editPerson.idSede,
                    idJornada = editPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexAllSchoolStudent");
            }
            catch
            {
                PlaceBiz placeBiz = new PlaceBiz();
                editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = editPerson.idColegio });
                return View(editPerson);
            }
        }

        //
        // GET: /Person/Delete/5
        [Authorize]
        public JsonResult DeleteStudent(int id)
        {
            string Res = "";
            try
            {
                personBiz.DeletePerson(new Person() { id = id });
                Res = "OK";
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }

            return Json(Res, JsonRequestBehavior.AllowGet);
        }

        /*Profesores*/
        [Authorize]
        public ActionResult IndexSchoolTeacher()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<vmPerson> lsVmPerson = new List<vmPerson>();
            List<Person> lsPerson = personBiz.GetPersonList(school, 4);

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Si" : "No",
                    desActivo = item.usuarioActivo ? "Activo" : "Inactivo", 
                };

                switch (item.idJornada)
                {
                    case 1:
                        person.nombreJornada = "Mañana";
                        break;
                    case 2:
                        person.nombreJornada = "Tarde";
                        break;
                    case 3:
                        person.nombreJornada = "Noche";
                        break;
                    case 4:
                        person.nombreJornada = "Única";
                        break;
                    default:
                        break;
                }

                person.nombreSede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson.OrderBy(x=> x.nombreCompleto).ToList());
        }        

        //
        // GET: /Person/Create
        [Authorize]
        public ActionResult CreateTeacher()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            vmPerson newPerson = new vmPerson();
            newPerson.lsPlace = placeBiz.GetPlaceList(school);
            return View(newPerson);
        }

        //
        // POST: /Person/Create
        [Authorize]
        [HttpPost]
        public ActionResult CreateTeacher(vmPerson newPerson)
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion
            try
            {

                // TODO: Add insert logic here
                Person person = new Person()
                {
                    tipoDocumento = newPerson.tipoDocumento,
                    documento = newPerson.documento,
                    primerNombre = newPerson.primerNombre,
                    segundoNombre = newPerson.segundoNombre,
                    primerApellido = newPerson.primerApellido,
                    segundoApellido = newPerson.segundoApellido,
                    sexo = newPerson.sexo,
                    retirado = false,
                    usuarioActivo = true,
                    idRol = 4,

                    ciudad = newPerson.ciudad,
                    direccion = newPerson.direccion,
                    telefono = newPerson.telefono,
                    email = newPerson.email,

                    madre = newPerson.madre,
                    telefonoMadre = newPerson.telefonoMadre,

                    padre = newPerson.padre,
                    telefonoPadre = newPerson.telefonoPadre,
                    nombreOtroContacto = newPerson.nombreOtroContacto,
                    telefonoOtroContacto = newPerson.telefonoOtroContacto,

                    idColegio = school.id,
                    idSede = newPerson.idSede,
                    idJornada = newPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolTeacher");
            }
            catch
            {
                newPerson.lsPlace = placeBiz.GetPlaceList(school);
                return View(newPerson);
            }
        }

        //
        // GET: /Person/Edit/5
        [Authorize]
        public ActionResult EditTeacher(int id)
        {
            PlaceBiz placeBiz = new PlaceBiz();
            Person person = personBiz.GetPersonByKey(new Person() { id = id });
            vmPerson editPerson = new vmPerson()
            {
                tipoDocumento = person.tipoDocumento,
                documento = person.documento,
                primerNombre = person.primerNombre,
                segundoNombre = person.segundoNombre,
                primerApellido = person.primerApellido,
                segundoApellido = person.segundoApellido,
                sexo = person.sexo,
                retirado = person.retirado,
                usuarioActivo = person.usuarioActivo,
                usuario = person.usuario,
                idRol = person.idRol,

                ciudad = person.ciudad,
                direccion = person.direccion,
                telefono = person.telefono,
                email = person.email,                

                idColegio = person.idColegio,
                idSede = person.idSede,
                idJornada = person.idJornada
            };

            editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = person.idColegio });

            return View(editPerson);
        }

        //
        // POST: /Person/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult EditTeacher(int id, vmPerson editPerson)
        {
            try
            {
                Person storedPerson = personBiz.GetPersonByKey(new Person() { id = id });

                // TODO: Add insert logic here
                Person person = new Person()
                {
                    id = id,
                    tipoDocumento = editPerson.tipoDocumento,
                    documento = editPerson.documento,
                    primerNombre = editPerson.primerNombre,
                    segundoNombre = editPerson.segundoNombre,
                    primerApellido = editPerson.primerApellido,
                    segundoApellido = editPerson.segundoApellido,
                    sexo = editPerson.sexo,
                    retirado = editPerson.retirado,
                    usuarioActivo = editPerson.usuarioActivo,
                    usuario = editPerson.usuario,
                    password = storedPerson.password,
                    idRol = 4,

                    ciudad = editPerson.ciudad,
                    direccion = editPerson.direccion,
                    telefono = editPerson.telefono,
                    email = editPerson.email,                    

                    idColegio = editPerson.idColegio,
                    idSede = editPerson.idSede,
                    idJornada = editPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolTeacher");
            }
            catch
            {
                PlaceBiz placeBiz = new PlaceBiz();
                editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = editPerson.idColegio });
                return View(editPerson);                
            }
        }

        //
        // GET: /Person/Delete/5
        [Authorize]
        public JsonResult DeleteTeacher(int id)
        {
            string Res = "";
            try
            {
                personBiz.DeletePerson(new Person() { id = id });
                Res = "OK";
            }
            catch (Exception ex)
            {
                Res = ex.Message;
            }

            return Json(Res, JsonRequestBehavior.AllowGet);
        }

        /*Coordinadores*/
        [Authorize]
        public ActionResult IndexSchoolCoordinator()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<vmPerson> lsVmPerson = new List<vmPerson>();
            List<Person> lsPerson = personBiz.GetPersonList(school, 3);

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Inactivo" : "Activo"
                };

                switch (item.idJornada)
                {
                    case 1:
                        person.nombreJornada = "Mañana";
                        break;
                    case 2:
                        person.nombreJornada = "Tarde";
                        break;
                    case 3:
                        person.nombreJornada = "Noche";
                        break;
                    case 4:
                        person.nombreJornada = "Única";
                        break;
                    default:
                        break;
                }

                person.nombreSede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson);
        }

        //
        // GET: /Person/Create
        [Authorize]
        public ActionResult CreateCoordinator()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            vmPerson newPerson = new vmPerson();
            newPerson.lsPlace = placeBiz.GetPlaceList(school);
            return View(newPerson);
        }

        //
        // POST: /Person/Create
        [Authorize]
        [HttpPost]
        public ActionResult CreateCoordinator(vmPerson newPerson)
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            try
            {             
                // TODO: Add insert logic here
                Person person = new Person()
                {
                    tipoDocumento = newPerson.tipoDocumento,
                    documento = newPerson.documento,
                    primerNombre = newPerson.primerNombre,
                    segundoNombre = newPerson.segundoNombre,
                    primerApellido = newPerson.primerApellido,
                    segundoApellido = newPerson.segundoApellido,
                    sexo = newPerson.sexo,
                    retirado = false,
                    usuarioActivo = true,
                    idRol = 3,

                    ciudad = newPerson.ciudad,
                    direccion = newPerson.direccion,
                    telefono = newPerson.telefono,
                    email = newPerson.email,

                    madre = newPerson.madre,
                    telefonoMadre = newPerson.telefonoMadre,

                    padre = newPerson.padre,
                    telefonoPadre = newPerson.telefonoPadre,
                    nombreOtroContacto = newPerson.nombreOtroContacto,
                    telefonoOtroContacto = newPerson.telefonoOtroContacto,

                    idColegio = school.id,
                    idSede = newPerson.idSede,
                    idJornada = newPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolCoordinator");
            }
            catch
            {
                newPerson.lsPlace = placeBiz.GetPlaceList(school);
                return View(newPerson);
            }
        }

        //
        // GET: /Person/Edit/5
        [Authorize]
        public ActionResult EditCoordinator(int id)
        {
            PlaceBiz placeBiz = new PlaceBiz();
            Person person = personBiz.GetPersonByKey(new Person() { id = id });
            vmPerson editPerson = new vmPerson()
            {
                tipoDocumento = person.tipoDocumento,
                documento = person.documento,
                primerNombre = person.primerNombre,
                segundoNombre = person.segundoNombre,
                primerApellido = person.primerApellido,
                segundoApellido = person.segundoApellido,
                sexo = person.sexo,
                retirado = person.retirado,
                usuarioActivo = person.usuarioActivo,
                usuario = person.usuario,
                idRol = person.idRol,

                ciudad = person.ciudad,
                direccion = person.direccion,
                telefono = person.telefono,
                email = person.email,

                idColegio = person.idColegio,
                idSede = person.idSede,
                idJornada = person.idJornada
            };

            editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = person.idColegio });

            return View(editPerson);
        }

        //
        // POST: /Person/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult EditCoordinator(int id, vmPerson editPerson)
        {
            try
            {
                Person storedPerson = personBiz.GetPersonByKey(new Person() { id = id });

                // TODO: Add insert logic here
                Person person = new Person()
                {
                    id = id,
                    tipoDocumento = editPerson.tipoDocumento,
                    documento = editPerson.documento,
                    primerNombre = editPerson.primerNombre,
                    segundoNombre = editPerson.segundoNombre,
                    primerApellido = editPerson.primerApellido,
                    segundoApellido = editPerson.segundoApellido,
                    sexo = editPerson.sexo,
                    retirado = editPerson.retirado,
                    usuarioActivo = editPerson.usuarioActivo,
                    usuario = editPerson.usuario,
                    password = storedPerson.password,
                    idRol = 3,

                    ciudad = editPerson.ciudad,
                    direccion = editPerson.direccion,
                    telefono = editPerson.telefono,
                    email = editPerson.email,

                    idColegio = editPerson.idColegio,
                    idSede = editPerson.idSede,
                    idJornada = editPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolCoordinator");
            }
            catch
            {
                PlaceBiz placeBiz = new PlaceBiz();
                editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = editPerson.idColegio });
                return View(editPerson);   
            }
        }

        //
        // GET: /Person/Delete/5
        [Authorize]
        public ActionResult DeleteCoordinator(int id)
        {
            personBiz.DeletePerson(new Person() { id = id });

            return RedirectToAction("IndexSchoolCoordinator");
        }

        /*Funcionarios*/
        [Authorize]
        public ActionResult IndexSchoolWorker()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            List<Place> lsPlace = placeBiz.GetPlaceList(school);
            List<vmPerson> lsVmPerson = new List<vmPerson>();
            List<Person> lsPerson = personBiz.GetPersonList(school, 6);

            foreach (Person item in lsPerson)
            {
                vmPerson person = new vmPerson()
                {
                    id = item.id,
                    nombreCompleto =
                    item.primerNombre + " " + (string.IsNullOrEmpty(item.segundoNombre) ? "" : item.segundoNombre + " ") +
                    item.primerApellido + (string.IsNullOrEmpty(item.segundoApellido) ? "" : " " + item.segundoApellido),
                    documento = item.documento,
                    usuario = item.usuario,
                    desRetirado = item.retirado ? "Inactivo" : "Activo"
                };

                switch (item.idJornada)
                {
                    case 1:
                        person.nombreJornada = "Mañana";
                        break;
                    case 2:
                        person.nombreJornada = "Tarde";
                        break;
                    case 3:
                        person.nombreJornada = "Noche";
                        break;
                    case 4:
                        person.nombreJornada = "Única";
                        break;
                    default:
                        break;
                }

                person.nombreSede = lsPlace.Where(x => x.id.Equals(item.idSede)).Select(x => x.descripcion).First();

                lsVmPerson.Add(person);
            }

            return View(lsVmPerson);
        }

        //
        // GET: /Person/Create
        [Authorize]
        public ActionResult CreateWorker()
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            vmPerson newPerson = new vmPerson();
            newPerson.lsPlace = placeBiz.GetPlaceList(school);
            return View(newPerson);
        }

        //
        // POST: /Person/Create
        [Authorize]
        [HttpPost]
        public ActionResult CreateWorker(vmPerson newPerson)
        {
            PlaceBiz placeBiz = new PlaceBiz();

            #region School identification
            System.Security.Principal.IIdentity context = HttpContext.User.Identity;
            int idColegio = 0;

            if (context.IsAuthenticated)
            {
                System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                string[] userRole = ci.Ticket.UserData.Split('|');
                idColegio = int.Parse(userRole[2]);
            }

            School school = new School() { id = idColegio };
            #endregion

            try
            {
                // TODO: Add insert logic here
                Person person = new Person()
                {
                    tipoDocumento = newPerson.tipoDocumento,
                    documento = newPerson.documento,
                    primerNombre = newPerson.primerNombre,
                    segundoNombre = newPerson.segundoNombre,
                    primerApellido = newPerson.primerApellido,
                    segundoApellido = newPerson.segundoApellido,
                    sexo = newPerson.sexo,
                    retirado = false,
                    usuarioActivo = true,
                    idRol = 6,

                    ciudad = newPerson.ciudad,
                    direccion = newPerson.direccion,
                    telefono = newPerson.telefono,
                    email = newPerson.email,

                    madre = newPerson.madre,
                    telefonoMadre = newPerson.telefonoMadre,

                    padre = newPerson.padre,
                    telefonoPadre = newPerson.telefonoPadre,
                    nombreOtroContacto = newPerson.nombreOtroContacto,
                    telefonoOtroContacto = newPerson.telefonoOtroContacto,

                    idColegio = school.id,
                    idSede = newPerson.idSede,
                    idJornada = newPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolWorker");
            }
            catch
            {
                newPerson.lsPlace = placeBiz.GetPlaceList(school);
                return View(newPerson);
            }
        }

        //
        // GET: /Person/Edit/5
        [Authorize]
        public ActionResult EditWorker(int id)
        {
            PlaceBiz placeBiz = new PlaceBiz();
            Person person = personBiz.GetPersonByKey(new Person() { id = id });
            vmPerson editPerson = new vmPerson()
            {
                tipoDocumento = person.tipoDocumento,
                documento = person.documento,
                primerNombre = person.primerNombre,
                segundoNombre = person.segundoNombre,
                primerApellido = person.primerApellido,
                segundoApellido = person.segundoApellido,
                sexo = person.sexo,
                retirado = person.retirado,
                usuarioActivo = person.usuarioActivo,
                usuario = person.usuario,
                idRol = person.idRol,

                ciudad = person.ciudad,
                direccion = person.direccion,
                telefono = person.telefono,
                email = person.email,

                idColegio = person.idColegio,
                idSede = person.idSede,
                idJornada = person.idJornada
            };

            editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = person.idColegio });

            return View(editPerson);
        }

        //
        // POST: /Person/Edit/5
        [Authorize]
        [HttpPost]
        public ActionResult EditWorker(int id, vmPerson editPerson)
        {
            try
            {
                Person storedPerson = personBiz.GetPersonByKey(new Person() { id = id });

                // TODO: Add insert logic here
                Person person = new Person()
                {
                    id = id,
                    tipoDocumento = editPerson.tipoDocumento,
                    documento = editPerson.documento,
                    primerNombre = editPerson.primerNombre,
                    segundoNombre = editPerson.segundoNombre,
                    primerApellido = editPerson.primerApellido,
                    segundoApellido = editPerson.segundoApellido,
                    sexo = editPerson.sexo,
                    retirado = editPerson.retirado,
                    usuarioActivo = editPerson.usuarioActivo,
                    usuario = editPerson.usuario,
                    password = storedPerson.password,
                    idRol = 6,

                    ciudad = editPerson.ciudad,
                    direccion = editPerson.direccion,
                    telefono = editPerson.telefono,
                    email = editPerson.email,

                    idColegio = editPerson.idColegio,
                    idSede = editPerson.idSede,
                    idJornada = editPerson.idJornada
                };

                personBiz.SavePerson(person);

                return RedirectToAction("IndexSchoolWorker");
            }
            catch
            {
                PlaceBiz placeBiz = new PlaceBiz();
                editPerson.lsPlace = placeBiz.GetPlaceList(new School() { id = editPerson.idColegio });
                return View(editPerson);   
            }
        }

        //
        // GET: /Person/Delete/5
        [Authorize]
        public ActionResult DeleteWorker(int id)
        {
            personBiz.DeletePerson(new Person() { id = id });

            return RedirectToAction("IndexSchoolWorker");
        }



        /* Reportes*/

        //public byte[] TeacherList(int curso, int estudiante, string rutaRpt)
        [Authorize]
        public FileContentResult TeacherList()
        {
            try
            {
                #region School identification
                System.Security.Principal.IIdentity context = HttpContext.User.Identity;
                int idColegio = 0;

                if (context.IsAuthenticated)
                {
                    System.Web.Security.FormsIdentity ci = (System.Web.Security.FormsIdentity)HttpContext.User.Identity;
                    string[] userRole = ci.Ticket.UserData.Split('|');
                    idColegio = int.Parse(userRole[2]);
                }

                School school = new School() { id = idColegio };
                #endregion

                string oConnStr = ConfigurationManager.ConnectionStrings["SchoolPanelADO"].ToString();
                string rutaRpt = Server.MapPath("~/Reporting/TeacherList.rpt");

                ReportDocument rpt;
                byte[] response = null;

                System.Data.SqlClient.SqlConnectionStringBuilder oConnBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(oConnStr);

                rpt = new ReportDocument();

                rpt.Load(rutaRpt);

                ParameterDiscreteValue colegioDiscreteValue = new ParameterDiscreteValue();
                colegioDiscreteValue.Value = idColegio;
                rpt.SetParameterValue("idColegio", colegioDiscreteValue);

                CrystalDecisions.Shared.ConnectionInfo connectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
                connectionInfo.DatabaseName = oConnBuilder.InitialCatalog;
                connectionInfo.UserID = oConnBuilder.UserID;
                connectionInfo.Password = oConnBuilder.Password;
                connectionInfo.ServerName = oConnBuilder.DataSource;

                Tables tables = rpt.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
                {
                    CrystalDecisions.Shared.TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                }

                for (int i = 0; i < rpt.DataSourceConnections.Count; i++)
                {
                    rpt.DataSourceConnections[i].SetConnection(oConnBuilder.DataSource, oConnBuilder.InitialCatalog, oConnBuilder.UserID, oConnBuilder.Password);
                }

                rpt.SetDatabaseLogon(oConnBuilder.UserID, oConnBuilder.Password, oConnBuilder.DataSource, oConnBuilder.InitialCatalog);


                System.IO.MemoryStream strMemory = (System.IO.MemoryStream)rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                response = new byte[strMemory.Length];

                strMemory.Read(response, 0, (int)strMemory.Length);

                return new FileContentResult(response, "application/pdf");
            }
            catch (Exception ex)
            {
                string error = ex.Message + " :::---::: " + ex.StackTrace;

                System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
                byte[] response = codificador.GetBytes(error);

                return new FileContentResult(response, "text/plain");
            }
        }

    }
}
