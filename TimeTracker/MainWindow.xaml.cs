using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

            this.list = new ObservableCollection<Statictics>(common.Sqlite.selectStaticticsAll());
            this.StaticticsListView.DataContext = list;
            //this.list = this.StaticticsListView.DataContext;

            this.StartButton.IsEnabled = true;
            this.FinishButton.IsEnabled = false;
            this.DeleteButton.IsEnabled = false;

            var context = DispatcherSynchronizationContext.Current;

            Task.Factory.StartNew(() =>
            {
                common.Sqlite.setupDatabase();
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            // タイマーを動かす
            if (button.Content.Equals("Start"))
            {
                // 新規に開始する場合
                if (this.statictics == null)
                {
                    this.statictics = new Statictics();
                    this.statictics.Id = Guid.NewGuid().ToString();
                    this.statictics.Start = DateTime.Now;

                    this.timer = new DispatcherTimer(DispatcherPriority.Normal);
                    this.timer.Interval = new TimeSpan(0, 0, 1);
                    this.timer.Tick += new EventHandler(this.DispatcherTimer_Tick);
                    this.timer.Start();
                }
                // 一時停止を解除する場合
                else
                {
                    this.timer.Start();
                }
                button.Content = "Pause";

                this.TitleTextBox.IsEnabled = false;

                this.FinishButton.IsEnabled = true;
            }
            // 一時停止する場合
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
            this.statictics.Time = this.secondCounter;
            this.statictics.Title = this.TitleTextBox.Text.Equals(String.Empty) ? "No title" : this.TitleTextBox.Text;

            this.timer.Stop();

            this.TitleTextBox.IsEnabled = true;

            this.StartButton.IsEnabled = true;
            this.FinishButton.IsEnabled = false;

            int i = 0;
            bool hitFlg = false;
            foreach (Statictics item in this.list)
            {
                if (item.Id.Equals(this.statictics.Id))
                {
                    this.list[i] = this.statictics;
                    hitFlg = true;
                    this.StaticticsListView.Items.Refresh();
                    break;
                }
                i++;
            }

            // 該当する行がない場合、追加
            if (!hitFlg)
            {
                this.list.Add(this.statictics);
                // DBに登録
                common.Sqlite.insertStatictics(this.statictics);
            }

            this.secondCounter = 0;
            TimeSpan ts = new TimeSpan(0, 0, (int)this.secondCounter);
            this.TimeLabel.Content = ts.ToString();
            this.StartButton.Content = "Start";
            this.statictics = null;

            this.TitleTextBox.Text = String.Empty;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.secondCounter++;
            TimeSpan ts = new TimeSpan(0, 0, (int)this.secondCounter);

            this.TimeLabel.Content = ts.ToString();
        }
        
        private void StaticticsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            object obj = item.Content;

            this.statictics = (Statictics)obj;

            this.TitleTextBox.Text = statictics.Title;
            this.secondCounter = statictics.Time;
            TimeSpan ts = new TimeSpan(0, 0, (int)this.secondCounter);
            this.TimeLabel.Content = ts.ToString();
            this.StartButton.Content = "Start";
            this.TitleTextBox.IsEnabled = true;
            this.FinishButton.IsEnabled = true;

            // タイマーを設定
            this.timer = new DispatcherTimer(DispatcherPriority.Normal);
            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        }
    }

    public class Statictics
    {
        public string Id { get; set; }
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
