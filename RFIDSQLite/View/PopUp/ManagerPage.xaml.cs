using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class ManagerPage : Popup
{
	public ManagerPage(ManagerPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}