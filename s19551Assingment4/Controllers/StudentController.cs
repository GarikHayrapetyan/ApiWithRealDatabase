using Microsoft.AspNetCore.Mvc;
using s19551Assingment4.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace s19551Assingment4.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController:ControllerBase
    {
        private string connectionString =
             @"Data Source=DESKTOP-6POC7D5\SQLEXPRESS;Initial Catalog=university;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
     
        public StudentController()
        {
           
        }


        [HttpGet]
        public IActionResult GetStudents()
        {
            var result = new List<Student>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select s.FirstName, s.LastName, s.BirthDate, st.Name as Studies, e.Semester " +//
                                      "from student s "+//
                                      "join Enrollment e on s.idEnrollment=e.idEnrollment " +//
                                      "join Studies st on st.idStudy=e.idStudy;";
                                                

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var student = new Student();
                    student.Name = reader["FirstName"].ToString();
                    student.Surname = reader["LastName"].ToString();
                    student.Birthdate = DateTime.Parse(reader["BirthDate"].ToString());
                    student.Study = reader["Studies"].ToString();
                    student.Semester = int.Parse(reader["Semester"].ToString());
                    result.Add(student);
                }
            }

            return Ok(result);
        }


        [HttpGet("{indexNumber}")]
        public IActionResult GetSemester(string indexNumber)
        {
            string semester = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "select e.semester from student as s " +//
                                      "join enrollment as e on s.idenrollment = e.idenrollment " +//
                                      "where s.indexnumber=@index";

                command.Parameters.AddWithValue("index", indexNumber);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();


                if (reader.Read())
                {
                    semester = reader["Semester"].ToString();
                }
                else
                {
                    semester = "Index number does not exist!";
                }
            }

            return Ok(indexNumber + ":" + semester);
        }

    }
}
