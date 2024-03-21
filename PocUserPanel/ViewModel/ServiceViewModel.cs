using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using ModernDashboard.Model;

namespace ModernDashboard.ViewModel
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource ServiceItemsCollection;
        public ICollectionView ServiceSourceCollection => ServiceItemsCollection.View;

        public ServiceViewModel()
        {
            ObservableCollection<ServiceItems> serviceItems = new ObservableCollection<ServiceItems>
            {

            };

            ServiceItemsCollection = new CollectionViewSource { Source = serviceItems };
            ServiceItemsCollection.Filter += MenuItems_Filter;

        }

        private string filterText;
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                ServiceItemsCollection.View.Refresh();
                OnPropertyChanged("FilterText");
            }
        }

        private void MenuItems_Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }

            ServiceItems _item = e.Item as ServiceItems;
            if (_item.ServiceName.ToUpper().Contains(FilterText.ToUpper()))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
