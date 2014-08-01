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

        public List<Evaluation> GetEvaluationList(Course course)
        {
            List<Evaluation> lsEvaluation = new List<Evaluation>();

            try
            {
                CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();

                List<CourseAsignature> lsCA = courseAsignatureBiz.GetCourseAsignatureList(course);

                foreach (CourseAsignature item in lsCA)
                {
                    lsEvaluation.AddRange(GetEvaluationList(item));
                }

                return lsEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<Evaluation> GetEvaluationList(CourseAsignature courseAsignature, AcademicPeriod academicPeriod)
        {
            List<Evaluation> lsEvaluation = new List<Evaluation>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsEvaluation = ctx.Evaluations.Where(x => x.idAsignatura.Equals(courseAsignature.id) && x.AcademicPeriod.id.Equals(academicPeriod.id)).ToList();
                }

                return lsEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        /*Dicipline evaluation*/

        public DiciplineEvaluation GetDiciplineEvaluationByKey(DiciplineEvaluation evaluationTarget)
        {
            DiciplineEvaluation evaluation = new DiciplineEvaluation();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    evaluation = ctx.DiciplineEvaluations.Where(x =>
                        x.idEstudiante.Equals(evaluationTarget.idEstudiante) &&
                        x.idAsignatura.Equals(evaluationTarget.idAsignatura) &&
                        x.idPeriodoAcademico.Equals(evaluationTarget.idPeriodoAcademico)).FirstOrDefault();
                }

                return evaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<DiciplineEvaluation> GetDiciplineEvaluationList(Course course)
        {
            List<DiciplineEvaluation> lsDiciplineEvaluation = new List<DiciplineEvaluation>();

            try
            {
                CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();

                List<CourseAsignature> lsCA = courseAsignatureBiz.GetCourseAsignatureList(course);

                foreach (CourseAsignature item in lsCA)
                {
                    lsDiciplineEvaluation.AddRange(GetDiciplineEvaluationList(item));
                }

                return lsDiciplineEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<DiciplineEvaluation> GetDiciplineEvaluationList(CourseAsignature courseAsignature)
        {
            List<DiciplineEvaluation> lsDiciplineEvaluation = new List<DiciplineEvaluation>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lsDiciplineEvaluation = ctx.DiciplineEvaluations.Where(x => x.idAsignatura.Equals(courseAsignature.id)).ToList();
                }

                return lsDiciplineEvaluation;
            }
            catch (Exception ex) { throw ex; }
        }

        public void SaveDiciplineEvaluation(DiciplineEvaluation evaluation)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the student exists
                    DiciplineEvaluation oEvaluation = GetDiciplineEvaluationByKey(evaluation);

                    if (oEvaluation != null)
                    {
                        // if exists then edit 
                        ctx.DiciplineEvaluations.Attach(oEvaluation);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oEvaluation, evaluation);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.DiciplineEvaluations.Add(evaluation);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

    }
}
