using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Web.UI.WebControls;
using System.Windows.Markup;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.Win32;
using Windows.UI.Notifications;

namespace NoMoreSeewo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        bool played = false, foundProcess = false, _foundProcess = false;

        SoundPlayer playerFound = new SoundPlayer(), playerMissing = new SoundPlayer();

        public float crdX = 0, crdY = 0;

        WebClient webClient = new(); InternetActions ia = new InternetActions();

        eewData _eewData = new eewData();

        ToastContentBuilder builder = new ToastContentBuilder();

        public MainWindow()
        {
            playerFound.SoundLocation = "C:\\Windows\\Media\\Windows Proximity Notification.wav";
            playerMissing.SoundLocation = "C:\\Windows\\Media\\Windows Notify Messaging.wav";
            playerFound.Load();
            playerMissing.Load();

            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.BorderBrush = Brushes.Transparent;
            this.ShowInTaskbar = false;

            crdX = 0.0f;crdY = 0.0f;    //将此处修改为你所在地的经纬度,南半球纬度为负


            _eewData = ia.GetEEW_FailBack();

            this.Left = SystemParameters.WorkArea.Right / 2 - 25;
            this.Top = 0;

            builder.Show();


                DispatcherTimer timer = new DispatcherTimer();

                timer.Tick += FindProcess;
                timer.Tick += Notify;
                timer.Tick += ListenEEW;
                timer.Start();

        }

        void FindProcess(object? sender, EventArgs e)
        {
            Process[] ps = Process.GetProcessesByName("rtcRemoteDesktop");
            foundProcess = ps.Length > 0;
        }

        void Notify(object? sender, EventArgs e)
        {
            if(foundProcess && !_foundProcess && !played)
            {
                this.ColorGrid.Background = Brushes.Red;
                playerFound.Play();

                played = true;
            }
            else if(!foundProcess && _foundProcess && played)
            {
                this.ColorGrid.Background = Brushes.Transparent;
                playerMissing.Play();

                played = false;
            }
            _foundProcess = foundProcess;
        }

        void ListenEEW(object? sender, EventArgs e)
        {

            eewData eewData = new();


                if(System.DateTime.Now.Millisecond % 50 == 0)
                {
                try
                {
                    eewData = ia.GetEEW_FailBack();


                    this.LocalInt.Text = eewData.ID.ToString();

                    float distance = GetDistance(eewData.x, eewData.y, crdX, crdY);
                    float localInt = (float)(0.92 + 1.63 * eewData.Magunitude - 3.49 * Math.Log10(distance));

                    if (eewData.OriginTime != _eewData.OriginTime)
                    {
                        if (localInt > 2 && localInt <= 12)
                        {
                            EEW eewPopup = new EEW();
                            eewPopup.TimeText.Text = eewData.OriginTime;
                            eewPopup.LocationText.Text = eewData.HypoCenter;
                            eewPopup.Level.Text = "震级" + eewData.Magunitude + "\n本地烈度" + (int)localInt;

                            eewPopup.Show();
                        }
                        else
                        {
                            new ToastContentBuilder().AddAttributionText(eewData.OriginTime + "于" + eewData.HypoCenter + "发生" + eewData.Magunitude + "级地震").Show();
                        }
                    }

                    _eewData=eewData;
                }
                catch(Exception ex) 
                {

                    MessageBox.Show(ex.Message);
                }
                }
        }

        
        public float GetDistance(float? x1, float? y1, float? x2, float? y2)
        {
            float a = rad(x1) - rad(x2);
            float b = rad(y1) - rad(y2);
            float s = (float)(2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(rad(x1)) * Math.Cos(rad(x2)) * Math.Pow(Math.Sin(b / 2), 2))));

            s = s * EARTH_RADIUS;
            s = (float)Math.Round(s * 10000) / 10000;

            return s;
        }

        float EARTH_RADIUS = 6378.137f;
        float rad(float? d)
        {
            return (float)(d * Math.PI / 180.0);
        }


    }
}
