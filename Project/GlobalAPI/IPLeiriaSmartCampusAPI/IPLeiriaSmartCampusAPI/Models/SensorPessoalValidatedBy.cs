using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace IPLeiriaSmartCampusAPI.Models
{
    public class SensorPessoalValidatedBy
    {
        private static string CONNECTION_STRING = Properties.Settings.Default.ConnectionString;
        public string ValidatedBy { get; set; }
        public DateTime DateValidatedBy { get; set; }

        public static SensorPessoal UpdateValido(SensorPessoal sensor, int id, int current_user)
        {
            try
            {
                DateTime data;
                if(sensor.Data == default(DateTime))
                {
                    data = DateTime.Now;
                } 
                else
                {
                    data = sensor.Data;
                }

                using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE SensoresPessoais SET ValidatedBy = @current_user, DateValidatedBy = @data WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@current_user", current_user);
                    cmd.Parameters.AddWithValue("@data", data);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == -1)
                    {
                        return null;
                    }

                    conn.Close();

                    return SensorPessoal.GetSensorById(id);
                }
            }
            catch (Exception execption)
            {
                return null;
            }
        }
    }
}