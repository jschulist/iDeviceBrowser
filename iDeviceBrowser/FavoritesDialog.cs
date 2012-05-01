using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    // TODO: ADD VALIDATION
    public partial class FavoritesDialog : Form
    {
        public List<Favorite> Favorites
        {
            get { return GetFavorites(); }
            set { PopulateFavoritesListView(value); }
        }

        public FavoritesDialog()
        {
            InitializeComponent();
        }

        private void PopulateFavoritesListView(List<Favorite> favorites)
        {
            foreach (Favorite favorite in favorites)
            {
                AddFavorite(favorite);
            }
        }

        private void AddFavorite(Favorite favorite)
        {
            ListViewItem listViewItemTemp = new ListViewItem(favorite.Name);
            listViewItemTemp.SubItems.Add(favorite.Path);

            this.FavoritesListView.Items.Add(listViewItemTemp);
        }

        private Favorite GetFavorite(ListViewItem listViewItem)
        {
            return new Favorite(listViewItem.Text, listViewItem.SubItems[1].Text);
        }

        private List<Favorite> GetFavorites()
        {
            List<Favorite> favorites = new List<Favorite>();
            foreach (ListViewItem listViewItem in this.FavoritesListView.Items)
            {
                favorites.Add(GetFavorite(listViewItem));
            }

            return favorites;
        } 

        private void UpdateFavorite(ListViewItem listViewItem, Favorite favorite)
        {
            listViewItem.Text = favorite.Name;
            listViewItem.SubItems[1].Text = favorite.Path;
        }

        #region Events
        private void Favorites_Load(object sender, EventArgs e)
        {
            //PopulateFavoritesListView();
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            if (this.FavoritesListView.SelectedItems.Count == 1 && this.FavoritesListView.SelectedItems[0].Index != this.FavoritesListView.Items.Count - 1)
            {
                this.FavoritesListView.BeginUpdate();
                ListViewItem listViewItem = this.FavoritesListView.SelectedItems[0];
                int index = listViewItem.Index;
                listViewItem.Remove();
                this.FavoritesListView.Items.Insert(++index, listViewItem);
                this.FavoritesListView.EndUpdate();
            }

            this.FavoritesListView.Select();
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            if (this.FavoritesListView.SelectedItems.Count == 1 && this.FavoritesListView.SelectedItems[0].Index != 0)
            {
                this.FavoritesListView.BeginUpdate();
                ListViewItem listViewItem = this.FavoritesListView.SelectedItems[0];
                int index = listViewItem.Index;
                listViewItem.Remove();
                this.FavoritesListView.Items.Insert(--index, listViewItem);
                this.FavoritesListView.EndUpdate();
            }

            this.FavoritesListView.Select();
        }
        #endregion Events

        private void AddButton_Click(object sender, EventArgs e)
        {
            FavoriteEditDialog fed = new FavoriteEditDialog();
            fed.Name = "Add Favorite";
            if (fed.ShowDialog() == DialogResult.OK)
            {
                AddFavorite(fed.Favorite);
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (this.FavoritesListView.SelectedItems.Count == 1)
            {
                this.FavoritesListView.SelectedItems[0].Remove();
            }
        }

        private void FavoritesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.FavoritesListView.SelectedItems.Count == 1)
            {
                ListViewItem listViewItem = this.FavoritesListView.SelectedItems[0];

                FavoriteEditDialog fed = new FavoriteEditDialog();
                fed.Name = "Edit Favorite";
                fed.Favorite = GetFavorite(listViewItem);
                if (fed.ShowDialog() == DialogResult.OK)
                {
                    UpdateFavorite(listViewItem, fed.Favorite);
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
