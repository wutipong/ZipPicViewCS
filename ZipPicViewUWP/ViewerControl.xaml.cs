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
    public sealed partial class ViewerControl : UserControl
    {
        public ViewerControl()
        {
            this.InitializeComponent();
        }

        public String Filename
        {
            get { return filenameTextBlock.Text; }
            set { filenameTextBlock.Text = value; }
        }

        public event RoutedEventHandler NextButtonClick
        {
            add { nextButton.Click += value; }
            remove { nextButton.Click -= value; }
        }

        public event RoutedEventHandler PrevButtonClick
        {
            add { prevButton.Click += value; }
            remove { prevButton.Click -= value; }
        }

        public event RoutedEventHandler CloseButtonClick
        {
            add { closeButton.Click += value; }
            remove { closeButton.Click -= value; }
        }
    }
}
