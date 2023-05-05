using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
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

namespace StudyTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long start;
        private Process[] process;
        SoundPlayer detectedPlayer = new SoundPlayer(),killPlayer = new SoundPlayer();
        bool played=false;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext= this;

            DispatcherTimer timer = new DispatcherTimer();
            start = Environment.TickCount;
            timer.Tick += Timer_Tick;
            timer.Tick += Timer_Monitoring;
            timer.IsEnabled = true;

            detectedPlayer.SoundLocation = "C:\\Windows\\Media\\Windows Notify Email.wav";
            killPlayer.SoundLocation = "C:\\Windows\\Media\\Windows Notify Messaging.wav";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (DateTime.Now.Second % 2 == 0) { TimeText.Text = DateTime.Now.ToString("yyyy年MM月dd日\nHH mm ss ff");}
            else { TimeText.Text = DateTime.Now.ToString("yyyy年MM月dd日\nHH:mm:ss:ff"); }
        }

        private void Timer_Monitoring(object? sender, EventArgs e)
        {
            if (DateTime.Now.Second % 2 == 0)
            {
                try
                {
                    process = Process.GetProcessesByName("rtcRemoteDesktop");
                    if (process.Length != 0) { CameraWarning.Text = "检测到摄像头监视进程,Pid="+process[0].Id; BtnText.Text = "杀死进程";KillBtn.Visibility = Visibility.Visible;if (!played) { detectedPlayer.Play(); played = true; } }
                    else { CameraWarning.Text = " "; BtnText.Text = " ";KillBtn.Visibility = Visibility.Hidden;played = false; }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void KillBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(Process currentProcess in process)
                {
                    currentProcess.Kill();
                    ErrorText.Text = " ";

                }
                killPlayer.Play();
            }
            catch(Exception ex)
            {
                ErrorText.Text = "杀死进程失败，错误代码:" + ex.Message.ToString();
            }
        }
    }
}
