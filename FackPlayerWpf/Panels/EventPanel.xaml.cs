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
using System.Configuration;

namespace FackPlayerWpf.Panels
{
    /// <summary>
    /// Interaktionslogik für EventPanel.xaml
    /// </summary>
    public partial class EventPanel : UserControl
    {
        public EventPanel()
        {
            InitializeComponent();
        }

        public Event myType;

        public void Initialize(Event type)
        {
            myType = type;
            
            textbox_duration.Text = getAppSetting("Duration_" + myID);
            textbox_interval.Text = getAppSetting("Interval_" + myID);

            label_type.Content = ((int)myType+1).ToString() + "-" + type.ToString();
        }

        public void Save()
        {
            setAppSetting("Duration_" + myID,textbox_duration.Text);
            setAppSetting("Interval_" + myID,textbox_interval.Text);
        }

        public string myID
        {
            get
            {
                return ((int)myType).ToString();
            }
        }

        public TimeSpan myDuration
        {
            get
            {
                return new TimeSpan(0, 0, int.Parse(textbox_duration.Text));
            }
        }
        public TimeSpan myInterval
        {
            get
            {
                return new TimeSpan(0, int.Parse(textbox_interval.Text), 0);
            }
        }

        private DateTime nextShowtime = DateTime.Now;
        public void reset()
        {
            nextShowtime = DateTime.Now + myInterval;
        }
        public void tryPlay()
        {
            if (DateTime.Now > nextShowtime)
            {
                reset();
                MainWindow.Instance.Dispatcher.Invoke(new MainWindow.doPlayEvent(MainWindow.Instance.PlayEvent), new object[] { myType, myDuration });
            }
        }

        public static string getAppSetting(string key)
        {
            //Laden der AppSettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Zurückgeben der dem Key zugehörigen Value
            if (config.AppSettings.Settings[key] != null)
                return config.AppSettings.Settings[key].Value;
            return "";
        }

        public static void setAppSetting(string key, string value)
        {
            //Laden der AppSettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Überprüfen ob Key existiert
            if (config.AppSettings.Settings[key] != null)
            {
                //Key existiert. Löschen des Keys zum "überschreiben"
                config.AppSettings.Settings.Remove(key);
            }
            //Anlegen eines neuen KeyValue-Paars
            config.AppSettings.Settings.Add(key, value);
            //Speichern der aktualisierten AppSettings
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void button_play_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.PlayEvent(myType, myDuration);
        }
    }
}
