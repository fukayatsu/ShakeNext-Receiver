using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace SN_Receiver1
{
    public partial class Form2 : Form
    {
        Form1 form1 = null;
        public Form2(Form1 f1)
        {
            InitializeComponent();
            form1 = f1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try {
                SerialPort sp = form1.getPort();
                sp.Close();

                sp.PortName = comboBox1.Text;
                sp.Encoding = Encoding.GetEncoding("Shift-JIS");
                sp.Open();
                this.Dispose();
            }catch(Exception e1){
                MessageBox.Show("受信用仮想COMポートが開けませんでした\n"+e1.ToString());
            }

            Properties.Settings ms = Properties.Settings.Default;
            try
            {
                //MessageBox.Show("save:"+comboBox1.SelectedItem.ToString());
                Properties.Settings.Default.cmb1 = comboBox1.SelectedItem.ToString();
                Properties.Settings.Default.b1 = comboBox2.SelectedItem.ToString();
                Properties.Settings.Default.b2 = comboBox3.SelectedItem.ToString();
                Properties.Settings.Default.b3 = comboBox4.SelectedItem.ToString();
                Properties.Settings.Default.b4 = comboBox5.SelectedItem.ToString();
                Properties.Settings.Default.l1 = comboBox6.SelectedItem.ToString();
                Properties.Settings.Default.l2 = comboBox7.SelectedItem.ToString();
                Properties.Settings.Default.m1 = comboBox8.SelectedItem.ToString();
                Properties.Settings.Default.m2 = comboBox9.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
            

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);

            String[] command = { "{HOME}", "{END}", "{LEFT}", "{RIGHT}", "{UP}", "{DOWN}", "{ESC}", "+{F5}" };
            comboBox2.Items.AddRange(command);
            comboBox3.Items.AddRange(command);
            comboBox4.Items.AddRange(command);
            comboBox5.Items.AddRange(command);
            comboBox6.Items.AddRange(command);
            comboBox7.Items.AddRange(command);
            comboBox8.Items.AddRange(command);
            comboBox9.Items.AddRange(command);

            Properties.Settings ms = Properties.Settings.Default;
            try
            {

                comboBox1.SelectedItem = ms.cmb1;
                comboBox2.SelectedItem = ms.b1;
                comboBox3.SelectedItem = ms.b2;
                comboBox4.SelectedItem = ms.b3;
                comboBox5.SelectedItem = ms.b4;
                comboBox6.SelectedItem = ms.l1;
                comboBox7.SelectedItem = ms.l2;
                comboBox8.SelectedItem = ms.m1;
                comboBox9.SelectedItem = ms.m2;
            }catch(Exception e1){
                MessageBox.Show(e1.ToString());
            }
        }

        
    }
}
