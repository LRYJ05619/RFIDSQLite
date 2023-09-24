using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
