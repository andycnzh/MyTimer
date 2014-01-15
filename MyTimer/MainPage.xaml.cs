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
        Timer timer;
        int totalMinutes = 0;
        int remainingSeconds = 0;
        int showMinute = 0;
        int showSecond = 0;
        string msg;
        bool timerEnabled = false;
        bool timerStart = false;
        string defaultMinuterNum = "请输入一个整数";
        string defaultOutputBlock = "输入分钟数，系统开始倒计时，当倒计时结束，手机将会进行震动。在倒计时过程中，如果退出应用，则会停止倒计时。";

        // 调用手机的震动
        VibrationDevice timerVibrate = VibrationDevice.GetDefault();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            myStopWatch = new Stopwatch();
            timer = new Timer(UpdateTextBlock, DigitalCanvas, 0, 1000);

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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtMinuteNum.Text = defaultMinuterNum;
            txtOutputMsg.Text = defaultOutputBlock;
            txtMinute.Text = "00";
            txtSecond.Text = "00";
            btnStop.Content = "暂停";
            totalMinutes = 0;
            remainingSeconds = 0;

            myStopWatch.Reset();
            timerEnabled = false;
            timerStart = false;
            btnStop.IsEnabled = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            if (txtMinuteNum.Text.Trim() == "")
            {
                MessageBox.Show("请输入倒计时分钟数");
                return;
            }
            else
            {
                bool parse = Int32.TryParse(txtMinuteNum.Text.Trim(), out totalMinutes);
                if (!parse)
                {
                    totalMinutes = 0;
                    MessageBox.Show("请输入倒计时分钟数");
                    return;
                }
            }

            showMinute = totalMinutes;
            showSecond = 0;

            myStopWatch.Restart();

            timerEnabled = true;
            timerStart = true;

            btnStop.IsEnabled = true;
            btnStop.Content = "暂停";
            msg = "倒计时计时中......\n";
        }

        private void UpdateTextBlock(object state)
        {
            if (timerStart)
            {
                if (timerEnabled)
                {
                    Canvas tmp_DigitalCanvas = (Canvas)state;

                    long elapseSecond = myStopWatch.ElapsedMilliseconds / 1000;

                    remainingSeconds = (int)(totalMinutes * 60 - (int)elapseSecond);
                    showMinute = remainingSeconds / 60;
                    showSecond = remainingSeconds % 60;

                    //如果remainingMinture=-1，则显示0
                    showMinute = showMinute < 0 ? 0 : showMinute;

                    if (remainingSeconds == 30)
                    {
                        timerVibrate.Vibrate(TimeSpan.FromSeconds(1));
                        msg = "30s倒计时！\n";
                    }
                    else if (remainingSeconds == 0)
                    {
                        myStopWatch.Stop();
                        timerEnabled = false;
                        timerStart = false;
                        btnStop.IsEnabled = false;

                        timerVibrate.Vibrate(TimeSpan.FromSeconds(3));
                        
                        msg = "倒计时已完成！\n";
                    }

                    tmp_DigitalCanvas.Dispatcher.BeginInvoke(delegate()
                    {
                        txtMinute.Text = showMinute.ToString().PadLeft(2, '0');
                        txtSecond.Text = showSecond.ToString().PadLeft(2, '0');
                        txtOutputMsg.Text = msg;
                    });
                }
            }
        }

        private void txtMinuteNum_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtMinuteNum.Text.Trim() == defaultMinuterNum)
            {
                txtMinuteNum.Text = "";
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (timerStart)
            {
                if (btnStop.Content.ToString() == "暂停")
                {
                    myStopWatch.Stop();
                    timerEnabled = false;
                    btnStop.Content = "继续";
                }
                else if (btnStop.Content.ToString() == "继续")
                {
                    myStopWatch.Start();
                    timerEnabled = true;
                    btnStop.Content = "暂停";
                }
            }
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