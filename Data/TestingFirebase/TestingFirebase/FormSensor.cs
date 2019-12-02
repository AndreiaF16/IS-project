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
    public partial class FormSensor : Form
    {
        public Sensor sensor = null;

        public FormSensor(Sensor item)
        {
            InitializeComponent();

            if (item != null)
            {
                textBox1.Text = item.SensorID.ToString();
                textBox2.Text = item.Temperature.ToString();
                textBox3.Text = item.Humidity.ToString();
                textBox4.Text = item.Battery.ToString();
                textBox5.Text = item.Timestamp.ToString();
                checkBox1.Checked = item.Alert;
            }
            else
            {
                textBox5.Text = DateTime.Now.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sensor = new Sensor()
            {
                SensorID = int.Parse(textBox1.Text),
                Temperature = decimal.Parse(textBox2.Text),
                Humidity = decimal.Parse(textBox3.Text),
                Battery = int.Parse(textBox4.Text),
                Timestamp = DateTime.Parse(textBox5.Text),
                Alert = checkBox1.Checked
            };

            this.Close();
        }
    }
}
