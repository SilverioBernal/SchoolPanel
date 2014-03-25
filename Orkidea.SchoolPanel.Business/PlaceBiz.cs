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
    public class PlaceBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<Place> GetPlaceList(School schoolTarget)
        {

            List<Place> lstPlace = new List<Place>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstPlace = ctx.Places.Where(x => x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstPlace;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="PlaceTarget"></param>
        /// <returns></returns>
        public Place GetPlaceByKey(Place PlaceTarget)
        {
            Place oPlace = new Place();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oPlace = ctx.Places.Where(x => x.id.Equals(PlaceTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oPlace;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="PlaceTarget"></param>
        public void SavePlace(Place PlaceTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Place oPlace = GetPlaceByKey(PlaceTarget);

                    if (oPlace != null)
                    {
                        // if exists then edit 
                        ctx.Places.Attach(oPlace);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oPlace, PlaceTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Places.Add(PlaceTarget);
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
        /// <param name="PlaceTarget"></param>
        public void DeletePlace(Place PlaceTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    Place oPlace = GetPlaceByKey(PlaceTarget);

                    if (oPlace != null)
                    {
                        // if exists then edit 
                        ctx.Places.Attach(oPlace);
                        ctx.Places.Remove(oPlace);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar esta sede porque existe información asociada a esta.");
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
