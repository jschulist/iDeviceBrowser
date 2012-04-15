using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    public partial class InputBox : Form
    {
        private InputBox()
        {
            InitializeComponent();
        }

        public string Result
        {
            get
            {
                return textBox.Text;
            }
        }

        public InputBox(string caption, string message)
            : this()
        {
            this.Text = caption;
            this.message.Text = message;
        }

        private void message_Resize(object sender, EventArgs e)
        {
            this.Size = new Size(this.Width, message.Height + 106);
        }

        public static DialogResult Show(string caption, string message, ref string result)
        {
            InputBox inputBox = new InputBox(caption, message);

            DialogResult dialogResult = inputBox.ShowDialog();
            result = inputBox.Result;

            return dialogResult;
        }
    }
}
