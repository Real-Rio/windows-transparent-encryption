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

        private void UpdateService(object sender, RoutedEventArgs e)
        {
            NavigationViewModel instance = NavigationViewModel.Instance;
            instance.UpdateDriverStatus();          
        }

        private void OpenService(object sender, RoutedEventArgs e)
        {
           
        }


        static public int CheckService()
        {
            string serviceName = "Poc";
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
