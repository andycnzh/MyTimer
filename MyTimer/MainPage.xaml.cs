using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyTimer.Resources;
using System.Diagnostics;
using System.Threading;
using Windows.Phone.Devices.Notification;
using System.Windows.Threading;

namespace MyTimer
{
    public partial class MainPage : PhoneApplicationPage
    {

        // 使用stopwatch来计算已逝去的时间
        // 使用timer来循环判断是否已经到达设定时间
        // 如果达到，停止stopwatch，给出用户提醒
        // 如果未到达，继续
        // 如果用户中途暂停，则暂停计时
        // 如果用户停止计时，则停止计时

        Stopwatch myStopWatch;
        DispatcherTimer updateUITimer;

        int showHour = 0;
        int showMinute = 0;
        int showSecond = 0;

        int totalSeconds = 0;
        int alarmSeconds = 0;
        int remainingSeconds = 0;
        string msg;

        bool timerStart = false;

        string defaultOutputBlock = "设置计时时间，应用开始倒计时，当倒计时结束，手机将会进行3秒震动，可以设置提醒时间，应用会在指定时刻震动1秒进行提醒。在倒计时过程中，如果退出应用，则会停止倒计时。";

        // 调用手机的震动
        VibrationDevice timerVibrate = VibrationDevice.GetDefault();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            myStopWatch = new Stopwatch();

            updateUITimer = new DispatcherTimer();
            updateUITimer.Interval = TimeSpan.FromSeconds(1);
            updateUITimer.Tick += OnTimerTick;

            // 禁用检测用户空闲
            PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
            phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            if (!timerStart)
            {
                btnStop.IsEnabled = false;
            }
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan settingTimeSpan = (TimeSpan)settingTimer.Value;

            totalSeconds = settingTimeSpan.Hours * 60 * 60 + settingTimeSpan.Minutes * 60 + settingTimeSpan.Seconds;

            TimeSpan alarmTimeSpan = (TimeSpan)alarmTimer.Value;

            alarmSeconds = alarmTimeSpan.Minutes * 60 + alarmTimeSpan.Seconds;

            if (totalSeconds < 1)
            {
                MessageBox.Show("请设置计时时间，必须大于0秒");
                return;
            }

            showHour = settingTimeSpan.Hours;
            showMinute = settingTimeSpan.Minutes;
            showSecond = settingTimeSpan.Seconds;

            myStopWatch.Restart();

            timerStart = true;
            btnStop.IsEnabled = true;
            btnStop.Content = "暂停";
            msg = "倒计时计时中......\n";

            updateUITimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine("Enter OnTimerTick Event******");

            if (true)
            {
                txtOutputMsg.Text = DateTime.Now.ToString();

                long elapseSecond = myStopWatch.ElapsedMilliseconds / 1000;

                remainingSeconds = (int)(totalSeconds - (int)elapseSecond);
             
                Debug.WriteLine("remainingSeconds is {0}", remainingSeconds.ToString());

                if (remainingSeconds >= 0)
                {
                    showHour = remainingSeconds / 3600;
                    showMinute = remainingSeconds / 60;
                    showSecond = remainingSeconds % 60;

                    Debug.WriteLine("调整前");
                    Debug.WriteLine("showHour is {0}", showHour.ToString());
                    Debug.WriteLine("showMinute is {0}", showMinute.ToString());
                    Debug.WriteLine("showSecond is {0}", showSecond.ToString());


                    ////如果remainingMinture=-1，则显示
                    //showHour = showHour < 0 ? 0 : showHour;
                    //showMinute = showMinute < 0 ? 0 : showMinute;
                    //showSecond = showSecond < 0 ? 0 : showSecond;

                    //Debug.WriteLine("调整后");
                    //Debug.WriteLine("showHour is {0}", showHour.ToString());
                    //Debug.WriteLine("showMinute is {0}", showMinute.ToString());
                    //Debug.WriteLine("showSecond is {0}", showSecond.ToString());

                    if (remainingSeconds == alarmSeconds)
                    {
                        timerVibrate.Vibrate(TimeSpan.FromSeconds(1));
                        msg = "提醒时间倒计时！\n";
                    }
                    else if (remainingSeconds == 0)
                    {
                        Debug.WriteLine("开始remaingSec为0的处理");

                        // timerEnabled = false;
                        timerStart = false;
                        btnStop.IsEnabled = false;

                        Debug.WriteLine("开始最后的震动");

                        timerVibrate.Vibrate(TimeSpan.FromSeconds(3));

                        Debug.WriteLine("完成最后的震动");

                        msg = "倒计时已完成！\n";
                        Debug.WriteLine("结束remaingSec为0的处理");
                        
                        myStopWatch.Stop();

                        updateUITimer.Stop();
                        
                        Debug.WriteLine("计时器停止成功");
                    }

                    txtHour.Text = showHour.ToString().PadLeft(2, '0');
                    txtMinute.Text = showMinute.ToString().PadLeft(2, '0');
                    txtSecond.Text = showSecond.ToString().PadLeft(2, '0');
                    txtOutputMsg.Text = msg;
                }
                else
                {
 
                }

            }
            Debug.WriteLine("Leave OnTimerTick Event======");
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (timerStart)
            {
                if (btnStop.Content.ToString() == "暂停")
                {
                    updateUITimer.Stop();
                    myStopWatch.Stop();
                    //timerEnabled = false;
               
                    btnStop.Content = "继续";
                }
                else if (btnStop.Content.ToString() == "继续")
                {
                    updateUITimer.Start();
                    myStopWatch.Start();
                    //timerEnabled = true;
              
                    btnStop.Content = "暂停";
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtOutputMsg.Text = defaultOutputBlock;
            txtHour.Text = "00";
            txtMinute.Text = "00";
            txtSecond.Text = "00";
            btnStop.Content = "暂停";

            totalSeconds = 0;
            alarmSeconds = 0;
            remainingSeconds = 0;

            myStopWatch.Reset();

            //timerEnabled = false;
            timerStart = false;
            btnStop.IsEnabled = false;

            updateUITimer.Stop();
        }
        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}