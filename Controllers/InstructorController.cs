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
    public class InstructorController : ControllerBase
    {
        private IConfiguration _config;

        public InstructorController(IConfiguration config)
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

        // GET: api/Instructor
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Fname, Lname, SlackHandle, CohortId, Specialty
                                            FROM Instructor";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor newInstructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Fname = reader.GetString(reader.GetOrdinal("Fname")),
                            Lname = reader.GetString(reader.GetOrdinal("Lname")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                        };
                        instructors.Add(newInstructor);
                    }
                    reader.Close();
                    return Ok(instructors);
                };
            }
        }

        // GET: api/Instructor/5
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


        // POST: api/Instructor
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
