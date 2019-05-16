using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PortTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly object lockObj = new object();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Btn_Test(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(startIP.Text) || string.IsNullOrEmpty(endIP.Text) ||
                string.IsNullOrEmpty(startPort.Text) || string.IsNullOrEmpty(endPort.Text) ||string.IsNullOrEmpty(timeOut.Text))
            {
                MessageBox.Show("请输入完整");
                return;
            }
            //check ip 
            if (!Utility.CheckIP(startIP.Text.Trim()))
            {
                MessageBox.Show("开始ip错误");
                return;
            }
            if (!Utility.CheckIP(endIP.Text.Trim()))
            {
                MessageBox.Show("结束ip错误");
                return;
            }

            //将ip转为ulong
            //开始ip
            var startIPValue = Utility.IP2ULong(startIP.Text.Trim());
            //结束ip
            var endIPValue = Utility.IP2ULong(endIP.Text.Trim());
            if (startIPValue > endIPValue)
            {
                MessageBox.Show("开始ip大于结束ip");
                return;
            }

            var currentIPValue = startIPValue;

            var startPortValue = int.Parse(startPort.Text);
            var endPortValue = int.Parse(endPort.Text);
            if (startPortValue > endPortValue)
            {
                MessageBox.Show("开始端口大于结束端口");
                return;
            }
            var timeOutValue = int.Parse(timeOut.Text.Trim());
            while (currentIPValue <= endIPValue)
            {
                //loop test port
                var currentPortValue = int.Parse(startPort.Text);
                while (currentPortValue <= endPortValue)
                {
                    var ip = Utility.ULong2IP(currentIPValue);
                    var tmpPort = currentPortValue;
                    Task.Run(() =>
                    {
                        Test(ip, tmpPort, timeOutValue);
                    });
                    currentPortValue++;
                }
                currentIPValue++;
            }
        }

        private void Test(string ipStr, int port,int timeoutValue)
        {
            IPAddress ip = IPAddress.Parse(ipStr);
            IPEndPoint point = new IPEndPoint(ip, port);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var result = s.BeginConnect(point, null, null);
            result.AsyncWaitHandle.WaitOne(timeoutValue, true);
            if (!result.IsCompleted)
            {
                AddLog($"【{ipStr}:{port}】:timeout");
                s.Close();
            }
            else
            {
                AddLog($"【{ipStr}:{port}】:success");
                s.Close();
            }
            //2
            //try
            //{
            //    TcpClient tcp = new TcpClient();
            //    tcp.Connect(point);
            //    MessageBox.Show("端口打开");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void AddLog(string value)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                resultListView.Items.Add(value);
                resultListView.SelectedIndex = resultListView.Items.Count - 1;
                resultListView.ScrollIntoView(resultListView.SelectedItem);
            }));
        }
        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d*$"))
            {
                MessageBox.Show("输入有误");
                e.Handled = true;
            }
        }
        #region 焦点复制
        private void endIP_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(startIP.Text))
            {
                if (string.IsNullOrEmpty(endIP.Text))
                {
                    endIP.Text = startIP.Text;
                }
            }
        }

        private void endPort_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(startPort.Text))
            {
                if (string.IsNullOrEmpty(endPort.Text))
                {
                    endPort.Text = startPort.Text;
                }
            }
        }
        #endregion

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new PortTest.About().ShowDialog();
        }
    }
}
