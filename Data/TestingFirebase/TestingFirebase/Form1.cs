using IPLeiriaSmartCampusFireBaseIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestingFirebase
{
    public partial class Form1 : Form
    {
        private IPLeiriaSmartCampusDatabase database;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Connecting to database... ";
            database = new IPLeiriaSmartCampusDatabase();
            if (database.IsConnected())
            {
                textBox1.Text += "connected!\n";
            }
            else
            {
                textBox1.Text += "not connected!\n";
            }
        }

        private async void buttonCreate_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Creating sensor... ";
            Sensor sensor = null;
            FormSensor formSensor = new FormSensor(null);
            formSensor.ShowDialog();
            sensor = formSensor.sensor;
            if (sensor == null)
            {
                textBox1.Text += "Canceled!\n";
                return;
            }
            textBox1.Text += "Done!\n";


            textBox1.Text += "Adding sensor into database... ";
            if (await database.CreateSensor(sensor))
            {
                textBox1.Text += "Done!\n";
            }
            else
            {
                textBox1.Text += "Error!\n";
            }
        }

        private async void buttonGet_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Get sensors from database... ";
            listBox1.Items.Clear();
            List<Sensor> sensors = await database.GetSensors();
            if (sensors == null)
            {
                textBox1.Text += "Error!\n";
                return;
            }

            if (sensors.Count > 0)
            {
                foreach (Sensor item in sensors)
                {
                    listBox1.Items.Add(item.Key + " " + item.Temperature);
                }
                textBox1.Text += "Done!\n";
            }
            else
            {
                textBox1.Text += "Database is empty!\n";
            }

        }

        private async void buttonPut_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Updating sensor... ";
            String key = listBox1.SelectedItem.ToString().Split(' ')[0];
            Sensor sensorToUpdate = await database.GetSensorById(key);
            Sensor sensor = new Sensor();

            FormSensor formSensor = new FormSensor(sensorToUpdate);
            formSensor.ShowDialog();
            sensor = formSensor.sensor;
            sensor.Key = sensorToUpdate.Key;
            textBox1.Text += "Done\n";

            textBox1.Text += "Saving sensor(" + sensor.Key + ") into database... ";
            if (await database.UpdateSensor(sensor))
            {
                textBox1.Text += "Done!\n";
            }
            else
            {
                textBox1.Text += "Error!\n";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            String key = listBox1.SelectedItem.ToString().Split(' ')[0];
            Sensor sensor = await database.GetSensorById(key);

            textBox1.Text += "Deleting sensor(" + sensor.Key + ")... ";
            if (await database.DeleteSensor(sensor))
            {
                textBox1.Text += "Done!\n";
            }
            else
            {
                textBox1.Text += "Error!\n";
            }
        }
    }
}
