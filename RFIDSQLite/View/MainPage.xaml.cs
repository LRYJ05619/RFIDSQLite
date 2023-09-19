using CommunityToolkit.Maui.Views;
using Microsoft.UI.Xaml.Input;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;
using System.Collections.ObjectModel;
using Windows.System;
using RFIDSQLite.Model;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;

namespace RFIDSQLite.View
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel; // 添加一个类级别的 viewModel 变量
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel; // 初始化 viewModel 变量
            BindingContext = this.viewModel;

            //添加按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenAddDataPage", (sender) =>
            {
                //打开popup
                var popup = new AddDataPage(new AddDataPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //写入失败回调(重新打开添加页面)
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenAddDataPage", (sender) =>
            {
                //打开popup
                var popup = new AddDataPage(new AddDataPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //添加数据成功回调
            MessagingCenter.Subscribe<AddDataPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message));
                this.ShowPopup(popup);
            });

            //设备管理按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenPortDataPage", (sender) =>
            {
                //打开popup
                var popup = new PortsPage(new PortsPageViewModel());
                this.ShowPopup(popup);
            });

            //主页通知
            MessagingCenter.Subscribe<MainPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message));
                this.ShowPopup(popup);
            });

            //删除按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenDeletePage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new DeletePage(new DeletePageViewModel());
                this.ShowPopup(popup);
            });

            //串口错误回调
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPortDataPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new PortsPage(new PortsPageViewModel());
                this.ShowPopup(popup);
            });

            //串口成功回调
            MessagingCenter.Subscribe<PortsPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message));
                this.ShowPopup(popup);
            });
        }
    }
}