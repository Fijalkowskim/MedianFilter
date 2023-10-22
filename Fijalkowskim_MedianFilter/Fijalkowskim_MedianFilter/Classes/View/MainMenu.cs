using System;
using System.Diagnostics;
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


        private void uploadImageButton_Click(object sender, EventArgs e)
        {
            int numberOfThreads;
            try
            {
                numberOfThreads = Int32.Parse(threadsNumber.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter a number of threads between 1 and 64", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (numberOfThreads < 1 || numberOfThreads > 64)
            {
                MessageBox.Show("Enter a number of threads between 1 and 64", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                return; 
            }
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "JPG files(*.jpg)|*.jpg|PNG files(*.png)|*.png";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(dialog.FileName);
                    controller.dataManager.LoadBitmap(bitmap, numberOfThreads);
                    baseImagePreview.Image = bitmap;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void filterImageButton_Click(object sender, EventArgs e)
        {               
            if(controller.dataManager.loadedBitmap != null)
            {
                DllType dllType = selectAsm.Checked ? DllType.ASM : selectCpp.Checked ? DllType.CPP : DllType.CPP;
                string executionTime = "";
                string previousExecutionTime = "";

                resultImagePreview.Image = controller.GetFunctionResult(controller.dataManager.loadedBitmap,
                    dllType, ref executionTime, ref previousExecutionTime);

                currentExecutionTimeLabel.Text = $"Execution time: {executionTime}ms";
                if (previousExecutionTime != "") previousExecutionTimeLabel.Text = $"Previous execution time: {previousExecutionTime}ms";

            }
            else
            {
                MessageBox.Show("You must upload an image first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
