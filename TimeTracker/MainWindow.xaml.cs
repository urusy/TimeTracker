using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TimeTracker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Statictics> list;
        private DispatcherTimer timer;
        private Statictics statictics;
        private long secondCounter;
        
        public MainWindow()
        {
            InitializeComponent();

            list = new ObservableCollection<Statictics>();
            this.StaticticsListView.DataContext = list;

            this.StartButton.IsEnabled = true;
            this.FinishButton.IsEnabled = false;
            this.DeleteButton.IsEnabled = false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button.Content.Equals("Start"))
            {
                if (this.statictics == null)
                {
                    this.statictics = new Statictics();
                    this.statictics.Start = DateTime.Now;

                    this.timer = new DispatcherTimer(DispatcherPriority.Normal);
                    this.timer.Interval = new TimeSpan(0, 0, 1);
                    this.timer.Tick += new EventHandler(this.DispatcherTimer_Tick);
                    this.timer.Start();
                }
                else
                {
                    this.timer.Start();
                }
                button.Content = "Pause";

                this.TitleTextBox.IsEnabled = false;

                this.FinishButton.IsEnabled = true;
            }
            else
            {
                this.timer.Stop();
                
                button.Content = "Start";

                this.TitleTextBox.IsEnabled = true;

                this.FinishButton.IsEnabled = true;
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            this.statictics.Finish = DateTime.Now;
            //this.statictics.Time = (this.statictics.Finish.Ticks - this.statictics.Start.Ticks) / TimeSpan.TicksPerSecond;
            this.statictics.Time = this.secondCounter;
            this.statictics.Title = this.TitleTextBox.Text.Equals(String.Empty) ? "No title" : this.TitleTextBox.Text;

            this.timer.Stop();

            this.TitleTextBox.IsEnabled = true;

            this.StartButton.IsEnabled = true;
            this.FinishButton.IsEnabled = false;

            this.list.Add(this.statictics);

            this.secondCounter = 0;
            TimeSpan ts = new TimeSpan(0, 0, (int)this.secondCounter);
            this.TimeLabel.Content = ts.ToString();
            this.StartButton.Content = "Start";
            this.statictics = null;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.secondCounter++;
            //TimeSpan ts = new TimeSpan(0, 0, (int)((DateTime.Now.Ticks - this.statictics.Start.Ticks) / TimeSpan.TicksPerSecond));
            TimeSpan ts = new TimeSpan(0, 0, (int)this.secondCounter);

            this.TimeLabel.Content = ts.ToString();
        }
    }

    public class Statictics
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public long Time { get; set; }
        public string TimeDisp
        {
            get
            {
                TimeSpan ts = new TimeSpan(0, 0, (int)this.Time);
                return ts.ToString();
            }
        }
    }
}
