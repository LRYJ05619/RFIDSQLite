using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RFIDSQLite.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class TitlePageViewModel : ObservableValidator
    {
        [ObservableProperty] 
        private string titleText;

        [ObservableProperty]
        private string password;

        public TitlePageViewModel()
        {
            if (App.NewTitle != null)
            {
                TitleText = App.NewTitle;
            }
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }

        [RelayCommand]
        void Change()
        {
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int dynamicPart = day + hour;

            App.NewTitle = TitleText;
            if (Password != $"9512{dynamicPart:D2}")
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "WrongPassword", "密码错误！");
                return;
            }
            SimpleConfigService.SetTitle(TitleText);
            MessagingCenter.Send(this, "ChangeTitleMessage", TitleText);
            MessagingCenter.Send(this, "ClosePopupMessage");
        }

        [RelayCommand]
        async Task Logout()
        {
            try
            {
                SimpleConfigService.ClearPasswordVerified();
                // 先关闭当前的 Popup
                MessagingCenter.Send(this, "ClosePopup");

                // 等待 Popup 关闭
                await Task.Delay(100);

                // 重启应用
                try
                {
                    // 方法1：使用 Application.Current.Quit() 然后重新启动
                    var appPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                    System.Diagnostics.Process.Start(appPath);
                    Application.Current.Quit();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error restarting Windows app: {ex.Message}");

                    // 备用方法：只是退出应用
                    Application.Current.Quit();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LogoutAsync: {ex.Message}");
            }
        }
    }
}
