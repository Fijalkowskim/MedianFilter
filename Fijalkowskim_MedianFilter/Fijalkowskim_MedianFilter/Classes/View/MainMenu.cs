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
        float filterCooldown = 100f;
        Stopwatch stopwatch;
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


        private void uploadImageButton_Click(object sender, EventArgs e)
        {
            if(controller.dataManager.applyingFilter)
            {
                MessageBox.Show("Wait till last process finishes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Bitmap bitmap = null;
            imageLoadedLabel.Visible = false;

            
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
               // dialog.Filter = "JPG files(*.jpg)|*.jpg;*png|PNG files(*.png)|*.png";
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
            //await controller.dataManager.LoadBitmapAsync(bitmap, numberOfThreads);

        }

        private void ReportImageLoadingProgress(object sender, ImageLoadingProgress e)
        {
            imageLoadingProgress.Value = e.percentageDone;
        }

        private async void filterImageButton_Click(object sender, EventArgs e)
        {
            if(stopwatch.ElapsedMilliseconds < filterCooldown)
            {
                MessageBox.Show("Wait a second before filtering again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            stopwatch.Restart();
            stopwatch.Start();
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
            if (controller.dataManager.dataLoaded)
            {
                DllType dllType = selectAsm.Checked ? DllType.ASM : selectCpp.Checked ? DllType.CPP : DllType.CPP;
                imageLoadingProgress.Value = 0;
                Progress<ImageLoadingProgress> progress = new Progress<ImageLoadingProgress>();
                progress.ProgressChanged += ReportImageLoadingProgress;
                resultImagePreview.Image = await controller.dataManager.UseMedianFilter(dllType,numberOfTasks, progress);

            }
            else
            {
                MessageBox.Show("You must upload an image first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SetExecutionTime(string executionTime, string previousExecutionTime)
        {
            currentExecutionTimeLabel.Text = $"Execution time: {executionTime}ms";
            if (previousExecutionTime != "") previousExecutionTimeLabel.Text = $"Previous execution time: {previousExecutionTime}ms";
        }
        public void SetFilteredBitmap(Bitmap bitmap)
        {
            resultImagePreview.Image = bitmap;
        }

    }
}
