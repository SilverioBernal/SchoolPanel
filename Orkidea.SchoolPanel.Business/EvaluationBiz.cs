using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class EvaluationBiz
    {
        public List<Evaluation> GetEvaluationList() 
        {
            List<Evaluation> lsEvaluation = new List<Evaluation>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsEvaluation = ctx.Evaluations.ToList(); ;
                }

                return lsEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public Evaluation GetEvaluationByKey(Evaluation evaluationTarget)
        {
            Evaluation evaluation = new Evaluation();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    evaluation = ctx.Evaluations.Where(x =>                         
                        x.idEstudiante.Equals(evaluationTarget.idEstudiante) &&                        
                        x.idAsignatura.Equals(evaluationTarget.idAsignatura) &&
                        x.idPeriodoAcademico.Equals(evaluationTarget.idPeriodoAcademico)).FirstOrDefault();
                }

                return evaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public void SaveEvaluation(Evaluation evaluation)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the student exists
                    Evaluation oEvaluation = GetEvaluationByKey(evaluation);

                    if (oEvaluation != null)
                    {
                        // if exists then edit 
                        ctx.Evaluations.Attach(oEvaluation);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oEvaluation, evaluation);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Evaluations.Add(evaluation);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }


        public List<Evaluation> GetEvaluationList(CourseAsignature courseAsignature)
        {
            List<Evaluation> lsEvaluation = new List<Evaluation>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsEvaluation = ctx.Evaluations.Where(x => x.idAsignatura.Equals(courseAsignature.id)).ToList(); 
                }

                return lsEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        //public List<Evaluation> GetEvaluationbyCourseEstudent(CourseStudent courseStudentTarget)
        //{
        //    List<Evaluation> lstEvaluation = new List<Evaluation>();

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            ctx.Configuration.ProxyCreationEnabled = false;
        //            lstEvaluation = (from x in ctx.Evaluations
        //                             where
        //                             (x.idColegio == courseStudentTarget.idColegio &&
        //                             x.idCurso == courseStudentTarget.idCurso &&
        //                             x.idEstudiante == courseStudentTarget.idEstudiante)
        //                             select x).ToList();
        //        }

        //    }
        //    catch (Exception ex) { throw ex; }

        //    return lstEvaluation;
        //}

        //public List<Evaluation> GetEvaluationResult(Evaluation evaluation)
        //{
        //    List<Evaluation> lstEvaluation = new List<Evaluation>();

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            ctx.Configuration.ProxyCreationEnabled = false;
        //            lstEvaluation = (from x in ctx.Evaluations
        //                             where
        //                             (x.idColegio ==  evaluation.idColegio &&
        //                             x.idCurso == evaluation.idCurso &&
        //                             x.idEstudiante == evaluation.idEstudiante &&
        //                             x.idPeriodoAcademico == evaluation.idPeriodoAcademico)
        //                             select x).ToList();
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }

        //    return lstEvaluation;
        //}

        //public List<Evaluation> GetEvaluationbyCourseAssignature(Evaluation evaluationTarget)
        //{
        //    List<Evaluation> lstEvaluation = new List<Evaluation>();

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            ctx.Configuration.ProxyCreationEnabled = false;
        //            lstEvaluation = (from x in ctx.Evaluations
        //                             where
        //                             (x.idColegio == evaluationTarget.idColegio &&
        //                             x.idCurso == evaluationTarget.idCurso &&
        //                             x.idAsignatura == evaluationTarget.idAsignatura &&
        //                             x.idPeriodoAcademico == evaluationTarget.idPeriodoAcademico)
        //                             select x).ToList();
        //        }

        //    }
        //    catch (Exception ex) { throw ex; }

        //    return lstEvaluation;
        //}

    }
}
