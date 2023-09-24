using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class ManagerPageViewModel : ObservableObject
    {
        [ObservableProperty]
        List<String> _list;

        public event PropertyChangedEventHandler PropertyChanged;

        private Grid grid;
        public Grid Grid
        {
            get { return grid; }
            set
            {
                if (grid != value)
                {
                    grid = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
