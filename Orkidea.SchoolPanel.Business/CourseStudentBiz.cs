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
    public class CourseStudentBiz
    {
        /*CRUD Course student*/

        /// <summary>
        /// Retrieve Course teacher list by school
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <returns></returns>
        public List<CourseStudent> GetCourseStudentList()
        {

            List<CourseStudent> lstCourseStudent = new List<CourseStudent>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourseStudent = ctx.CourseStudents.ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourseStudent;
        }

        /// <summary>
        /// Retrieve CourseStudent information based in the primary key
        /// </summary>
        /// <param name="courseStudent"></param>
        /// <returns></returns>
        public CourseStudent GetCourseStudentbyKey(CourseStudent courseStudent)
        {
            CourseStudent oCourseStudent = new CourseStudent();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    oCourseStudent =
                        ctx.CourseStudents.Where(x => x.id.Equals(courseStudent.id)).FirstOrDefault();
                }
            }
            catch (Exception ex) { throw ex; }

            return oCourseStudent;
        }

        /// <summary>
        /// Create or update a CourseStudent
        /// </summary>
        /// <param name="courseStudent"></param>
        public void SaveCourseStudent(CourseStudent courseStudent)
        {

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the student exists
                    CourseStudent oCourseStudent = GetCourseStudentbyKey(courseStudent);

                    if (oCourseStudent != null)
                    {
                        // if exists then edit 
                        ctx.CourseStudents.Attach(oCourseStudent);
                        _GenericEntityValidation.EnumeratePropertyDifferences(oCourseStudent, courseStudent);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        // else create
                        ctx.CourseStudents.Add(courseStudent);
                        ctx.SaveChanges();
                    }
                }

            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Delete a CourseStudent
        /// </summary>
        /// <param name="courseStudent"></param>
        public void DeleteCourseStudent(CourseStudent courseStudent)
        {
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    //verify if the school exists
                    CourseStudent oCourseStudent = GetCourseStudentbyKey(courseStudent);

                    if (oCourseStudent != null)
                    {
                        // if exists then edit 
                        ctx.CourseStudents.Attach(oCourseStudent);
                        ctx.CourseStudents.Remove(oCourseStudent);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /*Complementary business methods*/

        /// <summary>
        /// Retrieve Course teacher list by school and course
        /// </summary>
        /// <param name="schoolTarget"></param>
        /// <param name="gradeTarget"></param>
        /// <returns></returns>
        public List<CourseStudent> GetCourseStudentList(Course course)
        {
            List<CourseStudent> lsCourseStudent = new List<CourseStudent>();
            List<Course> lsCourse = new List<Course>();
            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;

                    CourseBiz courseBiz = new CourseBiz();

                    if (course.id > 0)
                        lsCourseStudent = ctx.CourseStudents.Where(x => x.idCurso.Equals(course.id)).ToList();
                    else
                    {
                        if (course.idColegio > 0 && course.idSede > 0 && course.idJornada > 0 && course.idGrado > 0 && course.ano > 0)
                            lsCourse = courseBiz.GetCourseList(new School() { id = course.idColegio }, new Place() { id = course.idSede }, new Grade() { id = course.idGrado }, course.idJornada, course.ano);
                        else
                        {
                            if (course.idColegio > 0 && course.idSede > 0 && course.idJornada == 0 && course.idGrado > 0 && course.ano > 0)
                                lsCourse = courseBiz.GetCourseList(new School() { id = course.idColegio }, new Place() { id = course.idSede }, new Grade() { id = course.idGrado }, course.ano);
                            else
                            {
                                if (course.idColegio > 0 && course.idSede == 0 && course.idJornada == 0 && course.idGrado > 0 && course.ano > 0)
                                    lsCourse = courseBiz.GetCourseList(new School() { id = course.idColegio }, new Grade() { id = course.idGrado }, course.ano);

                                if (course.idColegio > 0 && course.idSede == 0 && course.idJornada > 0 && course.idGrado > 0 && course.ano > 0)
                                    lsCourse = courseBiz.GetCourseList(new School() { id = course.idColegio }, new Grade() { id = course.idGrado }, course.idJornada, course.ano);

                                if (course.idColegio > 0 && course.idSede == 0 && course.idJornada == 0 && course.idGrado == 0 && course.ano > 0)
                                    lsCourse = courseBiz.GetCourseList(new School() { id = course.idColegio }, course.ano);
                            }
                        }

                        List<int> lsCursos = lsCourse.Select(x => x.id).ToList();
                        lsCourseStudent = ctx.CourseStudents.Where(x => lsCursos.Contains(x.idCurso)).ToList();
                    }                    
                }
            }
            catch (Exception ex) { throw ex; }

            return lsCourseStudent;
        }

        public List<CourseStudent> GetCourseStudentList(Person estudiante)
        {
            List<CourseStudent> lstCourseStudent = new List<CourseStudent>();

            try
            {
                using (var ctx = new SchoolPanelEntities())
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    lstCourseStudent = ctx.CourseStudents.Where(x=> x.idEstudiante.Equals(estudiante.id)).ToList();
                }
            }
            catch (Exception ex) { throw ex; }

            return lstCourseStudent;
        }
    }
}
