using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class DeleteProjectPageViewModel : ObservableValidator
    {
        [ObservableProperty] private bool isChecked;

        [RelayCommand]
        void Confirm()
        {
            if (IsChecked)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "DeleteMessage");
            }
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
