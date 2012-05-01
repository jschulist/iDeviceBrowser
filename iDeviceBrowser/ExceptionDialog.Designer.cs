namespace iDeviceBrowser
{
    partial class ExceptionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ExceptionTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CopyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ExceptionTextBox
            // 
            this.ExceptionTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ExceptionTextBox.Location = new System.Drawing.Point(12, 38);
            this.ExceptionTextBox.Multiline = true;
            this.ExceptionTextBox.Name = "ExceptionTextBox";
            this.ExceptionTextBox.ReadOnly = true;
            this.ExceptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ExceptionTextBox.Size = new System.Drawing.Size(355, 228);
            this.ExceptionTextBox.TabIndex = 0;
            this.ExceptionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ExceptionTextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(350, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "An unhandled exception occured.  Please copy the exception below and\r\nreport it a" +
    "s an issue on the project website.";
            // 
            // CopyButton
            // 
            this.CopyButton.Location = new System.Drawing.Point(292, 272);
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(75, 23);
            this.CopyButton.TabIndex = 2;
            this.CopyButton.Text = "Copy";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // ExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 307);
            this.Controls.Add(this.CopyButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExceptionTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExceptionDialog";
            this.Text = "Exception";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ExceptionTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CopyButton;
    }
}