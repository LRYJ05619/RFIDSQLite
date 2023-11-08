using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class DeleteProjectPage : Popup
{
	public DeleteProjectPage()
	{
		InitializeComponent();

        MessagingCenter.Subscribe<DeleteProjectPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }
}