using s19551Assingment4.DTOs;
using System;
using System.Data.SqlClient;
using s19551Assingment4.Statics;
using s19551Assingment4.DTOs.Responce;

namespace s19551Assingment4.Services
{
    public class SqlServerStudentDbService : IStudentServiceDb
    {
        public string EnrollStudent(EnrollStudentRequest request)
        {
            using (SqlConnection connection = new SqlConnection(Static.CONNECTION_STRING))
            {
                string idStudy;
                string enrollmentId;
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "select IdStudy from studies where name=@name";
                    command.Parameters.AddWithValue("name", request.Study);
                    command.Connection = connection;

                    connection.Open();

                    //Checking study name exists 
                    var query = command.ExecuteReader();

                    if (!query.Read())
                    {
                        return null;
                    }

                    idStudy = query["IdStudy"].ToString();
                }

                using (SqlCommand command = new SqlCommand())
                {
                    //checking data
                    command.CommandText = @"select IdEnrollment 
                                            from Enrollment where semester=1 
                                            and idStudy=@id";
                    command.Parameters.AddWithValue("id", idStudy);

                    var query = command.ExecuteReader();

                    if (query.Read())
                    {
                        enrollmentId = query["IdEnrollment"].ToString();
                    }
                    else
                    {
                        //adding enrollment
                        command.CommandText = "insert into enrollment values(@id,@semester,@IsStudy,@startDate)";
                        command.Parameters.AddWithValue("Id", 4);
                        command.Parameters.AddWithValue("semester", 1);
                        command.Parameters.AddWithValue("idStudy", idStudy);
                        command.Parameters.AddWithValue("startdate", DateTime.Now);
                        command.ExecuteNonQuery();
                        enrollmentId = 4.ToString();
                    }

                }
                using (SqlCommand command = new SqlCommand())
                {
                    //Adding student
                    command.CommandText = @"select 1 
                                            from student 
                                            where indexnumber=@index";
                    command.Parameters.AddWithValue("index", request.IndexNumber);

                    var index = command.ExecuteReader();
                    if (index.Read())
                    {
                        return null;
                    }

                    command.CommandText = @"insert into Student 
                                            values(@indexnumber,@firstname,@lastname,@birthdate,@idenrollment)";

                    command.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                    command.Parameters.AddWithValue("firstname", request.Name);
                    command.Parameters.AddWithValue("lastname", request.Surname);
                    command.Parameters.AddWithValue("birthdate", request.Birthdate);
                    command.Parameters.AddWithValue("idenrollment", enrollmentId);

                    command.ExecuteNonQuery();

                }
            }

            return "Done!!";
        }
        


        public EnrollStudentResponse PromoteStudent(string study, int semester)
        {
            using (SqlConnection connection = new SqlConnection(Static.CONNECTION_STRING))
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "PromoteStudent";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Connection = connection;
                command.Parameters.AddWithValue("@studyName", study);
                command.Parameters.AddWithValue("@semester", semester);

                connection.Open();
                try
                {
                    var reader=command.ExecuteReader();
                    if (reader.Read())
                    {
                        int id = int.Parse(reader["IdEnrollment"].ToString());
                        var sem = reader["semester"].ToString();
                        var idStudy = int.Parse(reader["IdStudy"].ToString());
                        var startDate = DateTime.Parse(reader["StartDate"].ToString());

                        return new EnrollStudentResponse {
                            IdEnrollment = id,
                            IdStudy=idStudy,
                            Semester=sem,
                            StartDate=startDate};
                    }
                    return null;
                    
                    
                }
                catch (SqlException e)
                {
                    return null;
                }
                
            }
        }
    }
}
