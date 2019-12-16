using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Globalization;
using System.Data.SqlClient;

namespace AssociationWithDatabase
{
    public partial class Form1 : Form
    {
        static string connectionString = @"Data Source=f1addfa7-1f87-47fb-9183-ab1b00af3c6b.sqlserver.sequelizer.com;Persist Security Info=True;User ID=styogfmpdgdczdjc;Password=kxPxixAhDiNeex8FmduXwiqu7vhWHfJh3CkcDgSpowJNBiF5C6RkV2JbgtzVqMGy";

        List<Sensor> allSensors = null;

        MqttClient mqttClient = new MqttClient("127.0.0.1");
        string[] topics = { "source", "alerts"};
        byte[] qosLevels =
        {
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE
        };
        public Form1()
        {
            InitializeComponent();
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            allSensors = new List<Sensor>();
            mqttClient.Connect(Guid.NewGuid().ToString());
            if (!mqttClient.IsConnected)
            {
                MessageBox.Show("Error connecting to the message broker...");
                return;
            }

            mqttClient.Subscribe(topics, qosLevels);
            mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
        }

        private void MqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            this.Invoke((MethodInvoker) delegate ()
            {
                //richTextBox1.AppendText($"Recived: {Encoding.UTF8.GetString(e.Message)} on topic {e.Topic}\n");
                SqlConnection conn = null;

                try
                {
                    using (conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        if (e.Topic == "source")
                        {
                            List<Sensor> sensores = ConvertReceivedDataSource(Encoding.UTF8.GetString(e.Message));
                            foreach (var sensor in sensores)
                            {
                                allSensors.Add(sensor);
                                SqlCommand cmd = new SqlCommand("INSERT INTO SENSORES VALUES (@id,@temperatura,@humidade,@bateria,@data,@alerta,@decricao)", conn);
                                cmd.Parameters.AddWithValue("@id", sensor.SensorID);
                                cmd.Parameters.AddWithValue("@temperatura", sensor.Temperature);
                                cmd.Parameters.AddWithValue("@humidade", sensor.Humidity);
                                cmd.Parameters.AddWithValue("@bateria", sensor.Battery);
                                cmd.Parameters.AddWithValue("@data", sensor.Timestamp);
                                cmd.Parameters.AddWithValue("@alerta", sensor.ActiveAlert);
                                cmd.Parameters.AddWithValue("@decricao", sensor.AlertDescription);
                                int result = cmd.ExecuteNonQuery();
                            }
                        }
                        else if (e.Topic == "alerts")
                        {
                            Sensor sensor = Newtonsoft.Json.JsonConvert.DeserializeObject<Sensor>(Encoding.UTF8.GetString(e.Message));
                            allSensors.Add(sensor);
                            SqlCommand cmd = new SqlCommand("UPDATE SENSORES SET AlertaAtivo = @AlertaAtivo, DescricaoAlerta = @DescricaoAlerta WHERE Id = @Id AND Data = @Data", conn);
                            cmd.Parameters.AddWithValue("@AlertaAtivo", sensor.ActiveAlert);
                            cmd.Parameters.AddWithValue("@DescricaoAlerta", sensor.AlertDescription);
                            cmd.Parameters.AddWithValue("@Id", sensor.SensorID);
                            cmd.Parameters.AddWithValue("@Data", sensor.Timestamp);
                            int result = cmd.ExecuteNonQuery();
                            
                        }
                        conn.Close();
                    }
                }
                catch (Exception exx)
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        MessageBox.Show("Erro na conecção");
                        conn.Close();
                    }

                }
            });


            decimal temperatura = 0;
            decimal humidade = 0;
            int bateria = 0;
            int i = 0;

            foreach (var item in allSensors)
            {
                temperatura = temperatura + item.Temperature;
                humidade = humidade + item.Humidity;
                bateria = bateria + item.Battery;

                i++;
            }

            this.Invoke((MethodInvoker)delegate ()
            {
                labelTemperatura.Text = "Average of Temperature: " + temperatura.ToString() + " ºC";
                labelHumidade.Text = "Average of Humidity: " + humidade.ToString() + " %";
                labelBateria.Text = "Average of Battery: " + bateria.ToString() + " %";
            });

        }

        private List<Sensor> ConvertReceivedDataSource(string message)
        {
            //ler msg david
            List<Sensor> sensors = new List<Sensor>();
            string[] vs = message.Split('_');
            for (int i = 0; i < vs.Length - 1; i++)
            {
                string[] sens=vs[i].Split('/');
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(sens[4]));
                DateTime dateTime = dateTimeOffset.LocalDateTime;

                Sensor sensor = new Sensor
                {
                    SensorID = int.Parse(sens[0]),
                    Temperature = decimal.Parse(sens[1], CultureInfo.InvariantCulture),
                    Humidity = decimal.Parse(sens[2], CultureInfo.InvariantCulture),
                    Battery = int.Parse(sens[3]),
                    Timestamp = dateTime,
                    ActiveAlert = false,
                    AlertDescription = "",
                };
                sensors.Add(sensor);
            }
            return sensors;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.Unsubscribe(topics);
                mqttClient.Disconnect();
            }
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {

        }

        private void labelTemperatura_Click(object sender, EventArgs e)
        {

        }

        private void labelHumidade_Click(object sender, EventArgs e)
        {

        }
    }
}
