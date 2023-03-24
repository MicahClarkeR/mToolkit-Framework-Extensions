using mToolkitPlatformComponentLibrary;
using mToolkitPlatformComponentLibrary.Workspace.Files;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static mToolkitFrameworkExtensions.mWorkspaceEtx;
using static mToolkitFrameworkExtensions.mWorkspaceEtx.mTempWorkspaceEtx;
using static System.Net.Mime.MediaTypeNames;

namespace mToolkitFrameworkExtensions.Windows.Elements
{
    /// <summary>
    /// Interaction logic for ImageGridItem.xaml
    /// </summary>
    public partial class ImageGridItem : UserControl
    {
        public readonly Uri Uri;
        public readonly ImageGrid Owner;
        public new readonly string? Name;

        internal ImageGridItem(mTool tool, ImageGrid owner, Uri uri, string? name)
        {
            InitializeComponent();
            Name = name;
            Uri = uri;
            Owner = owner;
            try
            {
                if (mTempWorkspaceEtx.CreateTempFileCopy(tool, uri.OriginalString, out mTemporaryReferenceFileEtx? temp) && temp != null)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        ImageName.Content = name;
                        Grid.SetRowSpan(Image, 1);
                    }
                    SetImage(temp.File.Stream.GetMemoryStream());

                    temp.OnChange += new FileSystemEventHandler((s, e) =>
                    {
                        if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
                        {
                            SetImage(temp.File.Stream.GetMemoryStream());
                        }
                        else if(e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Renamed)
                        {
                            SetImage(new MemoryStream(Properties.Resources.Missing));
                        }
                    });
                }
            }
            catch (Exception ex)
            { }
        }

        private void SetImage(MemoryStream stream)
        {
            Image.Dispatcher.Invoke(new Action(() =>
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                Image.Source = image;
            }));
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Highlight.Visibility = Visibility.Visible;
            Owner.Selected = this;
        }

        public void Unfocus()
        {
            Highlight.Visibility = Visibility.Hidden;
        }
    }
}
