using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
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

        public TitlePageViewModel()
        {

        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }

        [RelayCommand]
        void Change()
        {
            Preferences.Set("TitleVerified", TitleText);
            MessagingCenter.Send(this, "ChangeTitleMessage", TitleText);
            MessagingCenter.Send(this, "ClosePopupMessage");
        }

        [RelayCommand]
        async Task Logout()
        {
            try
            {
                Preferences.Remove("PasswordVerified");
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
