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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Configuration;
using System.Threading;

namespace FackPlayerWpf
{
    public enum Event { hoeglinger = 0, fackman = 1 } /*probst = 0, muss = 1, /*preise=2, orientation=3, jelly = 4, fackman = 5 *///, hoeglinger=6 }*/
    public enum MonitorPos { Left = 0,/* Middle = 1,*/ Center = 2, Right = 3 }
    public enum PlayerType {Video=0,Image=1 }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),new FrameworkPropertyMetadata { DefaultValue = 20 });

            Instance = this;

            Panels.MonitorPanel panel;
            foreach(MonitorPos pos in Enum.GetValues(typeof(MonitorPos)))
            {
                panel = new Panels.MonitorPanel();
                panel.Initialize(pos);
                stackPanel_monitors.Children.Add(panel);
            }

            Panels.EventPanel epanel;
            foreach (Event e in Enum.GetValues(typeof(Event)))
            {
                epanel = new Panels.EventPanel();
                epanel.Initialize(e);
                stackPanel_events.Children.Add(epanel);
                eventPanels.Add(epanel);
            }

            ThreadStart ts = new ThreadStart(EventLoop);
            Thread t = new Thread(ts);
            t.IsBackground = true;
            t.Start();
        }

        private List<Panels.EventPanel> eventPanels = new List<Panels.EventPanel>();

        private bool active = false;
        private void EventLoop()
        {
            //return;
            while (true)
            {
                try
                {
                    if (active && !eventGoingOn)
                    {
                        foreach (Panels.EventPanel panel in eventPanels)
                        {
                            if (eventGoingOn)
                                break;
                            panel.Dispatcher.Invoke(new Action(panel.tryPlay), null);
                        }
                    }
                    Thread.Sleep(500);
                }
                catch { }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.Start();
            }
            foreach (Panels.EventPanel panel in stackPanel_events.Children)
            {
                panel.reset();
            }

            active = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.myPlayer.PlayPause();
            }
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.Save();
            }
            foreach (Panels.EventPanel panel in stackPanel_events.Children)
            {
                panel.Save();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            active = false;
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.myPlayer.mediaPlayer_Main.Stop();
                panel.myPlayer.Close();
            }
        }


        private void button_identify_Click(object sender, RoutedEventArgs e)
        {
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.switchIdentify();
            }
        }        

        private void button_testEvent_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public bool eventGoingOn=false;

        public delegate void doPlayEvent(Event type,TimeSpan duration);
        public void PlayEvent(Event type,TimeSpan duration)
        {
            if(eventGoingOn)
                return;
            eventGoingOn=true;
            foreach (Panels.MonitorPanel panel in stackPanel_monitors.Children)
            {
                panel.myPlayer.mediaPlayer_event.Visibility = System.Windows.Visibility.Collapsed;

                switch (type)
                {
                    //case Event.preise:
                    //    if(panel.myPos== MonitorPos.Center)
                    //        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/MonitorTemplateFackMan.jpg", UriKind.Relative));
                    //    else
                    //        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Preisliste.jpg", UriKind.Relative));
                    //    break;
                    case Event.fackman:
                        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/MonitorTemplateFackMan.jpg", UriKind.Relative));
                        break;
                    case Event.hoeglinger:
                        switch (panel.myPos)
                        {
                            case MonitorPos.Left:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                            //case MonitorPos.Middle:
                            //    panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/Audi.jpg", UriKind.Relative));
                            //    break;
                            case MonitorPos.Center:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Right:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                        }
                        break;
                    /*case Event.segway:
                        switch (panel.myPos)
                        {
                            case MonitorPos.Left:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/segway/segway.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Middle:
                                panel.myPlayer.mediaPlayer_event.Source = new Uri("Videos/Film.wmv", UriKind.Relative);
                                panel.myPlayer.mediaPlayer_event.Visibility = System.Windows.Visibility.Visible;
                                panel.myPlayer.mediaPlayer_event.Play();
                                break;
                            case MonitorPos.Center:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/segway/segwayCenter.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Right:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/segway/segway.jpg", UriKind.Relative));
                                break;
                        }
                        break;
                    case Event.hpark:
                        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Einschaltung_sFack.jpg", UriKind.Relative));
                        break;
                    case Event.muss:
                        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/muss/logo_muss.png", UriKind.Relative));
                        break;
                    case Event.probst:
                        if (panel.myPos == MonitorPos.Center)
                        {
                            panel.myPlayer.image_event.Stretch = Stretch.Uniform;
                            panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/remax/Plakat Ackersberg.png", UriKind.Relative));
                        }
                        else
                        {
                            panel.myPlayer.mediaPlayer_event.Source = new Uri("Videos/REMAX(1).flv", UriKind.Relative);
                            panel.myPlayer.mediaPlayer_event.Visibility = System.Windows.Visibility.Visible;
                            panel.myPlayer.mediaPlayer_event.Play();
                        }
                        break;
                    /*case Event.preise:
                        if(panel.myPos== MonitorPos.Center)
                            panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/MonitorTemplateFackMan.jpg", UriKind.Relative));
                        else
                            panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Preisliste.jpg", UriKind.Relative));
                        break;
                    case Event.orientation:
                        switch (panel.myPos)
                        {
                            case MonitorPos.Left:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Orientation/6PackEvent.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Middle:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/MonitorTemplateFackMan.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Center:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Orientation/Special.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Right:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/Orientation/Ständer.jpg", UriKind.Relative));
                                break;
                        }
                        break;
                    case Event.fackman:
                        panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/MonitorTemplateFackMan.jpg", UriKind.Relative));
                        break;
                    case Event.jelly:
                        switch (panel.myPos)
                        {
                            case MonitorPos.Left:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/jelly/JellyErdbeerKirsch.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Middle:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/jelly/JellyKokosMaracuja.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Center:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/jelly/JellyCenter.jpg", UriKind.Relative));
                                
                                /*panel.myPlayer.mediaPlayer_event.Source = new Uri("Videos/Jelly.flv", UriKind.Relative);
                                panel.myPlayer.mediaPlayer_event.Visibility = System.Windows.Visibility.Visible;
                                panel.myPlayer.mediaPlayer_event.Play();
                                break;
                            case MonitorPos.Right:
                                //panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/jelly/JellyMiddle.jpg", UriKind.Relative));

                                panel.myPlayer.mediaPlayer_event.Source = new Uri("Videos/Jelly.flv", UriKind.Relative);
                                panel.myPlayer.mediaPlayer_event.Visibility = System.Windows.Visibility.Visible;
                                panel.myPlayer.mediaPlayer_event.Play();
                                break;
                        }
                        break;
                    /*case Event.hoeglinger:
                        switch (panel.myPos)
                        {
                            case MonitorPos.Left:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Middle:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/Audi.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Center:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                            case MonitorPos.Right:
                                panel.myPlayer.image_event.Source = new BitmapImage(new Uri("/FackPlayerWpf;component/Images/hoeglinger/logo.jpg", UriKind.Relative));
                                break;
                        }
                        break;*/
                }
                panel.myPlayer.EventTransition(duration);
            }
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {            
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.Space)
                {
                    Button_Click(sender, new RoutedEventArgs());
                }
                else
                {
                    int ev;
                    if (int.TryParse(e.Key.ToString().Replace("D", ""), out ev))
                    {
                        ev--;
                        if (ev < Enum.GetValues(typeof(Event)).Length)
                        {
                            Event type = (Event)ev;

                            foreach (Panels.EventPanel panel in stackPanel_events.Children)
                            {
                                if (panel.myType == type)
                                {
                                    PlayEvent(type, panel.myDuration);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (e.Key == Key.Space)
            {
                Button_Click_1(sender, new RoutedEventArgs());
            }
        }

    }
}
