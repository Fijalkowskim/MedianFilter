
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
            this.calculateButton = new System.Windows.Forms.Button();
            this.textBoxA = new System.Windows.Forms.TextBox();
            this.textBoxB = new System.Windows.Forms.TextBox();
            this.textBoxALabel = new System.Windows.Forms.Label();
            this.textBoxBLabel = new System.Windows.Forms.Label();
            this.resultLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectAsm = new System.Windows.Forms.RadioButton();
            this.libraryLabel = new System.Windows.Forms.Label();
            this.selectCpp = new System.Windows.Forms.RadioButton();
            this.currentExecutionTimeLabel = new System.Windows.Forms.Label();
            this.previousExecutionTimeLabel = new System.Windows.Forms.Label();
            this.plusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // calculateButton
            // 
            this.calculateButton.Location = new System.Drawing.Point(54, 218);
            this.calculateButton.Margin = new System.Windows.Forms.Padding(5);
            this.calculateButton.Name = "calculateButton";
            this.calculateButton.Size = new System.Drawing.Size(129, 40);
            this.calculateButton.TabIndex = 1;
            this.calculateButton.Text = "Calculate";
            this.calculateButton.UseVisualStyleBackColor = true;
            this.calculateButton.Click += new System.EventHandler(this.calculateButton_Click);
            // 
            // textBoxA
            // 
            this.textBoxA.Location = new System.Drawing.Point(54, 56);
            this.textBoxA.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxA.Name = "textBoxA";
            this.textBoxA.Size = new System.Drawing.Size(146, 33);
            this.textBoxA.TabIndex = 2;
            // 
            // textBoxB
            // 
            this.textBoxB.Location = new System.Drawing.Point(256, 56);
            this.textBoxB.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxB.Name = "textBoxB";
            this.textBoxB.Size = new System.Drawing.Size(146, 33);
            this.textBoxB.TabIndex = 3;
            // 
            // textBoxALabel
            // 
            this.textBoxALabel.AutoSize = true;
            this.textBoxALabel.Location = new System.Drawing.Point(113, 25);
            this.textBoxALabel.Name = "textBoxALabel";
            this.textBoxALabel.Size = new System.Drawing.Size(23, 26);
            this.textBoxALabel.TabIndex = 4;
            this.textBoxALabel.Text = "a";
            // 
            // textBoxBLabel
            // 
            this.textBoxBLabel.AutoSize = true;
            this.textBoxBLabel.Location = new System.Drawing.Point(319, 25);
            this.textBoxBLabel.Name = "textBoxBLabel";
            this.textBoxBLabel.Size = new System.Drawing.Size(23, 26);
            this.textBoxBLabel.TabIndex = 5;
            this.textBoxBLabel.Text = "b";
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(235, 225);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(69, 26);
            this.resultLabel.TabIndex = 6;
            this.resultLabel.Text = "Result";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.selectAsm);
            this.groupBox1.Controls.Add(this.libraryLabel);
            this.groupBox1.Controls.Add(this.selectCpp);
            this.groupBox1.Location = new System.Drawing.Point(54, 106);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 83);
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
            // currentExecutionTimeLabel
            // 
            this.currentExecutionTimeLabel.AutoSize = true;
            this.currentExecutionTimeLabel.Location = new System.Drawing.Point(54, 296);
            this.currentExecutionTimeLabel.Name = "currentExecutionTimeLabel";
            this.currentExecutionTimeLabel.Size = new System.Drawing.Size(225, 26);
            this.currentExecutionTimeLabel.TabIndex = 8;
            this.currentExecutionTimeLabel.Text = "Current execution Time";
            // 
            // previousExecutionTimeLabel
            // 
            this.previousExecutionTimeLabel.AutoSize = true;
            this.previousExecutionTimeLabel.Location = new System.Drawing.Point(54, 336);
            this.previousExecutionTimeLabel.Name = "previousExecutionTimeLabel";
            this.previousExecutionTimeLabel.Size = new System.Drawing.Size(233, 26);
            this.previousExecutionTimeLabel.TabIndex = 9;
            this.previousExecutionTimeLabel.Text = "Previous execution time:";
            // 
            // plusLabel
            // 
            this.plusLabel.AutoSize = true;
            this.plusLabel.Location = new System.Drawing.Point(464, 56);
            this.plusLabel.Name = "plusLabel";
            this.plusLabel.Size = new System.Drawing.Size(95, 26);
            this.plusLabel.TabIndex = 10;
            this.plusLabel.Text = "result = a";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto Slab", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(551, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 19);
            this.label1.TabIndex = 11;
            this.label1.Text = "b";
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 420);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.plusLabel);
            this.Controls.Add(this.previousExecutionTimeLabel);
            this.Controls.Add(this.currentExecutionTimeLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.resultLabel);
            this.Controls.Add(this.textBoxBLabel);
            this.Controls.Add(this.textBoxALabel);
            this.Controls.Add(this.textBoxB);
            this.Controls.Add(this.textBoxA);
            this.Controls.Add(this.calculateButton);
            this.Font = new System.Drawing.Font("Roboto Slab", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainMenu";
            this.Text = "Median Filter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button calculateButton;
        private System.Windows.Forms.TextBox textBoxA;
        private System.Windows.Forms.TextBox textBoxB;
        private System.Windows.Forms.Label textBoxALabel;
        private System.Windows.Forms.Label textBoxBLabel;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton selectCpp;
        private System.Windows.Forms.RadioButton selectAsm;
        private System.Windows.Forms.Label libraryLabel;
        private System.Windows.Forms.Label currentExecutionTimeLabel;
        private System.Windows.Forms.Label previousExecutionTimeLabel;
        private System.Windows.Forms.Label plusLabel;
        private System.Windows.Forms.Label label1;
    }
}

