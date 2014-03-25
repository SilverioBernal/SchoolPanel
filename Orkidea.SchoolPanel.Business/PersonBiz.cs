using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class PersonBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<Person> GetPersonList()
        {

            List<Person> lstPerson = new List<Person>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPerson = ctx.Persons.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPerson;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="PersonTarget"></param>
        /// <returns></returns>
        public Person GetPersonByKey(Person PersonTarget)
        {
            Person oPerson = new Person();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oPerson = ctx.Persons.Where(x => x.id.Equals(PersonTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oPerson;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="personTarget"></param>
        public void SavePerson(Person personTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Person oPerson = GetPersonByKey(personTarget);

                    if (oPerson != null)
                    {
                        // if exists then edit 
                        ctx.Persons.Attach(oPerson);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oPerson, personTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        Cryptography cryptography = new Cryptography();
                        // else create
                        personTarget.usuario = GetNewUserName(personTarget);
                        personTarget.password = cryptography.Encrypt(personTarget.documento);
                        ctx.Persons.Add(personTarget);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (DbEntityValidationException e)
            {
                StringBuilder oError = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    oError.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    foreach (var ve in eve.ValidationErrors)
                    {
                        oError.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                string msg = oError.ToString();
                throw new Exception(msg);
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a record
        /// </summary>
        /// <param name="PersonTarget"></param>
        public void DeletePerson(Person PersonTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Person oPerson = GetPersonByKey(PersonTarget);

                    if (oPerson != null)
                    {
                        // if exists then edit 
                        ctx.Persons.Attach(oPerson);
                        ctx.Persons.Remove(oPerson);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar esta persona porque existe información asociada a esta.");
                }
            }
            catch (Exception ex) { throw ex; }
        }


        /*Complementary business methods*/

        public Person GetPersonByUserName(Person personTarget)
        {
            Person oPerson = new Person();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oPerson = ctx.Persons.Where(x => x.usuario== personTarget.usuario).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oPerson;
        }

        public List<Person> GetPersonList(School schoolTarget)
        {

            List<Person> lstPerson = new List<Person>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPerson = ctx.Persons.Where(x => x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPerson;
        }

        public List<Person> GetPersonList(Course courseTarget, int rolTarget)
        {

            /*
             * Roles por usuario
             * 1 - Administrador general (root)
             * 2 - administrador de colegio
             * 3 - coordinador 
             * 4 - profesor
             * 5 - estudiante
             * 6 - funcionario
             */
            List<Person> lstPersonReturn = new List<Person>();
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    List<Person> lstPerson = new List<Person>();

                    CourseBiz courseBiz = new CourseBiz();

                    courseTarget = courseBiz.GetCoursebyKey(courseTarget);

                    lstPerson = ctx.Persons.Where(x => x.idColegio.Equals(courseTarget.idColegio)).ToList();

                    if (rolTarget == 5)
                    {
                        List<CourseStudent> lstCourseStudent = new List<CourseStudent>();

                        lstCourseStudent = ctx.CourseStudents.Where(x => x.idCurso.Equals(courseTarget.id)).ToList();
                        

                        foreach (CourseStudent item in lstCourseStudent)
                        {
                            Person person = lstPerson.Where(x => x.id.Equals(item.idEstudiante)).First();
                            lstPersonReturn.Add(person);
                        }
                    }
                    else if (rolTarget == 3 || rolTarget == 4)
                    {
                        List<CourseAsignature> lstCourseAsignature = new List<CourseAsignature>();

                        lstCourseAsignature = ctx.CourseAsignatures.Where(x => x.idCurso.Equals(courseTarget.id)).ToList();                        

                        foreach (CourseAsignature item in lstCourseAsignature)
                        {
                            Person person = lstPerson.Where(x => x.id.Equals(item.idProfesor)).First();
                            lstPersonReturn.Add(person);
                        }
                    }
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPersonReturn;
        }

        public List<Person> GetPersonList(Grade gradeTarget, int rolTarget)
        {

            /*
             * Roles por usuario
             * 1 - Administrador general (root)
             * 2 - administrador de colegio
             * 3 - coordinador 
             * 4 - profesor
             * 5 - estudiante
             * 6 - funcionario
             */
            List<Person> lstPersonReturn = new List<Person>();
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    List<Person> lstPerson = new List<Person>();
                    List<Grade> lstGrade = new List<Grade>();

                    lstGrade = ctx.Grades.Where(x => x.id.Equals(gradeTarget.id)).ToList();
                    lstPerson = ctx.Persons.ToList();

                    foreach (Grade gradeItem in lstGrade)
                    {
                        if (rolTarget == 5)
                        {
                            List<CourseStudent> lstCourseStudent = new List<CourseStudent>();
                            lstCourseStudent = ctx.CourseStudents.Where(x => x.idCurso.Equals(gradeItem.id)).ToList();

                            foreach (CourseStudent item in lstCourseStudent)
                            {
                                Person person = lstPerson.Where(x => x.id.Equals(item.idEstudiante)).First();
                                lstPersonReturn.Add(person);
                            }
                        }
                        else if (rolTarget == 3 || rolTarget == 4)
                        {
                            List<CourseAsignature> lstCourseAsignature = new List<CourseAsignature>();
                            lstCourseAsignature = ctx.CourseAsignatures.Where(x => x.idCurso.Equals(gradeItem.id)).ToList();                            

                            foreach (CourseAsignature item in lstCourseAsignature)
                            {
                                Person person = lstPerson.Where(x => x.id.Equals(item.idProfesor)).First();
                                lstPersonReturn.Add(person);
                            }
                        }
                    }

                }
            }
            catch (Exception ex) { throw ex; }

            return lstPersonReturn;
        }

        public List<Person> GetPersonList(School schoolTarget, int rolTarget)
        {
            /*
             * Roles por usuario
             * 1 - Administrador general (root)
             * 2 - administrador de colegio
             * 3 - coordinador 
             * 4 - profesor
             * 5 - estudiante
             * 6 - funcionario
             */
            List<Person> lstPerson = new List<Person>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPerson = ctx.Persons.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idRol.Equals(rolTarget)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPerson;
        }

        public List<Person> GetPersonList(Place place, int rolTarget)
        {
            /*
             * Roles por usuario
             * 1 - Administrador general (root)
             * 2 - administrador de colegio
             * 3 - coordinador 
             * 4 - profesor
             * 5 - estudiante
             * 6 - funcionario
             */
            List<Person> lstPerson = new List<Person>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPerson = ctx.Persons.Where(x => x.idSede.Equals(place.id) && x.idRol.Equals(rolTarget)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPerson;
        }

        public List<Person> GetPersonList(Place place, int rolTarget, int jornada)
        {
            /*
             * Roles por usuario
             * 1 - Administrador general (root)
             * 2 - administrador de colegio
             * 3 - coordinador 
             * 4 - profesor
             * 5 - estudiante
             * 6 - funcionario
             */
            List<Person> lstPerson = new List<Person>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPerson = ctx.Persons.Where(x => x.idSede.Equals(place.id) && x.idRol.Equals(rolTarget) && x.idJornada.Equals(jornada)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPerson;
        }

        private string GetNewUserName(Person personTarget)
        {
            string UserName =
                personTarget.primerNombre.Substring(0, 1) +
                (string.IsNullOrEmpty(personTarget.segundoNombre) ? "" : personTarget.segundoNombre.Substring(0, 1)) +
                personTarget.primerApellido +
                (string.IsNullOrEmpty(personTarget.segundoApellido) ? "" : personTarget.segundoApellido.Substring(0, 1));
            

            for (int i = 1; i < 500; i++)
            {
                Person person = GetPersonByUserName(new Person() { usuario = UserName });

                if (person == null)
                    break;
                else
                {
                    UserName = UserName + i.ToString();
                }
            }

            return UserName.ToLower();
        }

    }
}
