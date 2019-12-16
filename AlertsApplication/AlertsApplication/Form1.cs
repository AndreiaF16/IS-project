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
using ProjectXML;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Web.Script.Serialization;

namespace AlertsApplication
{
    public partial class Form1 : Form
    {
        HandlerXML handler = null;
        MqttClient mqttClient = new MqttClient("127.0.0.1");
        string[] topics = { "source", "alerts" };
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
            handler = new HandlerXML(Environment.CurrentDirectory + "/Alerts.xml", Environment.CurrentDirectory + "/Alerts.xsd");
            updateLists();
            String[] operations = { "between", "equal", "minor", "bigger" };
            comboBoxOperator.DataSource = operations;
            String[] types = { "temperature", "humidity" };
            comboBoxCreateType.DataSource = types;

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
            Sensor sensorToSend = null;
            this.Invoke((MethodInvoker)delegate ()
            {
                if (e.Topic == "source")
                {
                    List<Sensor> sensores = ConvertReceivedDataSource(Encoding.UTF8.GetString(e.Message));
                    foreach (var sensor in sensores)
                    {
                        List<Alert> alertas_ativos = handler.GetActiveAlerts();
                        foreach (var alert in alertas_ativos)
                        {
                            if (alert.Type.Equals("temperature")) {
                                switch (alert.Operator)
                                {
                                    case "between":
                                        if (sensor.Temperature>=alert.Value && sensor.Temperature<=alert.Value2) {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description; //"Temperatura entre " + alert.Value + "ºC e " + alert.Value2 +"ºC";
                                        }
                                        break;
                                    case "equal":
                                        if (sensor.Temperature == alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;//"Temperatura é de " + alert.Value + "ºC";
                                        }
                                        break;
                                    case "minor":
                                        if (sensor.Temperature < alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;//"Temperatura é menor que " + alert.Value + "ºC";
                                        }
                                        break;
                                    case "bigger":
                                        if (sensor.Temperature > alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;//"Temperatura é maior que " + alert.Value + "ºC";
                                        }
                                        break;
                                }
                            }

                            if (alert.Type.Equals("humidity"))
                            {
                                switch (alert.Operator)
                                {
                                    case "between":
                                        if (sensor.Humidity >= alert.Value && sensor.Humidity <= alert.Value2)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;
                                        }
                                        break;
                                    case "equal":
                                        if (sensor.Humidity == alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;
                                        }
                                        break;
                                    case "minor":
                                        if (sensor.Humidity < alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;
                                        }
                                        break;
                                    case "bigger":
                                        if (sensor.Humidity > alert.Value)
                                        {
                                            sensorToSend = sensor;
                                            sensorToSend.ActiveAlert = true;
                                            sensorToSend.AlertDescription = alert.Description;
                                        }
                                        break;
                                }
                            }

                            if (sensorToSend != null) {
                                if (mqttClient.IsConnected)
                                {
                                    mqttClient.Subscribe(topics, qosLevels);
                                }

                                var json = new JavaScriptSerializer().Serialize(sensorToSend);
                                byte[] msg = Encoding.UTF8.GetBytes(json);
                                mqttClient.Publish("alerts", msg);
                                sensorToSend = null;
                            }
                        }
                    }
                }

            });
        }

        private List<Sensor> ConvertReceivedDataSource(string message)
        {
            List<Sensor> sensors = new List<Sensor>();
            string[] vs = message.Split('_');
            for (int i = 0; i < vs.Length - 1; i++)
            {
                string[] sens = vs[i].Split('/');
                //DateTime Timestamp = DateTimeOffset.ParseExact(sens[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture).UtcDateTime;
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
                    AlertDescription ="",
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
        private void buttonCreateAlert_Click(object sender, EventArgs e)
        {
            if (comboBoxOperator.SelectedItem.ToString().Equals("between"))
            {
                string troca;
                if (double.Parse(textBoxCreateValue.Text) > double.Parse(textBoxCreateValue2.Text))
                {
                    troca = textBoxCreateValue.Text;
                    textBoxCreateValue.Text = textBoxCreateValue2.Text;
                    textBoxCreateValue2.Text = troca;
                }
                else if (double.Parse(textBoxCreateValue.Text) == double.Parse(textBoxCreateValue2.Text))
                {
                    MessageBox.Show("Operador between não é apropriado para o seu alerta\nÉ aconselhável usar o equal para este alerta");
                    return;
                }
            }
            Alert alert = null;
            if (textBoxCreateValue2.Text.Equals(""))
            {
                alert = new Alert
                {
                    ID = int.Parse(textBoxCreateId.Text),
                    Type = comboBoxCreateType.SelectedItem.ToString(),
                    Operator = comboBoxOperator.SelectedItem.ToString(),
                    Value = decimal.Parse(textBoxCreateValue.Text, CultureInfo.InvariantCulture),
                    Enable = checkBoxCreateEnable.Checked,
                    Description = textBoxCreateDescription.Text,
                };
            }
            else {
                alert = new Alert
                {
                    ID = int.Parse(textBoxCreateId.Text),
                    Type = comboBoxCreateType.SelectedItem.ToString(),
                    Operator = comboBoxOperator.SelectedItem.ToString(),
                    Value = decimal.Parse(textBoxCreateValue.Text, CultureInfo.InvariantCulture),
                    Value2 = decimal.Parse(textBoxCreateValue2.Text, CultureInfo.InvariantCulture),
                    Enable = checkBoxCreateEnable.Checked,
                    Description = textBoxCreateDescription.Text,
                };
            }
            
            handler.AddAlert(alert);
            updateLists();
            MessageBox.Show("Alerta Adicionado");
        }

        private void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!comboBoxOperator.SelectedItem.ToString().Equals("between"))
            {
                textBoxCreateValue2.Enabled = false;
            }
            else
            {
                textBoxCreateValue2.Enabled = true;
            }
        }

        private void listBoxEditAlertas_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] vs = listBoxEditAlertas.SelectedItem.ToString().Split(':');
            string[] vs1 = vs[1].Split(' ');
            Alert alert = handler.GetAlertById(vs1[1]);
            if (alert.Enable)
            {
                buttonEnableAlert.Enabled = false;
                buttonDisableAlert.Enabled = true;
            }
            else {
                buttonEnableAlert.Enabled = true;
                buttonDisableAlert.Enabled = false;
            }
        }

        private void updateLists() {
            listBoxEditAlertas.DataSource = handler.GetAlertsId();
            dataGridViewListar.DataSource = handler.GetAlerts();
            dataGridViewEliminar.DataSource = handler.GetAlerts();
        }

        private void buttonEnableAlert_Click(object sender, EventArgs e)
        {
            string[] vs = listBoxEditAlertas.SelectedItem.ToString().Split(':');
            string[] vs1 = vs[1].Split(' ');
            handler.UpdateAlertEnableById(int.Parse(vs1[1]),true);
            updateLists();
        }

        private void buttonDisableAlert_Click(object sender, EventArgs e)
        {
            string[] vs = listBoxEditAlertas.SelectedItem.ToString().Split(':');
            string[] vs1 = vs[1].Split(' ');
            handler.UpdateAlertEnableById(int.Parse(vs1[1]), false);
            updateLists();
        }

        private void buttonEliminarAlerta_Click(object sender, EventArgs e)
        {
            int i = dataGridViewEliminar.CurrentCell.RowIndex;
            int id = int.Parse(dataGridViewEliminar.Rows[i].Cells[0].Value.ToString());
            handler.DeleteAlertById(id);
            updateLists();
        }
    }
}
