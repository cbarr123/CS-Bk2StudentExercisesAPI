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
    public class ExercisesController : ControllerBase
    {
        private IConfiguration _config;

        public ExercisesController(IConfiguration config)
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

        

        // GET: api/Exercises
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Ename, Elanguage
                                            FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise newExercise = new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Ename = reader.GetString(reader.GetOrdinal("Ename")),
                            Elanguage = reader.GetString(reader.GetOrdinal("Elanguage"))
                        };
                        exercises.Add(newExercise);
                    }
                    reader.Close();
                    return Ok(exercises);
                };
            }    
        }
        // GET: api/Exercises/5
        [HttpGet("{id}", Name = "GetExercise")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Ename, Elanguage
                            FROM Exercise
                            WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Ename = reader.GetString(reader.GetOrdinal("Ename")),
                            Elanguage = reader.GetString(reader.GetOrdinal("Elanguage"))
                        };
                    }
                    reader.Close();
                    return Ok(exercise);
                }
            }
        }

        // POST: api/Exercises
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (eName, eLanguage)
                                            OUTPUT INSERTED.Id
                                            VALUES (@Ename, @Elanguage)";
                    cmd.Parameters.Add(new SqlParameter("@Ename", exercise.Ename));
                    cmd.Parameters.Add(new SqlParameter("@Elanguage", exercise.Elanguage));

                    int newId = (int)cmd.ExecuteScalar();
                    exercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, exercise);
                }
            }
        }

        // PUT: api/Exercises
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Exercise exercise)
        {
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                                SET Ename = @Ename,
                                                    Elanguage = @Elanguage
                                                    WHERE Id = @id";
                    }
                }
            }
        }


        // DELETE: api/ApiWithActions
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
