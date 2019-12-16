using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class SensorPessoal
    {
        private static string CONNECTION_STRING = Properties.Settings.Default.ConnectionString;

        public int Id { get; set; }
        public string Local { get; set; }
        public decimal Temperatura { get; set; }
        public decimal Humidade { get; set; }
        public DateTime Data { get; set; }
        public bool Valido { get; set; }
	    public string Utilizador { get; set; }
        public SensorPessoalValidatedBy Validated { get; set; }

        public static List<SensorPessoal> GetAllSensores()
        {
            List<SensorPessoal> sensores = null;
            try
            {
                sensores = new List<SensorPessoal>();
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SensoresPessoais", conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SensorPessoal sensorPessoal = new SensorPessoal()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Data = (DateTime)reader["Data"],
                                Valido = (bool)reader["Valido"],
                                Local = (string)reader["Local"],
                                Utilizador = Models.Utilizador.GetUtilizadorById((int)reader["UtilizadorID"]).Username
                            };
                            if (reader["ValidatedBy"] != DBNull.Value)
                            {
                                sensorPessoal.Validated = new SensorPessoalValidatedBy()
                                {
                                    ValidatedBy = Models.Utilizador.GetUtilizadorById((int)reader["ValidatedBy"]).Username,
                                    DateValidatedBy = (DateTime)reader["DateValidatedBy"]
                                };
                            }

                            sensores.Add(sensorPessoal);
                        }
                    }
                    conn.Close();
                }
                return sensores;
            } catch (Exception execption)
            {
                return null;
            }
        }

        public static List<SensorPessoal> GetMySensores(int id)
        {
            List<SensorPessoal> sensores = null;
            try
            {
                sensores = new List<SensorPessoal>();
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SensoresPessoais WHERE UtilizadorID = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SensorPessoal sensorPessoal = new SensorPessoal()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Data = (DateTime)reader["Data"],
                                Valido = (bool)reader["Valido"],
                                Local = (string)reader["Local"],
                                Utilizador = Models.Utilizador.GetUtilizadorById((int)reader["UtilizadorID"]).Username
                            };
                            if (reader["ValidatedBy"] != DBNull.Value)
                            {
                                sensorPessoal.Validated = new SensorPessoalValidatedBy()
                                {
                                    ValidatedBy = Models.Utilizador.GetUtilizadorById((int)reader["ValidatedBy"]).Username,
                                    DateValidatedBy = (DateTime)reader["DateValidatedBy"]
                                };
                            }
                            sensores.Add(sensorPessoal);
                        }
                    }
                    conn.Close();
                }
                return sensores;
            }
            catch (Exception execption)
            {
                return null;
            }
        }

        public static SensorPessoal CreateNewSensor(SensorPessoal sensor, int id)
        {
            try
            {
                if(sensor.Temperatura == 0 || sensor.Humidade == 0 || sensor.Local == null)
                {
                    return null;
                }

                DateTime date;
                if (sensor.Data == default(DateTime))
                {
                    date = DateTime.Now;
                }
                else
                {
                    date = sensor.Data;
                }

                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO SensoresPessoais (Temperatura, Humidade, Data, Valido, UtilizadorID, Local) " +
                                                        "VALUES (@temperatura, @humidade, @data, @valido, @userId, @local)", conn);
                    
                    cmd.Parameters.AddWithValue("@temperatura", sensor.Temperatura);
                    cmd.Parameters.AddWithValue("@humidade", sensor.Humidade);
                    cmd.Parameters.AddWithValue("@data", date);
                    cmd.Parameters.AddWithValue("@valido", 1);
                    cmd.Parameters.AddWithValue("@userId", id);
                    cmd.Parameters.AddWithValue("@local", sensor.Local);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == -1)
                    {
                        return null;
                    }

                    return GetSensorByDateAndUserId(id, date);
                }
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public static SensorPessoal GetSensorByDateAndUserId(int id, DateTime data)
        {
            SensorPessoal sensorPessoal = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SensoresPessoais WHERE UtilizadorID = @id AND Data = @data", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@data", data);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sensorPessoal = new SensorPessoal()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Data = (DateTime)reader["Data"],
                                Valido = (bool)reader["Valido"],
                                Local = (string)reader["Local"],
                                Utilizador = Models.Utilizador.GetUtilizadorById((int)reader["UtilizadorID"]).Username
                            };
                            if (reader["ValidatedBy"] != DBNull.Value)
                            {
                                sensorPessoal.Validated = new SensorPessoalValidatedBy()
                                {
                                    ValidatedBy = Models.Utilizador.GetUtilizadorById((int)reader["ValidatedBy"]).Username,
                                    DateValidatedBy = (DateTime)reader["DateValidatedBy"]
                                };
                            }
                        }
                    }
                    conn.Close();
                }
                return sensorPessoal;
            }
            catch (Exception execption)
            {
                return null;
            }
        }

        public static SensorPessoal GetSensorById(int id)
        {
            SensorPessoal sensorPessoal = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SensoresPessoais WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sensorPessoal = new SensorPessoal()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Data = (DateTime)reader["Data"],
                                Valido = (bool)reader["Valido"],
                                Local = (string)reader["Local"],
                                Utilizador = Models.Utilizador.GetUtilizadorById((int)reader["UtilizadorID"]).Username
                            };
                            if (reader["ValidatedBy"] != DBNull.Value)
                            {
                                sensorPessoal.Validated = new SensorPessoalValidatedBy()
                                {
                                    ValidatedBy = Models.Utilizador.GetUtilizadorById((int)reader["ValidatedBy"]).Username,
                                    DateValidatedBy = (DateTime)reader["DateValidatedBy"]
                                };
                            }
                        }
                    }
                    conn.Close();
                }
                return sensorPessoal;
            }
            catch (Exception execption)
            {
                return null;
            }
        }

        public static List<SensorPessoal> GetSensoresByUsername(string username)
        {
            List<SensorPessoal> sensores = null;
            try
            {
                sensores = new List<SensorPessoal>();
                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    Utilizador utilizador = Models.Utilizador.GetUtilizadorByUsername(username);

                    SqlCommand cmd = new SqlCommand("SELECT * FROM SensoresPessoais WHERE UtilizadorID = @id", conn);
                    cmd.Parameters.AddWithValue("@id", utilizador.Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SensorPessoal sensorPessoal = new SensorPessoal()
                            {
                                Id = (int)reader["Id"],
                                Temperatura = (decimal)reader["Temperatura"],
                                Humidade = (decimal)reader["Humidade"],
                                Data = (DateTime)reader["Data"],
                                Valido = (bool)reader["Valido"],
                                Local = (string)reader["Local"],
                                Utilizador = Models.Utilizador.GetUtilizadorById((int)reader["UtilizadorID"]).Username
                            };
                            if (reader["ValidatedBy"] != DBNull.Value)
                            {
                                sensorPessoal.Validated = new SensorPessoalValidatedBy()
                                {
                                    ValidatedBy = Models.Utilizador.GetUtilizadorById((int)reader["ValidatedBy"]).Username,
                                    DateValidatedBy = (DateTime)reader["DateValidatedBy"]
                                };
                            }

                            sensores.Add(sensorPessoal);
                        }
                    }
                    conn.Close();
                }
                return sensores;
            }
            catch (Exception execption)
            {
                return null;
            }
        }
    }
}
