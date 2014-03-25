using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orkidea.SchoolPanel.Entities;
using Orkidea.SchoolPanel.DataAccessEF;
using Orkidea.SchoolPanel.Utilities;

namespace Orkidea.SchoolPanel.Business
{
    public class CourseBiz
    {
        /*CRUD Course*/

        /// <summary>
        /// Retrieve course list by school
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<Course> GetCourseList(School schoolTarget)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse = ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }        

        /// <summary>
        /// Retrieve course information based in the primary key
        /// </summary>
        /// <param name="CourseTarget"></param>
        /// <returns></returns>
        public Course GetCoursebyKey(Course CourseTarget)
        {
            Course oCourse = new Course();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oCourse =
                        ctx.Courses.Where(x => x.id.Equals(CourseTarget.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oCourse;
        }

        /// <summary>
        /// Create or update a course
        /// </summary>
        /// <param name="Course"></param>
        public void SaveCourse(Course Course)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the student exists
                    Course oCourse = GetCoursebyKey(Course);

                    if (oCourse != null)
                    {
                        // if exists then edit 
                        ctx.Courses.Attach(oCourse);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oCourse, Course);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.Courses.Add(Course);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <param name="CourseTarget"></param>
        public void DeleteCourse(Course CourseTarget)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    Course oCourse = GetCoursebyKey(CourseTarget);

                    if (oCourse != null)
                    {
                        // if exists then edit 
                        ctx.Courses.Attach(oCourse);
                        ctx.Courses.Remove(oCourse);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }


        /*Complementary business methods*/

        public List<Course> GetCourseList(Grade gradeTarget)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idGrado.Equals(gradeTarget.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }

        public List<Course> GetCourseList(School schoolTarget, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) &&
                            x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }

        public List<Course> GetCourseList(School schoolTarget, Place place, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idSede.Equals(place.id) && x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }

        public List<Course> GetCourseList(School schoolTarget, Grade grade, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idGrado.Equals(grade.id) && x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;

        }

        public List<Course> GetCourseList(School schoolTarget, Place place, Grade grade, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idSede.Equals(place.id) && x.idGrado.Equals(grade.id) && x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;

        }

        public List<Course> GetCourseList(School schoolTarget, Place place, Grade grade, int idJornada, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idSede.Equals(place.id) && x.idGrado.Equals(grade.id) && x.idJornada.Equals(idJornada) && x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;

        }

        public List<Course> GetCourseList(School schoolTarget, Grade grade, int idJornada, int year)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourse =
                        ctx.Courses.Where(x => x.idColegio.Equals(schoolTarget.id) && x.idGrado.Equals(grade.id) && x.idJornada.Equals(idJornada) && x.ano.Equals(year)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;

        }       

        public List<Course> GetCourseList(Person profesor)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    CourseAsignatureBiz courseAsignatureBiz = new CourseAsignatureBiz();

                    List<int> lsCursos = courseAsignatureBiz.GetCourseAsignatureList(profesor).Select(x => x.idCurso).Distinct().ToList(); ;

                    lstCourse = ctx.Courses.Where(x => lsCursos.Contains(x.id) && x.finalizado == false).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }

        public List<Course> GetCourseList(int idEstudiante)
        {

            List<Course> lstCourse = new List<Course>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    PersonBiz personBiz = new PersonBiz();
                    Person person= personBiz.GetPersonByKey(new Person(){id = idEstudiante});

                    CourseBiz courseBiz = new CourseBiz();
                    List<int> lsCourse = courseBiz.GetCourseList(new School() { id = person.idColegio })
                        //.Where(x => !x.finalizado)
                        .Select(x => x.id).Distinct().ToList();

                    CourseStudentBiz courseStudentBiz = new CourseStudentBiz();

                    List<int> lsCursos = courseStudentBiz.GetCourseStudentList(person).Where(x => lsCourse.Contains(x.idCurso)).Select(x => x.idCurso).Distinct().ToList();

                    lstCourse = ctx.Courses.Where(x => lsCursos.Contains(x.id)).OrderByDescending(x => x.id).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourse;
        }        

    }
}
