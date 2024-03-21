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

        private void checkService_Click(object sender, RoutedEventArgs e)
        {
            string Service_Name = ServiceName.Text;

            try
            {
                ServiceController sc = new ServiceController(Service_Name);

                // print service status
                MessageBox.Show("Service Status: " + sc.Status.ToString());
            }
            catch (Exception)
            {

                MessageBox.Show("Service Not Found");
            }

            
            
        }

    }
}
