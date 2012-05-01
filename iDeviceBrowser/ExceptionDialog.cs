using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    public partial class ExceptionDialog : Form
    {
        private Exception _exception;
        public Exception Exception
        {
            set
            {
                _exception = value;
                this.ExceptionTextBox.Text = value.ToString();
            }
        }

        public ExceptionDialog()
        {
            InitializeComponent();
            this.Icon = System.Drawing.SystemIcons.Error;
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            AddExceptionToClipboard();
        }

        private void ExceptionTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // select all
            if (e.Control && e.KeyCode == Keys.A)
            {
                this.ExceptionTextBox.SelectAll();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                this.ExceptionTextBox.SelectAll();
                AddExceptionToClipboard();
            }
        }

        private void AddExceptionToClipboard()
        {
            Clipboard.SetText(this.ExceptionTextBox.Text);
        }
    }
}
