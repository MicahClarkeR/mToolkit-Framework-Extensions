using mToolkitFrameworkExtensions.Windows.Elements;
using mToolkitPlatformComponentLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mToolkitFrameworkExtensions.Windows
{
    /// <summary>
    /// Interaction logic for ImageGrid.xaml
    /// </summary>
    public partial class ImageGrid : UserControl
    {
        public ObservableCollection<KeyValuePair<string, string>> Images = new ObservableCollection<KeyValuePair<string, string>>();
        public string Prefix = "";
        public float HeightPerRow = -1.0f;
        public ImageGridItem? Selected = null;
        public int ColumnCount
        {
            get
            {
                return ContentGrid.ColumnDefinitions.Count;
            }
            set
            {
                SetColumnCount(value);
            }
        }
        public mTool Tool;

        public ImageGrid()
        {
            InitializeComponent();
            Images.CollectionChanged += new NotifyCollectionChangedEventHandler(Images_CollectionChanged);

            ContextMenu menu = new ContextMenu();
            ContextMenuExt.CreateMenuItem(menu, "Add Column", (sender, args) => SetColumnCount(ContentGrid.ColumnDefinitions.Count + 1));
            ContextMenuExt.CreateMenuItem(menu, "Remove Column", (sender, args) => SetColumnCount(ContentGrid.ColumnDefinitions.Count - 1));
            ContextMenu = menu;
        }

        private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                List<KeyValuePair<string, string>> newImages = e.NewItems as List<KeyValuePair<string, string>> ?? new List<KeyValuePair<string, string>>();

                foreach(KeyValuePair<string, string> newImage in newImages)
                {
                    string path = newImage.Value;

                    if (!string.IsNullOrEmpty(Prefix))
                        path = $"{Prefix}{path}";

                    ImageGridItem image = new ImageGridItem(Tool, this, new Uri(path), newImage.Key);
                    ContentGrid.Children.Add(image);
                    Reorganise();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                List<string> newImages = e.NewItems as List<string> ?? new List<string>();

                foreach (string newImage in newImages)
                {
                    List<ImageGridItem> toRemove = new List<ImageGridItem>(ContentGrid.Children.Cast<ImageGridItem>().Where(el => (el.Uri == new Uri(newImage))));
                    
                    foreach(ImageGridItem image in toRemove)
                        ContentGrid.Children.Remove(image);

                    Reorganise();
                }
            }
        }

        public void SetColumnCount(int columns)
        {
            if (columns < 0) return;

            ContentGrid.ColumnDefinitions.Clear();

            for (int i = 1; i <= columns; i++)
            {
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(50, GridUnitType.Star)
                });
            }

            Reorganise();
        }

        public void Reset()
        {
            ContentGrid.Children.Clear();
            foreach (KeyValuePair<string, string> newImage in Images)
            {
                string path = newImage.Value;

                if (!string.IsNullOrEmpty(Prefix))
                    path = $"{Prefix}{path}";

                ImageGridItem image = new ImageGridItem(Tool, this, new Uri(path), newImage.Key);
                ContentGrid.Children.Add(image);
            }
            Reorganise();
        }

        private void Reorganise()
        {
            int count = (int) Math.Ceiling((float)(ContentGrid.Children.Count) / (float)(ContentGrid.ColumnDefinitions.Count));

            ContentGrid.RowDefinitions.Clear();

            for (int j = 0; j < count; j++)
            {
                RowDefinition row = new RowDefinition();

                if (HeightPerRow != -1)
                    row.Height = new GridLength(HeightPerRow, GridUnitType.Pixel);

                ContentGrid.RowDefinitions.Add(row);
            }

            int i = 0;
            foreach (ImageGridItem image in ContentGrid.Children)
            {
                Grid.SetColumn(image, (i % ContentGrid.ColumnDefinitions.Count));
                Grid.SetRow(image, ((i / ContentGrid.ColumnDefinitions.Count) % ContentGrid.RowDefinitions.Count));
                i++;
            }
        }

        private void ContentGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (ImageGridItem image in ContentGrid.Children)
                image.Unfocus();
        }
    }
}
