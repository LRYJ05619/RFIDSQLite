using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class PropertyPage : Popup
{
	public PropertyPage(PropertyPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

        MessagingCenter.Subscribe<PropertyPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }
}