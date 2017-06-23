using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZipPicViewUWP
{
    public sealed partial class ViewerControl : UserControl
    {
        private static readonly TimeSpan[] advanceDurations = 
            {
                new TimeSpan(0, 0, 1),
                new TimeSpan(0, 0, 5),
                new TimeSpan(0, 0, 10),
                new TimeSpan(0, 0, 15),
                new TimeSpan(0, 0, 30),
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 2, 30),
                new TimeSpan(0, 5, 0),
                new TimeSpan(0, 10, 0),
                new TimeSpan(0, 15, 0),
                new TimeSpan(0, 30, 0),
                new TimeSpan(1, 0, 0)
            };
        private DispatcherTimer timer;

        private int counter;

        public delegate void PreCountEvent(object sender);
        private PreCountEvent onPreCount;

        public delegate void AutoAdvanceEvent(object sender);
        private AutoAdvanceEvent onAutoAdvance;

        private RoutedEventHandler nextButtonClick;

        public event PreCountEvent OnPreCount
        {
            add { onPreCount += value; }
            remove { onPreCount -= value; }
        }

        public event AutoAdvanceEvent OnAutoAdvance
        {
            add { onAutoAdvance += value; }
            remove { onAutoAdvance -= value; }
        }

        public ViewerControl()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1); 

            nextButton.Click += NextButton_Click;

            durationList.Items.Clear();
            var oneMinute = TimeSpan.FromMinutes(1.00);
            foreach (var duration in advanceDurations)
            {
                var durationStr = duration < oneMinute ?
                    String.Format("{0} Second(s)", duration.Seconds) :
                    String.Format("{0}:{1:00} Minute(s)", (int)duration.TotalMinutes, duration.Seconds);

                durationList.Items.Add(durationStr);
            }

            durationList.SelectedIndex = 0;

            if (!Windows.Graphics.Printing.PrintManager.IsSupported())
            {
                printBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            nextButtonClick(this, e);
        }

        private void Timer_Tick(object sender, object e)
        {
            counter--;
            if(counter < 5 && counter > 0 && precountToggle.IsOn) { onPreCount?.Invoke(this); }
            if(counter == 0)
            {
                onAutoAdvance?.Invoke(this);
                nextButtonClick?.Invoke(this, null);
                ResetCounter();
            }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                filenameTextBlock.Text = filename.Ellipses(70);
            }
        }

        public event RoutedEventHandler NextButtonClick
        {
            add { nextButtonClick += value; }
            remove { nextButtonClick -= value; }
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

        public event RoutedEventHandler SaveButtonClick
        {
            add { saveBtn.Click += value; }
            remove { saveBtn.Click -= value; }
        }

        public event RoutedEventHandler PrintButtonClick
        {
            add { printBtn.Click += value; }
            remove { printBtn.Click -= value; }
        }

        public bool? AutoEnabled
        {
            get { return autoBtn.IsChecked; }
            set { autoBtn.IsChecked = value; }
        }

        private void autoBtn_Checked(object sender, RoutedEventArgs e)
        {
            autoBtn.Content = new SymbolIcon(Symbol.Pause);
            autoDurationBtn.IsEnabled = false;
            timer.Start();
            saveBtn.IsEnabled = false;
            ResetCounter();
        }

        public void ResetCounter()
        {
            counter = (int)advanceDurations[durationList.SelectedIndex].TotalSeconds;
        }

        private void autoBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            autoBtn.Content = new SymbolIcon(Symbol.Play);
            autoDurationBtn.IsEnabled = true;
            timer.Stop();
            saveBtn.IsEnabled = true;
        }
    }
}
