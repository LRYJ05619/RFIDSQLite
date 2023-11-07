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
        [RelayCommand]
        void Confirm()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
            MessagingCenter.Send(this, "DeleteMessage");
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
