using RFIDSQLite.Service;
using RFIDSQLite.ViewModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View;

public partial class StartPage : ContentPage
{
    private const string CorrectPassword = "951236";
    private const string PasswordVerifiedKey = "PasswordVerified";

    public StartPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);

        TitleLabel.Text = TitleGetService.Get();

        //�����¼
        Preferences.Remove("PasswordVerified");
        // ����Ƿ��Ѿ���֤������
        CheckIfAlreadyVerified();
    }
    private async void CheckIfAlreadyVerified()
    {
        if (Preferences.Get(PasswordVerifiedKey, false))
        {
            // �Ѿ���֤����ֱ����ת
            await NavigateToProjectPage();
        }
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        if (password.Text != "951236")
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new NotifyPage(new NotifyPageViewModel("�������"))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
            return;
        }
        // ������֤״̬
        Preferences.Set(PasswordVerifiedKey, true);
        await Navigation.PushModalAsync(new ProjectPage());
    }
    private async Task NavigateToProjectPage()
    {
        await Navigation.PushModalAsync(new ProjectPage());
    }

    private void Password_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue == null)
            return;

        string text = e.NewTextValue;

        string numberPattern = "^[0-9]*$";

        if (!Regex.IsMatch(text, numberPattern))
        {
            // ������Ч����ֹ����
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}