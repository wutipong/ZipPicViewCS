using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ZipPicViewUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FolderListItem : UserControl
    {
        public FolderListItem()
        {
            this.InitializeComponent();
        }

        public ImageSource ImageSource
        {
            set
            {
                if (value == null)
                {
                    image.Visibility = Visibility.Collapsed;
                    folderIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    image.Source = value;
                    image.Visibility = Visibility.Visible;
                    folderIcon.Visibility = Visibility.Collapsed;
                }
            }
        }

        public async void SetImageSourceAsync(ImageSource source)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ImageSource = source;
            });
        }

        public string Text
        {
            get { return name.Text; }
            set { name.Text = value; }
        }

        public string Value { get; set; }
    }
}