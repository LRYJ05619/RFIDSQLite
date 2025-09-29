using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Service
{
    public static class PopupHelper
    {
        public static void ShowPopupOnCurrentPage(Popup popup)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var currentPage = GetCurrentPage();
                if (currentPage != null)
                {
                    currentPage.ShowPopup(popup);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("无法找到当前页面显示 Popup");
                }
            });
        }

        private static Page GetCurrentPage()
        {
            var mainPage = Application.Current?.MainPage;

            if (mainPage is NavigationPage navPage)
            {
                return navPage.CurrentPage;
            }

            if (mainPage is Shell shell)
            {
                return shell.CurrentPage;
            }

            return mainPage;
        }
    }
}
