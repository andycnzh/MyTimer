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

        Stopwatch stopWatch;
        Timer timer;
        int interval = 0;
        string msg;
        bool enabledTimer = false;
        string defaultMinuterNum = "请输入一个整数";
        string defaultOutputBlock = "输入分钟数，系统开始倒计时，当倒计时结束，手机将会进行震动。在倒计时过程中，如果退出应用，则会停止倒计时。";

        // 调用手机的震动
        VibrationDevice timerVibrate = VibrationDevice.GetDefault();

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            stopWatch = new Stopwatch();
            timer = new Timer(UpdateTextBlock, outputBlock, 0, 1000);

            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            minutenumBox.Text = defaultMinuterNum;
            outputBlock.Text = defaultOutputBlock;

            stopWatch.Reset();
            enabledTimer = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            if (minutenumBox.Text.Trim() == "")
            {
                MessageBox.Show("请输入倒计时分钟数");
                return;
            }
            else
            {
                bool parse = Int32.TryParse(minutenumBox.Text.Trim(), out interval);
                if (!parse)
                {
                    interval = 0;
                    MessageBox.Show("请输入倒计时分钟数");
                    return;
                }
            }

            stopWatch.Restart();
            enabledTimer = true;
            outputBlock.Text = "倒计时开始\n";
        }

        private void UpdateTextBlock(object state)
        {
            if (enabledTimer)
            {
                TextBlock tmp_output_Block = (TextBlock)state;

                long elapseSecond = stopWatch.ElapsedMilliseconds / 1000;

                if (elapseSecond == (long)interval * 60)
                {
                    stopWatch.Stop();
                    enabledTimer = false;
                    timerVibrate.Vibrate(TimeSpan.FromSeconds(3));
                    msg = "定时器已完成！\n";
                }
                else
                {
                    msg = DateTime.Now.ToString("h:mm:ss") + " : " + elapseSecond + " s\n";
                }

                tmp_output_Block.Dispatcher.BeginInvoke(delegate() { tmp_output_Block.Text = msg + tmp_output_Block.Text; });
            }
        }

        private void txtMinuteNum_GotFocus(object sender, RoutedEventArgs e)
        {
            if (minutenumBox.Text.Trim() == defaultMinuterNum)
            {
                minutenumBox.Text = "";
            }
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            stopWatch.Stop();
            enabledTimer = false;
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