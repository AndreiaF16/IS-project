using IPLeiriaSmartCampusAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace IPLeiriaSmartCampusAPI.Controllers
{
    public class SensoresController : ApiController
    {
        private string connectionString = Properties.Settings.Default.ConnectionString;

        [Route("api/sensores/")]
        [HttpGet]
        public IEnumerable<Sensor> GetAllSensors()
        {
            List<Sensor> sensores = null;
            try
            {
                sensores = new List<Sensor>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Sensores", conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
			                Alerta alerta = null;
			                if((bool)reader["AlertaAtivo"]) 
                            {
                                alerta = new Alerta
                                {
                                    Ativo = true,
                                    Descricao = (string)reader["DescricaoAlerta"]
                                };
			                }

                            Sensor sensor = new Sensor()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Bateria = (int)reader["Bateria"],
                                Data = (DateTime)reader["Data"]
                            };

			                if(alerta != null) 
			                {
				                sensor.Alerta = alerta;
			                }
                            
                            sensores.Add(sensor);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return null;
        }

        [Route("api/sensores/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetSensorById(int id)
        {
            Sensor sensor = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Sensores WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Alerta alerta = null;
                        if ((bool)reader["AlertaAtivo"])
                        {
                            alerta = new Alerta
                            {
                                Ativo = true,
                                Descricao = (string)reader["DescricaoAlerta"]
                            };
                        }

                        while (reader.Read())
                        {
                            sensor = new Sensor
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Bateria = (int)reader["Bateria"],
                                Data = (DateTime)reader["Data"]
                            };
                        }

                        if(alerta != null)
                        {
                            sensor.Alerta = alerta;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }

            if (sensor != null)
            {
                return Ok(sensor);
            }

            return NotFound();
        }

    }
}