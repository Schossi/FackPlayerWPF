using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FackPlayerWpf.Players
{
    /// <summary>
    /// Interaktionslogik für MoviePlayer.xaml
    /// </summary>
    public partial class MoviePlayer : Window
    {
        public MoviePlayer()
        {
            InitializeComponent();

            myEventScale = new ScaleTransform(0, 0);
        }

        private ScaleTransform myEventScale;
        private DoubleAnimation eventScaleXAnimation = new DoubleAnimation(0, 5, new Duration(new TimeSpan(0, 0, 1)));
        private DoubleAnimation eventScaleYAnimation = new DoubleAnimation(0, 5, new Duration(new TimeSpan(0, 0, 1)));

        private PlayerType myType;
        private string[] myMedia;

        private bool imageBit = false;
        private DoubleAnimation fadeOut = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 1)));
        private DoubleAnimation fadeIn = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 1)));
        private DispatcherTimer imageTimer;

        public void Initialize(PlayerType type, string folder)
        {
            image_EventFack.RenderTransformOrigin = new Point(0.5, 0.5);

            myType = type;
            
            myMedia = Directory.GetFiles(folder);

            listBox_playlist.ItemsSource = myMedia;

            if (myType == PlayerType.Image)
            {
                imageTimer = new DispatcherTimer(new TimeSpan(0, 0, 20), DispatcherPriority.Background, new EventHandler(imageTimerTick), this.Dispatcher);
                imageTimer.Start();
            }
            else if (myType == PlayerType.Video)
            {
                //if (!Panels.MonitorPanel.debug)
                    mediaPlayer_Main.IsMuted = true;
            }

            PlayNext();
        }
        
        private void imageTimerTick(object sender, EventArgs e)
        {
            PlayNext();
        }
        
        private bool paused = false;

        public void PlayPause()
        {
            if (myType == PlayerType.Image)
            {
                if (paused)
                    imageTimer.Stop();
                else
                    imageTimer.Start();
            }
            else if (myType == PlayerType.Video)
            {
                if (paused)
                    mediaPlayer_Main.Play();
                else
                    mediaPlayer_Main.Pause();
            }

            paused = !paused;
        }

        public void EventTransition(TimeSpan duration)
        {
            DoubleAnimation endAnim1 = (DoubleAnimation)TryFindResource("e1");
            DoubleAnimation endAnim2 = (DoubleAnimation)TryFindResource("e2");
            endAnim1.BeginTime = duration;
            endAnim2.BeginTime = duration.Add(new TimeSpan(0, 0, 1));

            mediaPlayer_Main.Pause();
            Storyboard board = (Storyboard)TryFindResource("EventTransition");
            board.Completed += new EventHandler(board_Completed);
            if (board != null)
                board.Begin(this);
        }

        void board_Completed(object sender, EventArgs e)
        {
            mediaPlayer_event.Stop();
            image_event.Stretch = Stretch.Fill;
            mediaPlayer_Main.Play();
            MainWindow.Instance.eventGoingOn = false;
        }

        private void PlayNext()
        {
            bool doneIt = false;
            while (!doneIt)
            {
                try
                {
                    if (myType == PlayerType.Image)
                    {
                        if (imageBit)
                        {
                            image_1.Source = new BitmapImage(getRandom());
                            image_1.BeginAnimation(Image.OpacityProperty, fadeIn);
                            image_2.BeginAnimation(Image.OpacityProperty, fadeOut);
                            doneIt = true;
                        }
                        else
                        {
                            image_2.Source = new BitmapImage(getRandom());
                            image_2.BeginAnimation(Image.OpacityProperty, fadeIn);
                            image_1.BeginAnimation(Image.OpacityProperty, fadeOut);
                            doneIt = true;
                        }
                    }
                    else if (myType == PlayerType.Video)
                    {
                        mediaPlayer_Main.Source = getRandom();
                        mediaPlayer_Main.Play();
                        doneIt = true;
                    }
                }
                catch { }
            }
        }

        private Random rand = new Random(DateTime.Now.Millisecond);
        private string overrideRandomOnce = "";

        private Uri getRandom()
        {
            if (overrideRandomOnce != "")
            {
                label_current.Content = overrideRandomOnce;
                string temp = overrideRandomOnce;
                overrideRandomOnce = "";
                return new Uri(temp, UriKind.RelativeOrAbsolute);
            }
            string newmedia = myMedia[rand.Next(0, myMedia.Length - 1)];
            label_current.Content = newmedia;
            return new Uri((newmedia), UriKind.RelativeOrAbsolute);
        }

        private void mediaPlayer_Main_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            PlayNext();
        }

        private void mediaPlayer_Main_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            MainWindow.Instance.Window_KeyDown(sender, e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (grid_playlist.Visibility == System.Windows.Visibility.Visible)
                    grid_playlist.Visibility = System.Windows.Visibility.Collapsed;
                else
                    grid_playlist.Visibility = System.Windows.Visibility.Visible;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                MainWindow.Instance.Top = System.Windows.Forms.Control.MousePosition.Y;
                MainWindow.Instance.Left = System.Windows.Forms.Control.MousePosition.X;
                MainWindow.Instance.Focus();
            }
        }

        private void listBox_playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            overrideRandomOnce = listBox_playlist.SelectedItem.ToString();
            PlayNext();
        }
    }
}
