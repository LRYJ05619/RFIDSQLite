using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class DeletePageViewModel : ObservableValidator
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
