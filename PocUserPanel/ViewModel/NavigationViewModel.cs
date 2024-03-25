
/// <summary>
/// ViewModel - [ "The Connector" ]
/// ViewModel exposes data contained in the Model objects to the View. The ViewModel performs 
/// all modifications made to the Model data.
/// </summary>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ModernDashboard.Model;
using ModernDashboard.View;

namespace ModernDashboard.ViewModel
{
    class NavigationViewModel : INotifyPropertyChanged
    {
        // CollectionViewSource enables XAML code to set the commonly used CollectionView properties,
        // passing these settings to the underlying view.
        private CollectionViewSource MenuItemsCollection;

        // ICollectionView enables collections to have the functionalities of current record management,
        // custom sorting, filtering, and grouping.
        public ICollectionView SourceCollection => MenuItemsCollection.View;

        public NavigationViewModel()
        {
            // ObservableCollection represents a dynamic data collection that provides notifications when items
            // get added, removed, or when the whole list is refreshed.
            ObservableCollection<MenuItems> menuItems = new ObservableCollection<MenuItems>
            {
                new MenuItems { MenuName = "授权进程管理", MenuImage = @"Assets/Desktop_Icon.png" },
                new MenuItems { MenuName = "特权加解密", MenuImage = @"Assets/Document_Icon.png" },
                new MenuItems { MenuName = "机密文件后缀", MenuImage = @"Assets/Download_Icon.png" },
                new MenuItems { MenuName = "开启驱动", MenuImage = @"Assets/Images_Icon.png" },
                new MenuItems { MenuName = "机密文件夹", MenuImage = @"Assets/Images_Icon.png" },

            };

            MenuItemsCollection = new CollectionViewSource { Source = menuItems };
            MenuItemsCollection.Filter += MenuItems_Filter;

            // Set Startup Page
            SelectedViewModel = new StartupViewModel();

            // Set Driver Status
            UpdateDriverStatus();

            _instance = this;
        }

        // Implement interface member for INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        // Text Search Filter.
        private string filterText;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                MenuItemsCollection.View.Refresh();
                OnPropertyChanged("FilterText");
            }
        }
       
        private void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if(string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            MenuItems _item = e.Item as MenuItems;
            if(_item.MenuName.ToUpper().Contains(FilterText.ToUpper()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        // Select ViewModel
        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set { _selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        // Switch Views
        public void SwitchViews(object parameter)
        {
            switch(parameter)
            {
                case "授权进程管理":
                    SelectedViewModel = new DesktopViewModel();
                    break;
                case "特权加解密":
                    SelectedViewModel = new DocumentViewModel();
                    break;
                case "机密文件后缀":
                    SelectedViewModel = new DownloadViewModel();
                    break;
                case "开启驱动":
                    SelectedViewModel = new ServiceViewModel();
                    break;
                case "机密文件夹":
                    SelectedViewModel = new PictureViewModel();
                    break;

                default:
                    SelectedViewModel = new DesktopViewModel();
                    break;
            }
        }
        // 驱动状态
        private object _driverStatus;
        public object DriverStatus
        {
            get => _driverStatus;
            set { _driverStatus = value; OnPropertyChanged("DriverStatus"); }
        }

        // Menu Button Command
        private ICommand _menucommand;
        public ICommand MenuCommand
        {
            get
            {
                if (_menucommand == null)
                {
                    _menucommand = new RelayCommand(param => SwitchViews(param));
                }
                return _menucommand;
            }
        }


        // Close App
        public void CloseApp(object obj)
        {
            MainWindow win = obj as MainWindow;
            win.Close();
        }

        // Close App Command
        private ICommand _closeCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(p => CloseApp(p));
                }
                return _closeCommand;
            }
        }

        // Update Driver Status
        public int UpdateDriverStatus()
        {
            //DriverStatus = obj;
            int status = ServiceView.CheckService();
            if (status == 0)
            {
                //driverStatus.Content = "未安装";
                DriverStatus = "未安装";
                //MessageBox.Show("请先安装Poc驱动");
            }
            else
            {
                ServiceControllerStatus serviceStatus = (ServiceControllerStatus)status;
                switch (serviceStatus)
                {
                    case ServiceControllerStatus.Running:
                        //driverStatus.Content = "运行中";
                        DriverStatus = "运行中";
                        break;
                    case ServiceControllerStatus.Stopped:
                        //driverStatus.Content = "已停止";
                        DriverStatus = "已停止";
                        break;
                    case ServiceControllerStatus.Paused:
                        //driverStatus.Content = "已暂停";
                        DriverStatus = "已暂停";
                        break;
                    default:
                        //driverStatus.Content = "未知状态";
                        DriverStatus = "未知状态";
                        break;
                }
            }
            return status;
        }

        public string GetDriverStatus()
        {
            return DriverStatus.ToString();
        }

        //private ICommand _updateStatusCommand;

        //public ICommand UpdateStatusCommand
        //{
        //    get
        //    {
        //        if(_updateStatusCommand == null)
        //        {
        //            _updateStatusCommand = new RelayCommand(UpdateDriverStatus);
        //        }
        //        return _updateStatusCommand;
        //    }
        //}

        // 获取当前类的实例
        private static NavigationViewModel _instance;
        public static NavigationViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationViewModel();
                }
                return _instance;
            }
        }
       
    }
}
