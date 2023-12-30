using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NoMoreSeewo
{
    /// <summary>
    /// EEW.xaml 的交互逻辑
    /// </summary>
    public partial class EEW : MetroWindow
    {
        SoundPlayer playerFound = new SoundPlayer();
        public EEW()
        {
            InitializeComponent();

            playerFound.SoundLocation = ".\\eew.wav";
            playerFound.Load();
            playerFound.Play();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
