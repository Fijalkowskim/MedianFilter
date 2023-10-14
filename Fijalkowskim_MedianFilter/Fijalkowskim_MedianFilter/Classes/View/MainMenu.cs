using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Fijalkowskim_MedianFilter
{
    public partial class MainMenu : Form
    {
        Controller controller;
        public MainMenu(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;

            resultLabel.Text = "";
            currentExecutionTimeLabel.Text = "";
            previousExecutionTimeLabel.Text = "";
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxA.Text, out int inputA) && int.TryParse(textBoxB.Text, out int inputB))
            {
                DllType dllType = selectAsm.Checked ? DllType.ASM : selectCpp.Checked ? DllType.CPP : DllType.CPP;
                //double executionTime = 0;
                string executionTime ="";
                string previousExecutionTime="";
                resultLabel.Text = "Result = " + controller.GetFunctionResult(inputA,inputB,dllType, ref executionTime, ref previousExecutionTime);
                currentExecutionTimeLabel.Text = $"Execution time: {executionTime}ms";
                if(previousExecutionTime != "") previousExecutionTimeLabel.Text = $"Previous execution time: {previousExecutionTime}ms";
            }
            else { MessageBox.Show("Enter valid integer numbers"); return; }
            

        }
    }
}
