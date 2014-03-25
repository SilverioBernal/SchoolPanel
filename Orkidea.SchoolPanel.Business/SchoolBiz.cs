using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Utilities;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

namespace Orkidea.SchoolPanel.Business
{
    public class SchoolBiz
    {
        /*CRUD Schools*/

        /// <summary>
        /// Retrieve school list without parameters
        /// </summary>
        /// <returns></returns>
        public List<School> GetSchoolList()
        {

            List<School> lstSchool = new List<School>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstSchool = ctx.Schools.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstSchool;
        }

        /// <summary>
        /// Retrieve school information based in the primary key
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public School GetSchoolbyKey(School schoolTarget)
        {
            School oSchool = new School();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oSchool = ctx.Schools.Where(x => x.id.Equals(schoolTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oSchool;
        }

        /// <summary>
        /// Create or update a school
        /// </summary>
        /// <param name="schoolTarget"></param>
        public void SaveSchool(School schoolTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    School oSchool = GetSchoolbyKey(schoolTarget);

                    if (oSchool != null)
                    {
                        // if exists then edit 
                        ctx.Schools.Attach(oSchool);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oSchool, schoolTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create

                        Cryptography cryptography = new Cryptography();

                        List<Person> admin = new List<Person>();

                        admin.Add(new Person()
                        {
                            idJornada = 1,
                            idRol = 2,
                            usuario = schoolTarget.nit.Replace(".", "").Replace("-","").Replace(" ",""), 
                            password = cryptography.Encrypt(schoolTarget.telefono.Replace(".", "").Replace("-","").Replace(" ","")),
                            usuarioActivo = true,
                            primerNombre = "Default",
                            primerApellido = "admin",
                            tipoDocumento = "OT",
                            documento = schoolTarget.nit,
                            telefono = schoolTarget.telefono,
                            direccion = schoolTarget.direccion,
                            ciudad = "Default",
                            retirado = false
                        });

                        schoolTarget.Places.Add(
                            new Place()
                            {
                                descripcion = schoolTarget.nombreColegio,
                                direccion = schoolTarget.direccion,
                                telefono = schoolTarget.telefono,
                                People = admin
                            });


                        ctx.Schools.Add(schoolTarget);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (DbEntityValidationException ex)
            {
                string error = "";
                foreach (var eve in ex.EntityValidationErrors)
                {

                    error = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        error += string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a school
        /// </summary>
        /// <param name="schoolTarget"></param>
        public void DeleteSchool(School schoolTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    School oSchool = GetSchoolbyKey(schoolTarget);

                    if (!string.IsNullOrEmpty(oSchool.nombreColegio))
                    {
                        AcademicPeriodBiz academicPeriodBiz = new AcademicPeriodBiz();
                        List<AcademicPeriod> lsAP = academicPeriodBiz.GetAcademicPeriodList(oSchool);

                        if (lsAP.Count() == 0)
                        {
                            PersonBiz personBiz = new PersonBiz();
                            List<Person> lsPerson = personBiz.GetPersonList(oSchool);

                            foreach (Person item in lsPerson)
                                personBiz.DeletePerson(item);

                            PlaceBiz placeBiz = new PlaceBiz();
                            List<Place> lsPlace = placeBiz.GetPlaceList(oSchool);

                            foreach (Place item in lsPlace)
                                placeBiz.DeletePlace(item);

                            // if exists then edit 
                            ctx.Schools.Attach(oSchool);
                            ctx.Schools.Remove(oSchool);
                            ctx.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }
    }
}
