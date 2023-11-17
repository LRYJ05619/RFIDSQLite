using RFIDSQLite.Service;
using RFIDSQLite.ViewModel;

namespace RFIDSQLite.View;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        TitleLabel.Text = TitleGetService.Get();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new ProjectPage());
    }
}