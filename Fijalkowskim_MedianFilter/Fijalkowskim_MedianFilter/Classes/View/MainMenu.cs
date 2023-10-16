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

            currentExecutionTimeLabel.Text = "";
            previousExecutionTimeLabel.Text = "";
        }



        private void openTestWindow_Click(object sender, EventArgs e)
        {
            controller.OpenTestWindow();
        }

        private void uploadImageButton_Click(object sender, EventArgs e)
        {

        }
    }
}
