using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

// Mateusz Fijałkowski
// Median Filter v1 - 14.01.2024
// Silesian University of Technology 2023/24


namespace Fijalkowskim_MedianFilter
{
    //MainMenu class controlls behaviour of user interface (button click events, image loading etc.)
    public partial class MainMenu : Form
    {
        //Controller for interacting with data manager
        Controller controller;
        //Cooldown for clicking filter button
        float filterCooldown = 100f;
        //Stopwatch for time measurements
        Stopwatch stopwatch;
        //Controlling if simulation of filtering with tasks from 1 to 64 is finished
        bool simulating = false;
        //Constructor for initializing components
        public MainMenu(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
            simulating = false;
            currentExecutionTimeLabel.Text = "";
            previousExecutionTimeLabel.Text = "";
            imageLoadedLabel.Visible = false;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        //Event for upload button click
        private void uploadImageButton_Click(object sender, EventArgs e)
        {
            if(controller.dataManager.applyingFilter || simulating)
            {
                MessageBox.Show("Wait untill last process finishes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap bitmap = null;
            imageLoadedLabel.Visible = false;   
            try
            {
                //Loading image using dialog box
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files (*.jpg;*.png)|*.jpg;*png";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    imageLoadingProgress.Value = 0;
                    bitmap = new Bitmap(dialog.FileName);
                    controller.dataManager.LoadBitmap(bitmap);
                    baseImagePreview.Image = bitmap;
                    imageLoadedLabel.Text = "Image loaded";
                    imageLoadedLabel.Visible = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong file selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Method for reporting progress into progress bar
        private void ReportImageLoadingProgress(object sender, ImageLoadingProgress e)
        {
            imageLoadingProgress.Value = e.percentageDone;
        }
        //Event for filter button click
        private async void filterImageButton_Click(object sender, EventArgs e)
        {
            //Click cooldown
            if(stopwatch.ElapsedMilliseconds < filterCooldown || simulating)
            {
                MessageBox.Show("Wait a second before filtering again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            stopwatch.Restart();
            stopwatch.Start();
            //Getting number of tasks from input box
            int numberOfTasks;
            try
            {
                numberOfTasks = Int32.Parse(threadsNumber.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Enter a number of threads between 1 and 64", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (numberOfTasks < 1 || numberOfTasks > 64)
            {
                MessageBox.Show("Enter a number of threads between 1 and 64", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Calling UseMedianFilter method on DataManager and awaiting for result
            if (controller.dataManager.dataLoaded)
            {
                //Set proper dll type (c++ or assembly) from checkboxes
                DllType dllType = selectAsm.Checked ? DllType.ASM : selectCpp.Checked ? DllType.CPP : DllType.CPP;
                imageLoadingProgress.Value = 0;
                Progress<ImageLoadingProgress> progress = new Progress<ImageLoadingProgress>();
                //Register progress event
                progress.ProgressChanged += ReportImageLoadingProgress;
                //Set filtered image preview
                resultImagePreview.Image = await controller.dataManager.UseMedianFilter(dllType,numberOfTasks, progress);

            }
            else
            {
                MessageBox.Show("You must upload an image first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Displaying execution time of filtering
        public void SetExecutionTime(string executionTime, string previousExecutionTime)
        {
            currentExecutionTimeLabel.Text = $"Execution time: {executionTime}ms";
            if (previousExecutionTime != "") previousExecutionTimeLabel.Text = $"Previous execution time: {previousExecutionTime}ms";
        }
        //Event for save button click
        private void saveButton_Click(object sender, EventArgs e)
        {
            if(resultImagePreview.Image == null || simulating)
            {
                MessageBox.Show("There is no image to save", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Save filtered image using dialog box
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Image files (*.png;*.jpg)|*.png;*jpg";
            if(sf.ShowDialog() == DialogResult.OK)
            {
                resultImagePreview.Image.Save(sf.FileName);
            }
        }
        //Helper button for printing execution time of filtering into text file
        private async void simulateBtn_Click(object sender, EventArgs e)
        {
            if (!controller.dataManager.dataLoaded)
            {
                MessageBox.Show("You must upload an image first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (simulating)
            {
                MessageBox.Show("Wait for simulation to be over", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            simulating = true;
            string fileName = controller.dataManager.loadedBitmap.Width.ToString() + "x" + controller.dataManager.loadedBitmap.Height.ToString();
            DllType dllType = selectAsm.Checked ? DllType.ASM : selectCpp.Checked ? DllType.CPP : DllType.CPP;
            fileName += dllType == DllType.ASM ? " ASM" : " CPP";
            fileName += ".txt";
            using (StreamWriter file = new StreamWriter(fileName, false))
            {
                file.WriteLine("\"Tasks\",\"Time\"");
                //Simulating several times for accurate time
                int simulationsForTask = 6;
                long[] times = new long[simulationsForTask];
                imageLoadedLabel.Text = "Simulated tasks: ";
                imageLoadedLabel.Visible = true;
                //Simulate number of tasks from 1 to 64
                for (int i = 1; i <= 64; i++)
                {
                    imageLoadedLabel.Text = "Simulated tasks: " + i.ToString();
                    for (int j = 0; j < simulationsForTask; j++)
                    {
                        times[j] = await controller.dataManager.SimulateFiltering(dllType, i);
                    }
                    file.WriteLine(i.ToString() + "," + times.Min());
                }
            }
            imageLoadedLabel.Text = "Simulation done";
            simulating = false;
        }
    }
}
