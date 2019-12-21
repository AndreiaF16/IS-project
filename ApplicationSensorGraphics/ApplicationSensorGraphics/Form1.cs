using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms.DataVisualization.Charting;

namespace ApplicationSensorGraphics
{
    public partial class Form1 : Form
    {
        MqttClient mqttClient = null;
        string[] topics = { "source" };
        byte[] qosLevels =
        {
                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE
        };
        List<Sensor> sensors = null;
        List<Sensor> sensor1 = null;
        List<Sensor> sensor2 = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sensors = new List<Sensor>();
            sensor1 = new List<Sensor>();
            sensor2 = new List<Sensor>();
            chartTemp.Visible = false;
            chartHumdidade.Visible = false;
            chartData.Visible = false;
            
            mqttClient = new MqttClient("127.0.0.1");
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
            _ = this.Invoke((MethodInvoker)delegate ()
              {
                  chartTemp.Series.Clear();
                  chartHumdidade.Series.Clear();
                  chartData.Series.Clear();
                  ConvertReceivedDataSource(Encoding.UTF8.GetString(e.Message));

                  decimal temp = 0;
                  decimal hum = 0;
                  int bat = 0;
                  DateTime timestamp = new DateTime(1970, 10, 10, 20, 20, 40);

                  foreach (var item in sensors)
                  {
                      if (item.SensorID == 1)
                      {
                          sensor1.Add(item);
                      }
                      else
                      {
                          sensor2.Add(item);
                      }
                      temp += item.Temperature;
                      hum += item.Humidity;
                      bat += item.Battery;
                      timestamp = item.Timestamp;

                  }
                  temp /= sensors.Count;
                  hum /= sensors.Count;
                  bat /= sensors.Count;
                  

                  if (sensors.Count == 0)
                  {
                      MessageBox.Show("Não existem dados!","Atenção", MessageBoxButtons.OK);
                      chartTemp.Visible = false;
                      chartHumdidade.Visible = false;
                      chartData.Visible = false;
                  }
                  else
                  {
                      chartTemp.Visible = true;
                      chartHumdidade.Visible = true;
                      chartData.Visible = true;
                  }


                  chartTemp.Series.Add("temperatura");
                  chartTemp.Series["temperatura"].IsValueShownAsLabel = true;
                  chartTemp.Series["temperatura"].ChartType = SeriesChartType.Column;
                  chartTemp.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                  chartTemp.Series[0].Points.AddXY("Temperatura", temp);

                  chartHumdidade.Series.Add("humidade");
                  chartHumdidade.Series["humidade"].IsValueShownAsLabel = true;
                  chartHumdidade.Series["humidade"].ChartType = SeriesChartType.Column;
                  chartHumdidade.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                  chartHumdidade.Series[0].Points.AddXY("Humidade", hum);


                  chartData.Series.Add("temperaturaD");

                  chartData.Series["temperaturaD"].IsValueShownAsLabel = true;
                  chartData.Series["temperaturaD"].ChartType = SeriesChartType.Line;

                  chartData.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                  chartData.Series[0].Points.AddXY("Temperatura", temp);
                  chartData.Series[0].Points.AddXY("Humidade", hum);
                  chartData.Series[0].Points.AddXY("Data", timestamp.ToString());
                  
                  chartData.Series[0].Points.AddXY("Bateria", bat);

              });
        }

        private void ConvertReceivedDataSource(string message)
        {
            string[] vs = message.Split('_');
            for (int i = 0; i < vs.Length - 1; i++)
            {
                string[] sens = vs[i].Split('/');
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(sens[4]));
                DateTime dateTime = dateTimeOffset.LocalDateTime;

                Sensor sensor = new Sensor
                {
                    SensorID = int.Parse(sens[0]),
                    Temperature = decimal.Parse(sens[1], CultureInfo.InvariantCulture),
                    Humidity = decimal.Parse(sens[2], CultureInfo.InvariantCulture),
                    Battery = int.Parse(sens[3]),
                    Timestamp = dateTime
                };
                sensors.Add(sensor);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.Unsubscribe(topics);
            }
        }

        private void exempleData()
        {
            /*Sensor s1 = new Sensor
            {
                SensorID = 1,
                Temperature = 23,
                Humidity = 50,
                Battery = 100,
                Timestamp = new DateTime(2019,10,10,20,20,20)
            };

            Sensor s2 = new Sensor
            {
                SensorID = 2,
                Temperature = 25,
                Humidity = 80,
                Battery = 100,
                Timestamp = new DateTime(2019, 10, 10, 20, 20, 20),
            };

            Sensor s3 = new Sensor
            {
                SensorID = 1,
                Temperature = 24,
                Humidity = 59,
                Battery = 96,
                Timestamp = new DateTime(2019, 10, 10, 20, 20, 30),
            };


            Sensor s4 = new Sensor
            {
                SensorID = 2,
                Temperature = 15,
                Humidity = 68,
                Battery = 78,
                Timestamp = new DateTime(2019, 10, 10, 20, 20, 30),
            };

            Sensor s5 = new Sensor
            {
                SensorID = 1,
                Temperature = 18,
                Humidity = 60,
                Battery = 80,
                Timestamp = new DateTime(2019, 10, 10, 20, 20, 40),
            };

            Sensor s6 = new Sensor
            {
                SensorID = 2,
                Temperature = 23,
                Humidity = 85,
                Battery = 90,
                Timestamp = new DateTime(2019, 10, 10, 20, 20, 40),
            };*/

            /*sensors.Add(s1);
            sensors.Add(s2);
            sensors.Add(s3);
            sensors.Add(s4);
            sensors.Add(s5);
            sensors.Add(s6);*/

            decimal temp = 0;
            decimal hum = 0;
            int bat = 0;
            DateTime timestamp = new DateTime(1970, 10, 10, 20, 20, 40);

            foreach (var item in sensors)
            {
                if (item.SensorID == 1)
                {
                    sensor1.Add(item);
                }
                else
                {
                    sensor2.Add(item);
                }
                temp += item.Temperature;
                hum += item.Humidity;
                bat += item.Battery;
                timestamp = item.Timestamp;

            }
            temp /= sensors.Count;
            hum /= sensors.Count;
            bat /= sensors.Count;



            chartTemp.Series.Add("temperatura");
            chartTemp.Series["temperatura"].IsValueShownAsLabel = true;
            chartTemp.Series["temperatura"].ChartType = SeriesChartType.Column;
            chartTemp.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chartTemp.Series[0].Points.AddXY("Temperatura", temp);

            chartHumdidade.Series.Add("humidade");
            chartHumdidade.Series["humidade"].IsValueShownAsLabel = true;
            chartHumdidade.Series["humidade"].ChartType = SeriesChartType.Column;
            chartHumdidade.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chartHumdidade.Series[0].Points.AddXY("Humidade", hum);


            chartData.Series.Add("temperaturaD");

            chartData.Series["temperaturaD"].IsValueShownAsLabel = true;
            chartData.Series["temperaturaD"].ChartType = SeriesChartType.Line;

            chartData.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chartData.Series[0].Points.AddXY("Temperatura", temp);
            chartData.Series[0].Points.AddXY("Data", timestamp.ToString());











        }



        private void chartSensors_Click(object sender, EventArgs e)
        {

        }
    }
}
