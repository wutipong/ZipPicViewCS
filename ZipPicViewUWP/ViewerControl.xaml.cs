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
        private static readonly TimeSpan[] advanceDurations = { new TimeSpan(0,0,5), new TimeSpan(0, 0, 10), new TimeSpan(0, 0, 15), new TimeSpan(0, 0, 30) };
        private int counter = 0;
        private DispatcherTimer timer;

        public DispatcherTimer Timer
        {
            get { return timer; }
        }

        public ViewerControl()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
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

        public bool? AutoEnabled
        {
            get { return autoBtn.IsChecked; }
            set { autoBtn.IsChecked = value; }
        }

        private void autoBtn_Checked(object sender, RoutedEventArgs e)
        {
            autoDurationBtn.IsEnabled = false;
            timer.Interval =  advanceDurations[durationList.SelectedIndex];
            timer.Start();
        }

        private void autoBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            autoDurationBtn.IsEnabled = true;
            timer.Stop();
        }
    }
}
