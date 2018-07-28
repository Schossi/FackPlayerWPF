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
using FackPlayerWpf;
using System.Diagnostics;

namespace FackPlayerWpf.Panels
{
    /// <summary>
    /// Interaktionslogik für MonitorPanel.xaml
    /// </summary>
    public partial class MonitorPanel : UserControl
    {
        public static bool debug = false;

        public MonitorPanel()
        {
            InitializeComponent();
        }

        public void Initialize(MonitorPos position)
        {
            foreach (System.Windows.Forms.Screen sc in System.Windows.Forms.Screen.AllScreens)
            {
                comboboxScreens.Items.Add(sc);
            }

            switch (position)
            {
                case MonitorPos.Left:
                    combobox_id.SelectedItem = comboItem_L;
                    break;
                /*case MonitorPos.Middle:
                    combobox_id.SelectedItem = comboItem_M;
                    break;*/
                case MonitorPos.Center:
                    combobox_id.SelectedItem = comboItem_C;
                    break;
                case MonitorPos.Right:
                    combobox_id.SelectedItem = comboItem_R;
                    break;
                default:
                    break;
            }

            textbox_folder.Text = getAppSetting("Folder_" + myID);
            myType = (PlayerType)int.Parse(getAppSetting("Type_" + myID));
            
            textbox_X.Text = getAppSetting("X_" + myID);
            textbox_Y.Text = getAppSetting("Y_" + myID);

            myPlayer = new Players.MoviePlayer();
        }

        public void Save()
        {
            setAppSetting("Folder_" + myID, textbox_folder.Text);
            setAppSetting("Type_" + myID, ((int)myType).ToString());
            
            setAppSetting("X_" + myID,textbox_X.Text);
            setAppSetting("Y_" + myID,textbox_Y.Text);
        }
        
        public void Start()
        {
            myPlayer.Close();
            myPlayer = new Players.MoviePlayer();

            if (Debugger.IsAttached && debug)
            {
                myPlayer.Height = 400;
                myPlayer.Width = 600;
            }

            myPlayer.Initialize(myType, textbox_folder.Text);
            
            myPlayer.Top = int.Parse(textbox_Y.Text);
            myPlayer.Left = int.Parse(textbox_X.Text);

            myPlayer.Show();
        }
        
        public Players.MoviePlayer myPlayer=new Players.MoviePlayer();
        public MonitorPos myPos
        {
            get
            {
                return ToMonPos(((ComboBoxItem)combobox_id.SelectedItem).Tag.ToString());
            }
        }
        public string myID
        {
            get
            {
                return ((int)myPos).ToString();
            }
        }
        public PlayerType myType
        {
            get
            {
                if (radiobutton_image.IsChecked.Value)
                    return PlayerType.Image;
                else if (radiobutton_video.IsChecked.Value)
                    return PlayerType.Video;
                return PlayerType.Video;
            }
            set
            {
                if (value == PlayerType.Image)
                {
                    radiobutton_image.IsChecked = true;
                }
                else if (value == PlayerType.Video)
                {
                    radiobutton_video.IsChecked = true;
                }
            }
        }
        
        public static MonitorPos ToMonPos(string monPos)
        {
            switch (monPos)
            {
                case "L":
                    return MonitorPos.Left;
                /*case "M":
                    return MonitorPos.Middle;*/
                case "C":
                    return MonitorPos.Center;
                case "R":
                    return MonitorPos.Right;
                default:
                    return MonitorPos.Center;
            }
        }

        private void button_folder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dia = new System.Windows.Forms.FolderBrowserDialog();
            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textbox_folder.Text = dia.SelectedPath;
            }
        }

        private void comboboxScreens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Forms.Screen screen=(System.Windows.Forms.Screen)comboboxScreens.SelectedItem;

            textbox_X.Text = screen.Bounds.X.ToString();
            textbox_Y.Text = screen.Bounds.Y.ToString();
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

        public void switchIdentify()
        {
            myPlayer.label_identity.Content = myPos.ToString();
            if (myPlayer.viewBox_identify.Visibility == System.Windows.Visibility.Collapsed)
                myPlayer.viewBox_identify.Visibility = System.Windows.Visibility.Visible;
            else
                myPlayer.viewBox_identify.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
