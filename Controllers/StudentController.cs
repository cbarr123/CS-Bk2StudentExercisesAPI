using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesAPI.Models;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }






        // GET: api/Student
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Fname, Lname, SlackHandle, CohortId
                                            FROM Student";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        Student newStudent = new Student()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Fname = reader.GetString(reader.GetOrdinal("Fname")),
                            Lname = reader.GetString(reader.GetOrdinal("Lname")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };
                        students.Add(newStudent);
                    }
                    reader.Close();
                    return Ok(students);
                };
            }
        }

        // GET: api/Student/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Fname, Lname, SlackHandle, CohortId
                            FROM Student
                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Fname = reader.GetString(reader.GetOrdinal("Fname")),
                            Lname = reader.GetString(reader.GetOrdinal("Lname")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };
                    }
                    reader.Close();
                    return Ok(student);
                }
            }
        }

        // POST: api/Student
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (Fname, Lname, SlackHandle, CohortId)
                                            OUTPUT INSERTED.Id
                                            VALUES (@Fname, @Lname, @SlackHandle, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@Fname", student.Fname));
                    cmd.Parameters.Add(new SqlParameter("@Lname", student.Lname));
                    cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));

                    int newId = (int)cmd.ExecuteScalar();
                    student.Id = newId;
                    return CreatedAtRoute("GetStudent", new { id = newId }, student);
                }
            }
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                                SET Fname = @Fname,
                                                    Lname = @Lname,
                                                    SlackHandle = @SlackHandle,
                                                    CohortId = @CohortId
                                                WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Fname", student.Fname));
                        cmd.Parameters.Add(new SqlParameter("@Lname", student.Lname));
                        cmd.Parameters.Add(new SqlParameter("@id", student.Id));
                        cmd.Parameters.Add(new SqlParameter("@SlackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool StudentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Fname, Lname, SlackHandle, CohortId
                        FROM Student
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
            //try
            //{

            //}
        //}



    }
}
