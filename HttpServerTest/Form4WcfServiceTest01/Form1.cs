using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form4WcfServiceTest01
{
    public partial class Form1 : Form
    {
        CalcService.ICalc calc;

        public Form1()
        {
            InitializeComponent();

            calc = new CalcService.CalcClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(calc.GetData(new Random().Next()));
        }
    }
}
