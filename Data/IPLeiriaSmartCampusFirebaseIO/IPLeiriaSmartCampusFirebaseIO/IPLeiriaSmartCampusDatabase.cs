using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Database.Query;

namespace IPLeiriaSmartCampusFireBaseIO
{
    public class IPLeiriaSmartCampusDatabase
    {
        private static String BASE_PATH = "https://ipsmartcampus.firebaseio.com/";
        private static String TABLE_SENSORS = "sensors";

        private FirebaseClient client = null;

        public IPLeiriaSmartCampusDatabase()
        {
            client = new FirebaseClient(BASE_PATH);
        }

        public bool IsConnected()
        {
            if (client != null)
            {
                return true;
            }

            return false;
        }

        private string TemporaryKey()
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(DateTime.Now.ToString());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public async Task<List<Sensor>> GetSensors()
        {
            try
            {
                List<Sensor> sensors = new List<Sensor>();

                var list = await client.Child(TABLE_SENSORS).OnceAsync<Sensor>();
                foreach (var item in list)
                {
                    sensors.Add(item.Object);
                }

                return sensors;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<Sensor> GetSensorById(string id)
        {
            try
            {
                //Get all sensors
                List<Sensor> sensors = await GetSensors();

                //Find sensor by id
                foreach (var sensor in sensors)
                {
                    if(sensor.Key == id)
                    {
                        return sensor;
                    }
                }                
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        
        
        public async Task<bool> CreateSensor(Sensor sensor)
        {
            try
            {
                //Create object
                sensor.Key = TemporaryKey();
                FirebaseObject<Sensor> postObject = await client.Child(TABLE_SENSORS).PostAsync(sensor);
                if (postObject.Object.Key != sensor.Key)
                {
                    return false;
                }

                //Update key from created object
                sensor.Key = postObject.Key;
                if (await UpdateSensor(sensor))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public async Task<bool> UpdateSensor(Sensor sensor)
        {
            try
            {
                //Update object
                await client.Child(TABLE_SENSORS).Child(sensor.Key.ToString()).PutAsync(sensor);

                //Verified updated object
                Sensor s = await GetSensorById(sensor.Key);
                if (s != null && s.SensorID == sensor.SensorID && s.Temperature == sensor.Temperature && s.Humidity == sensor.Humidity
                             && s.Battery == sensor.Battery && s.Timestamp == sensor.Timestamp && s.Battery == sensor.Battery)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public async Task<bool> DeleteSensor(Sensor sensor)
        {
            try
            {
                //Delete object
                await client.Child(TABLE_SENSORS).Child(sensor.Key.ToString()).DeleteAsync();

                //Verified if object exists
                Sensor s = await GetSensorById(sensor.Key);
                if (s == null)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

    }
}
