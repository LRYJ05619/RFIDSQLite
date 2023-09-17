using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class DeletePage : Popup
{
	public DeletePage(DeletePageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        MessagingCenter.Subscribe<DeletePageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }
}