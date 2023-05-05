using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyTimer
{
    internal class Showing : INotifyPropertyChanged
    {
        int hour, minute, second, ms;
        public event PropertyChangedEventHandler PropertyChanged;

        public string TimeText
        {
            get
            {
                //ms = DateTime.Now.Millisecond;
                //second = DateTime.Now.Second;
                //minute = DateTime.Now.Minute;
                //hour = DateTime.Now.Hour;
                return DateTime.Now.ToLongTimeString();
                //PropertyChanged(this,new PropertyChangedEventArgs("TimeText"));
            }
        }
    }
}
