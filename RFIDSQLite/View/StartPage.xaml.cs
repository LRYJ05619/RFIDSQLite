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

        //清理登录
        Preferences.Remove("PasswordVerified");
        // 检查是否已经验证过密码
        CheckIfAlreadyVerified();
    }
    private async void CheckIfAlreadyVerified()
    {
        if (Preferences.Get(PasswordVerifiedKey, false))
        {
            // 已经验证过，直接跳转
            await NavigateToProjectPage();
        }
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        if (password.Text != "951236")
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new NotifyPage(new NotifyPageViewModel("密码错误"))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
            return;
        }
        // 保存验证状态
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
            // 输入无效，阻止更改
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}