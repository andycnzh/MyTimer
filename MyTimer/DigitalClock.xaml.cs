using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyTimer
{
    public partial class DigitalClock : PhoneApplicationPage
    {
        public DigitalClock()
        {
            InitializeComponent();

            //Dispay current time
            txtHour.Text = DateTime.Now.Hour.ToString();
            txtMinute.Text = DateTime.Now.Minute.ToString();
            txtSecond.Text = DateTime.Now.Second.ToString();

        }
    }
}