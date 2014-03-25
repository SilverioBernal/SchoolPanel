using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class Obsolete_StudentBitacoreBiz
    {        /*CRUD StudentBitacores*/

        ///// <summary>
        ///// Retrieve studentBitacore list by school
        ///// </summary>
        ///// <param name="studentTarget"></param>
        ///// <returns></returns>
        //public List<StudentBitacore> GetStudentBitacoreList(Student studentTarget)
        //{

        //    List<StudentBitacore> lstStudentBitacore = new List<StudentBitacore>();

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            ctx.Configuration.ProxyCreationEnabled = false;
        //            lstStudentBitacore = (from x in ctx.StudentBitacores
        //                                  where (x.idColegio == studentTarget.idColegio &&
        //                                  x.idEstudiante == studentTarget.id)
        //                                  select x).ToList();
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }

        //    return lstStudentBitacore;
        //}

        ///// <summary>
        ///// Retrieve studentBitacore information based in the primary key
        ///// </summary>
        ///// <param name="studentBitacoreTarget"></param>
        ///// <returns></returns>
        //public StudentBitacore GetStudentBitacorebyKey(StudentBitacore studentBitacoreTarget)
        //{
        //    StudentBitacore oStudentBitacore = new StudentBitacore();

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            ctx.Configuration.ProxyCreationEnabled = false;

        //            oStudentBitacore = ctx.StudentBitacores.Where(x => x.idEstudiante.Equals(studentBitacoreTarget.idEstudiante) &&
        //                x.idColegio.Equals(studentBitacoreTarget.idColegio) && x.fecha.Equals(studentBitacoreTarget.fecha)).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex) { throw ex; }

        //    return oStudentBitacore;
        //}        

        ///// <summary>
        ///// Create or update a studentBitacore
        ///// </summary>
        ///// <param name="studentBitacoreTarget"></param>
        //public void SaveStudentBitacore(StudentBitacore studentBitacoreTarget)
        //{

        //    try
        //    {
        //        using (var ctx = new SchoolPanelEntities())
        //        {
        //            //verify if the studentBitacore exists
        //            StudentBitacore oStudentBitacore = GetStudentBitacorebyKey(studentBitacoreTarget);

        //            if (oStudentBitacore != null)
        //            {
        //                // if exists then edit 
        //                ctx.StudentBitacores.Attach(oStudentBitacore);
        //                _GenericEntityValidation.EnumeratePropertyDifferences(oStudentBitacore, studentBitacoreTarget);
        //                ctx.SaveChanges();
        //            }
        //            else
        //            {
        //                // else create
        //                ctx.StudentBitacores.Add(studentBitacoreTarget);
        //                ctx.SaveChanges();
        //            }
        //        }

        //    }
        //    catch (Exception ex) { throw ex; }
        //}

        ///// <summary>
        ///// Delete a grade
        ///// </summary>
        ///// <param name="studentBitacoreTarget"></param>
        ////public void DeleteStudentBitacore(StudentBitacore studentBitacoreTarget)
        ////{
        ////    try
        ////    {
        ////        using (var ctx = new SchoolPanelEntities())
        ////        {
        ////            //verify if the school exists
        ////            StudentBitacore oStudentBitacore = GetStudentBitacorebyKey(studentBitacoreTarget);

        ////            if (oStudentBitacore != null)
        ////            {
        ////                // if exists then edit 
        ////                ctx.StudentBitacores.Attach(oStudentBitacore);
        ////                ctx.StudentBitacores.Remove(oStudentBitacore);
        ////                ctx.SaveChanges();
        ////            }
        ////        }
        ////    }
        ////    catch (Exception ex) { throw ex; }
        ////}
    }
}

