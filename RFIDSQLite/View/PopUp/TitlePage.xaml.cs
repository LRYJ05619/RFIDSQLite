using RFIDSQLite.Service;
using RFIDSQLite.ViewModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class TitlePage : Popup
{
	public TitlePage()
	{
		InitializeComponent();
        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;
        MessagingCenter.Subscribe<TitlePageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }
    private void Password_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue == null)
            return;

        string text = e.NewTextValue;

        string numberPattern = "^[0-9]*$";

        if (!Regex.IsMatch(text, numberPattern))
        {
            // 输入无效，阻止更改
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}