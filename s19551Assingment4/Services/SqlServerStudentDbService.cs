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
                int enrollmentId;
                SqlTransaction transaction = connection.BeginTransaction();
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
                        transaction.Rollback();
                        return null;
                    }

                    idStudy = query["IdStudy"].ToString();
                    query.Close();
                }

                using (SqlCommand command = new SqlCommand())
                {
                    //checking data
                    command.CommandText = @"select IdEnrollment 
                                            from Enrollment where semester=1 
                                            and idStudy=@id";
                    command.Parameters.AddWithValue("id", idStudy);
                    command.Connection = connection;

                    var IdEnroll = command.ExecuteReader();

                    if (IdEnroll.Read())
                    {
                        enrollmentId = int.Parse(IdEnroll["IdEnrollment"].ToString());
                        IdEnroll.Close();
                    }
                    else
                    {
                        IdEnroll.Close();
                        using (SqlCommand enrollCommand= new SqlCommand())
                        {
                            enrollCommand.CommandText = @"select max(IdEnrollment) as maxId
                                                          from Enrollment";
                            enrollCommand.Connection = connection;
                            var max = enrollCommand.ExecuteReader();
                            if (max.Read())
                            {
                                enrollmentId = int.Parse(max["maxId"].ToString());
                            }
                            else
                            {
                                enrollmentId = 1;
                            }
                            max.Close();
                           
                        }
                        //adding new enrollment
                        using (SqlCommand sqlCommand = new SqlCommand())
                        {                        
                            sqlCommand.Transaction = transaction;

                            try
                            {
                                sqlCommand.CommandText = @"insert into enrollment
                                                       values(@id,@semester,@IsStudy,@startDate)";
                                sqlCommand.Parameters.AddWithValue("Id", enrollmentId);
                                sqlCommand.Parameters.AddWithValue("semester", 1);
                                sqlCommand.Parameters.AddWithValue("idStudy", idStudy);
                                sqlCommand.Parameters.AddWithValue("startdate", DateTime.Now);
                                sqlCommand.Connection = connection;
                                sqlCommand.ExecuteNonQuery();
                                transaction.Commit();
                            }
                            catch (SqlException ex)
                            {
                                transaction.Rollback();
                            }
                            
                        }
                    }

                }

                using (SqlCommand command = new SqlCommand())
                {
                    //Adding student
                    command.CommandText = @"select 1 
                                            from student 
                                            where indexnumber=@index";
                    command.Parameters.AddWithValue("index", request.IndexNumber);
                    command.Connection = connection;
                    var index = command.ExecuteReader();
                    if (index.Read())
                    {
                        transaction.Rollback();
                        return null;
                    }

                    index.Close();                    
                }

                using (SqlCommand commandInsert = new SqlCommand())
                {
                    commandInsert.CommandText = @"insert into Student 
                                            values(@indexnumber,@firstname,@lastname,@birthdate,@idenrollment)";
                    commandInsert.Connection = connection;
                    commandInsert.Parameters.AddWithValue("indexnumber", request.IndexNumber);
                    commandInsert.Parameters.AddWithValue("firstname", request.Name);
                    commandInsert.Parameters.AddWithValue("lastname", request.Surname);
                    commandInsert.Parameters.AddWithValue("birthdate", request.Birthdate);
                    commandInsert.Parameters.AddWithValue("idenrollment", enrollmentId);

                    try
                    {
                        commandInsert.ExecuteNonQuery();
                    }catch(SqlException ex)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return "Done!!";
        }



        public EnrollStudentResponse PromoteStudent(string study, int semester)
        {
            using (SqlConnection connection = new SqlConnection(Static.CONNECTION_STRING))
            {
                SqlTransaction transaction = connection.BeginTransaction();
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
                        var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            int id = int.Parse(reader["IdEnrollment"].ToString());
                            var sem = reader["semester"].ToString();
                            var idStudy = int.Parse(reader["IdStudy"].ToString());
                            var startDate = DateTime.Parse(reader["StartDate"].ToString());

                            return new EnrollStudentResponse
                            {
                                IdEnrollment = id,
                                IdStudy = idStudy,
                                Semester = sem,
                                StartDate = startDate
                            };
                        }
                        transaction.Rollback();
                        return null;


                    }
                    catch (SqlException e)
                    {
                        transaction.Rollback();
                        return null;
                    }

                }
            }
        }
    }
}
