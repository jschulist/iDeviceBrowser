using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    public partial class FavoriteEditDialog : Form
    {
        public Favorite Favorite
        {
            get { return new Favorite(this.NameTextBox.Text, this.PathTextBox.Text); }
            set
            {
                this.NameTextBox.Text = value.Name;
                this.PathTextBox.Text = value.Path;
            }
        }

        public FavoriteEditDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
