using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZipPicViewUWP
{
    public sealed partial class Thumbnail : UserControl
    {
        public Thumbnail()
        {
            this.InitializeComponent();
            button.Click += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Click(this, e);
        }

        public TextBlock Label
        {
            get
            {
                return label;
            }
        }

        public Image Image
        {
            get
            {
                return image;
            }
        }

        public ProgressRing ProgressRing
        {
            get { return loading; }
        }

        public event RoutedEventHandler Click;

        private string userData;
        public string UserData
        {
            get { return userData; }
            set { userData = value; }
        }
    }
}
