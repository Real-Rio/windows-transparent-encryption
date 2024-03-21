
/// <summary>
/// ViewModel - [ "The Connector" ]
/// ViewModel exposes data contained in the Model objects to the View. The ViewModel performs 
/// all modifications made to the Model data.
/// </summary>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ModernDashboard.Model;

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
       
    }
}
