using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;
using CommunityToolkit.Maui.Views;

public partial class NotifyPage : Popup
{
    public NotifyPage(NotifyPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }
}