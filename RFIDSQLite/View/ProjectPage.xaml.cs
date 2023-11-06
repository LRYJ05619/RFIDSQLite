using CommunityToolkit.Maui.Views;
using RFIDSQLite.View;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.ViewModel;

public partial class ProjectPage : ContentPage
{
	public ProjectPage()
	{
		InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        //Ç°ÍùÖ÷Ò³
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "GoToMainPage", (sender) =>
        {
            Navigation.PopModalAsync();
        });
    }
}