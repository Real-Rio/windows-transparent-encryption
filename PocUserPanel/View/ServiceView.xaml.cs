using System;
using System.ServiceProcess;
using System.Collections.Generic;
using System.Linq;
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
using System.Runtime.InteropServices;
using System.Threading;
using ModernDashboard.ViewModel;

namespace ModernDashboard.View
{
    /// <summary>
    /// Interaction logic for PictureView.xaml
    /// </summary>
    public partial class ServiceView : UserControl
    {
        public ServiceView()
        {
            InitializeComponent();
        }

        [DllImport("PocUserDll.dll")]
        static extern int AuthorizeAndOpenDriver();

        const int AUTHORIZATION_NOUSBKEY = 0;
        const int AUTHORIZATION_NOSIGNATUREKEY = 1;
        const int AUTHORIZATION_ERRGETCERT = 2;
        const int AUTHORIZATION_ERRMALLOC = 3;
        const int AUTHORIZATION_ERRGETCERTCONTEXT = 4;
        const int AUTHORIZATION_ERRGETCERTSTORE = 5;
        const int AUTHORIZATION_SUCCESS = 6;
        const int AUTHORIZATION_FALSE = 7;
        const string serviceName = "Poc";





        private void UpdateService(object sender, RoutedEventArgs e)
        {
            NavigationViewModel instance = NavigationViewModel.Instance;
            instance.UpdateDriverStatus();          
        }

        private void CloseService(object sender, RoutedEventArgs e)
        {
            ServiceController sc = new ServiceController(serviceName);
            if(sc.Status!=ServiceControllerStatus.Running)
            {
                MessageBox.Show("驱动未启动");
                return;
            }
            sc.Stop();
            Thread.Sleep(1000);
            //sc.Refresh();

            NavigationViewModel instance = NavigationViewModel.Instance;
            int newStatus = instance.UpdateDriverStatus();
            if(newStatus==(int)ServiceControllerStatus.Stopped)
            {
                MessageBox.Show("驱动已成功关闭");
            }
            else
            {
                MessageBox.Show("关闭驱动失败，请稍后重试");
            }


        }


        private void OpenService(object sender, RoutedEventArgs e)
        {
            NavigationViewModel instance = NavigationViewModel.Instance;
            if(instance.GetDriverStatus()!= "已停止")
            {
                MessageBox.Show("请确保驱动已经正确安装，且状态处于已停止");
                return;
            }

            int status = AuthorizeAndOpenDriver();
            instance.UpdateDriverStatus();
            if (status == AUTHORIZATION_SUCCESS)
            {
                // TODO :打开驱动
                ServiceController sc = new ServiceController(serviceName);
                sc.Start();
                Thread.Sleep(1000);
                //sc.Refresh();

                int newStatus =  instance.UpdateDriverStatus();
                if (newStatus == (int)ServiceControllerStatus.Running)
                {
                    MessageBox.Show("驱动已成功启动");
                }
                else
                {
                    MessageBox.Show("驱动启动失败，请稍后重试");
                }
                return;
            }
            else if (status == AUTHORIZATION_NOUSBKEY)
            {
                MessageBox.Show("未检测到USBKEY");
            }
            //else if(status==AUTHORIZATION_NOSIGNATUREKEY)
            //{
            //    MessageBox.Show("未检测到签名KEY");
            //}
            //else if(status==AUTHORIZATION_ERRGETCERT)
            //{
            //    MessageBox.Show("获取证书失败");
            //}
            //else if(status==AUTHORIZATION_ERRMALLOC)
            //{
            //    MessageBox.Show("内存分配失败");
            //}
            //else if(status==AUTHORIZATION_ERRGETCERTCONTEXT)
            //{
            //    MessageBox.Show("获取证书上下文失败");
            //}
            //else if(status==AUTHORIZATION_ERRGETCERTSTORE)
            //{
            //    MessageBox.Show("获取证书存储失败");
            //}
            else if (status == AUTHORIZATION_FALSE)
            {
                MessageBox.Show("UKey认证失败");
            }
            else
            {
                MessageBox.Show("发生错误，返回值为 "+status);
            }

            return;


        }


        static public int CheckService()
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                return (int)sc.Status;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
