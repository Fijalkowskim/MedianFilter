
namespace Fijalkowskim_MedianFilter
{
    partial class MainMenu
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectAsm = new System.Windows.Forms.RadioButton();
            this.libraryLabel = new System.Windows.Forms.Label();
            this.selectCpp = new System.Windows.Forms.RadioButton();
            this.previousExecutionTimeLabel = new System.Windows.Forms.Label();
            this.currentExecutionTimeLabel = new System.Windows.Forms.Label();
            this.baseImagePreview = new System.Windows.Forms.PictureBox();
            this.basePreviewGroup = new System.Windows.Forms.GroupBox();
            this.resultPreviewGroup = new System.Windows.Forms.GroupBox();
            this.resultImagePreview = new System.Windows.Forms.PictureBox();
            this.baseImageLabel = new System.Windows.Forms.Label();
            this.resultImageLabel = new System.Windows.Forms.Label();
            this.uploadImageButton = new System.Windows.Forms.Button();
            this.filterImageButton = new System.Windows.Forms.Button();
            this.threadsNumber = new System.Windows.Forms.TextBox();
            this.enterThreadsNumber = new System.Windows.Forms.Label();
            this.testLabel = new System.Windows.Forms.Label();
            this.imageLoadingProgress = new System.Windows.Forms.ProgressBar();
            this.imageLoadedLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseImagePreview)).BeginInit();
            this.basePreviewGroup.SuspendLayout();
            this.resultPreviewGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultImagePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.selectAsm);
            this.groupBox1.Controls.Add(this.libraryLabel);
            this.groupBox1.Controls.Add(this.selectCpp);
            this.groupBox1.Location = new System.Drawing.Point(44, 383);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(405, 90);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // selectAsm
            // 
            this.selectAsm.AutoSize = true;
            this.selectAsm.Location = new System.Drawing.Point(181, 47);
            this.selectAsm.Name = "selectAsm";
            this.selectAsm.Size = new System.Drawing.Size(73, 30);
            this.selectAsm.TabIndex = 2;
            this.selectAsm.Text = "ASM";
            this.selectAsm.UseVisualStyleBackColor = true;
            // 
            // libraryLabel
            // 
            this.libraryLabel.AutoSize = true;
            this.libraryLabel.Location = new System.Drawing.Point(18, 18);
            this.libraryLabel.Name = "libraryLabel";
            this.libraryLabel.Size = new System.Drawing.Size(289, 26);
            this.libraryLabel.TabIndex = 1;
            this.libraryLabel.Text = "Choose library you want to use";
            // 
            // selectCpp
            // 
            this.selectCpp.AutoSize = true;
            this.selectCpp.Checked = true;
            this.selectCpp.Location = new System.Drawing.Point(18, 47);
            this.selectCpp.Name = "selectCpp";
            this.selectCpp.Size = new System.Drawing.Size(64, 30);
            this.selectCpp.TabIndex = 0;
            this.selectCpp.TabStop = true;
            this.selectCpp.Text = "C++";
            this.selectCpp.UseVisualStyleBackColor = true;
            // 
            // previousExecutionTimeLabel
            // 
            this.previousExecutionTimeLabel.AutoSize = true;
            this.previousExecutionTimeLabel.Location = new System.Drawing.Point(502, 469);
            this.previousExecutionTimeLabel.Name = "previousExecutionTimeLabel";
            this.previousExecutionTimeLabel.Size = new System.Drawing.Size(233, 26);
            this.previousExecutionTimeLabel.TabIndex = 9;
            this.previousExecutionTimeLabel.Text = "Previous execution time:";
            // 
            // currentExecutionTimeLabel
            // 
            this.currentExecutionTimeLabel.AutoSize = true;
            this.currentExecutionTimeLabel.Location = new System.Drawing.Point(502, 423);
            this.currentExecutionTimeLabel.Name = "currentExecutionTimeLabel";
            this.currentExecutionTimeLabel.Size = new System.Drawing.Size(225, 26);
            this.currentExecutionTimeLabel.TabIndex = 8;
            this.currentExecutionTimeLabel.Text = "Current execution Time";
            // 
            // baseImagePreview
            // 
            this.baseImagePreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.baseImagePreview.Location = new System.Drawing.Point(6, 16);
            this.baseImagePreview.Name = "baseImagePreview";
            this.baseImagePreview.Size = new System.Drawing.Size(505, 319);
            this.baseImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.baseImagePreview.TabIndex = 13;
            this.baseImagePreview.TabStop = false;
            // 
            // basePreviewGroup
            // 
            this.basePreviewGroup.BackColor = System.Drawing.Color.Silver;
            this.basePreviewGroup.Controls.Add(this.baseImagePreview);
            this.basePreviewGroup.Location = new System.Drawing.Point(27, 36);
            this.basePreviewGroup.Name = "basePreviewGroup";
            this.basePreviewGroup.Size = new System.Drawing.Size(517, 341);
            this.basePreviewGroup.TabIndex = 14;
            this.basePreviewGroup.TabStop = false;
            // 
            // resultPreviewGroup
            // 
            this.resultPreviewGroup.BackColor = System.Drawing.Color.Silver;
            this.resultPreviewGroup.Controls.Add(this.resultImagePreview);
            this.resultPreviewGroup.Location = new System.Drawing.Point(580, 36);
            this.resultPreviewGroup.Name = "resultPreviewGroup";
            this.resultPreviewGroup.Size = new System.Drawing.Size(517, 341);
            this.resultPreviewGroup.TabIndex = 15;
            this.resultPreviewGroup.TabStop = false;
            // 
            // resultImagePreview
            // 
            this.resultImagePreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.resultImagePreview.Location = new System.Drawing.Point(6, 16);
            this.resultImagePreview.Name = "resultImagePreview";
            this.resultImagePreview.Size = new System.Drawing.Size(505, 319);
            this.resultImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.resultImagePreview.TabIndex = 13;
            this.resultImagePreview.TabStop = false;
            // 
            // baseImageLabel
            // 
            this.baseImageLabel.AutoSize = true;
            this.baseImageLabel.Location = new System.Drawing.Point(27, 7);
            this.baseImageLabel.Name = "baseImageLabel";
            this.baseImageLabel.Size = new System.Drawing.Size(119, 26);
            this.baseImageLabel.TabIndex = 16;
            this.baseImageLabel.Text = "Base image:";
            // 
            // resultImageLabel
            // 
            this.resultImageLabel.AutoSize = true;
            this.resultImageLabel.Location = new System.Drawing.Point(580, 7);
            this.resultImageLabel.Name = "resultImageLabel";
            this.resultImageLabel.Size = new System.Drawing.Size(147, 26);
            this.resultImageLabel.TabIndex = 17;
            this.resultImageLabel.Text = "Filtered image:";
            // 
            // uploadImageButton
            // 
            this.uploadImageButton.Location = new System.Drawing.Point(44, 516);
            this.uploadImageButton.Margin = new System.Windows.Forms.Padding(5);
            this.uploadImageButton.Name = "uploadImageButton";
            this.uploadImageButton.Size = new System.Drawing.Size(190, 58);
            this.uploadImageButton.TabIndex = 18;
            this.uploadImageButton.Text = "Upload image";
            this.uploadImageButton.UseVisualStyleBackColor = true;
            this.uploadImageButton.Click += new System.EventHandler(this.uploadImageButton_Click);
            // 
            // filterImageButton
            // 
            this.filterImageButton.Location = new System.Drawing.Point(259, 516);
            this.filterImageButton.Margin = new System.Windows.Forms.Padding(5);
            this.filterImageButton.Name = "filterImageButton";
            this.filterImageButton.Size = new System.Drawing.Size(190, 58);
            this.filterImageButton.TabIndex = 19;
            this.filterImageButton.Text = "Filter image";
            this.filterImageButton.UseVisualStyleBackColor = true;
            this.filterImageButton.Click += new System.EventHandler(this.filterImageButton_Click);
            // 
            // threadsNumber
            // 
            this.threadsNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.threadsNumber.Location = new System.Drawing.Point(336, 485);
            this.threadsNumber.MaxLength = 2;
            this.threadsNumber.Name = "threadsNumber";
            this.threadsNumber.PlaceholderText = "1";
            this.threadsNumber.Size = new System.Drawing.Size(50, 26);
            this.threadsNumber.TabIndex = 20;
            this.threadsNumber.Text = "1";
            this.threadsNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // enterThreadsNumber
            // 
            this.enterThreadsNumber.AutoSize = true;
            this.enterThreadsNumber.Location = new System.Drawing.Point(44, 485);
            this.enterThreadsNumber.Name = "enterThreadsNumber";
            this.enterThreadsNumber.Size = new System.Drawing.Size(286, 26);
            this.enterThreadsNumber.TabIndex = 21;
            this.enterThreadsNumber.Text = "Enter number of threads (1-64)";
            // 
            // testLabel
            // 
            this.testLabel.AutoSize = true;
            this.testLabel.Location = new System.Drawing.Point(795, 423);
            this.testLabel.Name = "testLabel";
            this.testLabel.Size = new System.Drawing.Size(0, 26);
            this.testLabel.TabIndex = 22;
            // 
            // imageLoadingProgress
            // 
            this.imageLoadingProgress.Location = new System.Drawing.Point(46, 582);
            this.imageLoadingProgress.Name = "imageLoadingProgress";
            this.imageLoadingProgress.Size = new System.Drawing.Size(403, 23);
            this.imageLoadingProgress.TabIndex = 23;
            // 
            // imageLoadedLabel
            // 
            this.imageLoadedLabel.AutoSize = true;
            this.imageLoadedLabel.Font = new System.Drawing.Font("Roboto Slab", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.imageLoadedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.imageLoadedLabel.Location = new System.Drawing.Point(110, 608);
            this.imageLoadedLabel.Name = "imageLoadedLabel";
            this.imageLoadedLabel.Size = new System.Drawing.Size(258, 50);
            this.imageLoadedLabel.TabIndex = 24;
            this.imageLoadedLabel.Text = "Image loaded";
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1117, 660);
            this.Controls.Add(this.imageLoadedLabel);
            this.Controls.Add(this.imageLoadingProgress);
            this.Controls.Add(this.testLabel);
            this.Controls.Add(this.enterThreadsNumber);
            this.Controls.Add(this.threadsNumber);
            this.Controls.Add(this.filterImageButton);
            this.Controls.Add(this.uploadImageButton);
            this.Controls.Add(this.resultImageLabel);
            this.Controls.Add(this.baseImageLabel);
            this.Controls.Add(this.resultPreviewGroup);
            this.Controls.Add(this.basePreviewGroup);
            this.Controls.Add(this.previousExecutionTimeLabel);
            this.Controls.Add(this.currentExecutionTimeLabel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Roboto Slab", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainMenu";
            this.Text = "Median Filter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseImagePreview)).EndInit();
            this.basePreviewGroup.ResumeLayout(false);
            this.resultPreviewGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultImagePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton selectCpp;
        private System.Windows.Forms.Label libraryLabel;
        private System.Windows.Forms.Label previousExecutionTimeLabel;
        private System.Windows.Forms.RadioButton selectAsm;
        private System.Windows.Forms.Label currentExecutionTimeLabel;
        private System.Windows.Forms.PictureBox baseImagePreview;
        private System.Windows.Forms.GroupBox basePreviewGroup;
        private System.Windows.Forms.GroupBox resultPreviewGroup;
        private System.Windows.Forms.PictureBox resultImagePreview;
        private System.Windows.Forms.Label baseImageLabel;
        private System.Windows.Forms.Label resultImageLabel;
        private System.Windows.Forms.Button uploadImageButton;
        private System.Windows.Forms.Button filterImageButton;
        private System.Windows.Forms.TextBox threadsNumber;
        private System.Windows.Forms.Label enterThreadsNumber;
        private System.Windows.Forms.Label testLabel;
        private System.Windows.Forms.ProgressBar imageLoadingProgress;
        private System.Windows.Forms.Label imageLoadedLabel;
    }
}

