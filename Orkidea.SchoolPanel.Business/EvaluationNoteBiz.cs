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
    public class EvaluationNoteBiz
    {
        /*CRUD Entity*/

        /// <summary>
        /// Retrieve list without parameters
        /// </summary>
        /// <returns></returns>
        public List<EvaluationNote> GetEvaluationNoteList(School schoolTarget)
        {

            List<EvaluationNote> lstEvaluationNote = new List<EvaluationNote>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstEvaluationNote = ctx.EvaluationNotes.Where(x => x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstEvaluationNote;
        }

        /// <summary>
        /// Retrieve information based in the primary key
        /// </summary>
        /// <param name="evaluationNoteTarget"></param>
        /// <returns></returns>
        public EvaluationNote GetEvaluationNoteByKey(EvaluationNote evaluationNoteTarget)
        {
            EvaluationNote oEvaluationNote = new EvaluationNote();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oEvaluationNote = ctx.EvaluationNotes.Where(x => x.id.Equals(evaluationNoteTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oEvaluationNote;
        }

        /// <summary>
        /// Create or update a new record
        /// </summary>
        /// <param name="evaluationNoteTarget"></param>
        public void SaveEvaluationNote(EvaluationNote evaluationNoteTarget)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    EvaluationNote oEvaluationNote = GetEvaluationNoteByKey(evaluationNoteTarget);

                    if (oEvaluationNote != null)
                    {
                        // if exists then edit 
                        ctx.EvaluationNotes.Attach(oEvaluationNote);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oEvaluationNote, evaluationNoteTarget);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.EvaluationNotes.Add(evaluationNoteTarget);
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
        /// <param name="evaluationNoteTarget"></param>
        public void DeleteEvaluationNote(EvaluationNote evaluationNoteTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the record exists
                    EvaluationNote oEvaluationNote = GetEvaluationNoteByKey(evaluationNoteTarget);

                    if (oEvaluationNote != null)
                    {
                        // if exists then edit 
                        ctx.EvaluationNotes.Attach(oEvaluationNote);
                        ctx.EvaluationNotes.Remove(oEvaluationNote);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException.Message.Contains("REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar esta observación del boletin porque existe información asociada a esta.");
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
