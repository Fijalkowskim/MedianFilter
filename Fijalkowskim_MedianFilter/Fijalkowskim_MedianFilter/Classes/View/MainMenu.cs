using System;
using System.Diagnostics;
using System.Drawing;
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
        //Constructor for initializing components
        public MainMenu(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;

            currentExecutionTimeLabel.Text = "";
            previousExecutionTimeLabel.Text = "";
            imageLoadedLabel.Visible = false;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        //Event for upload button click
        private void uploadImageButton_Click(object sender, EventArgs e)
        {
            if(controller.dataManager.applyingFilter)
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
            if(stopwatch.ElapsedMilliseconds < filterCooldown)
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
            if(resultImagePreview.Image == null)
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
    }
}
