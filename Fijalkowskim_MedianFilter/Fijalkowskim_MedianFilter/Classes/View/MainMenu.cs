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
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "JPG files(*.jpg)|*.jpg|PNG files(*.png)|*.png";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bitmap = new Bitmap(dialog.FileName);
                    baseImagePreview.Image = bitmap;
                    controller.dataManager.loadedBitmap = bitmap;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void filterImageButton_Click(object sender, EventArgs e)
        {
            if(controller.dataManager.loadedBitmap != null)
            {
                resultImagePreview.Image = controller.dataManager.MedianFiltering(controller.dataManager.loadedBitmap);
            }
            else
            {
                MessageBox.Show("You must upload image first");
            }
        }
    }
}
