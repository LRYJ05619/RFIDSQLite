using CommunityToolkit.Maui.Views;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            //添加按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenAddDataPage", (sender) =>
            {
                //打开popup
                var popup = new AddDataPage()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //写入失败回调(重新打开添加页面)
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenAddDataPage", (sender) =>
            {
                //打开popup
                var popup = new AddDataPage()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //添加数据成功回调
            MessagingCenter.Subscribe<AddDataPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //设备管理按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenPortDataPage", (sender) =>
            {
                //打开popup
                var popup = new PortsPage(new PortsPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //主页通知
            MessagingCenter.Subscribe<MainPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //删除按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenDeletePage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new DeletePage(new DeletePageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //串口错误回调
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPortDataPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new PortsPage(new PortsPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //串口成功回调
            MessagingCenter.Subscribe<PortsPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //删除按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenManagerPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new PropertyPage()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //串口成功回调
            MessagingCenter.Subscribe<PropertyPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //双击修改
            MessagingCenter.Subscribe<MainPageViewModel,TodoSQLite>(this, "OpenModifyDataPage", (sender,message) =>
            {
                //打开popup
                var popup = new ModifyDataPage(new ModifyDataPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //修改回调
            MessagingCenter.Subscribe<ModifyDataPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //写入芯片按钮
            MessagingCenter.Subscribe<MainPageViewModel>(this, "OpenWriteChipPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new WriteChipPage(new WriteChipPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //写入芯片失败
            MessagingCenter.Subscribe<WriteChipPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new NotifyPage(new NotifyPageViewModel(message))
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //重新打开写入芯片
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenWriteChipPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new WriteChipPage(new WriteChipPageViewModel())
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //重新属性管理界面
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPropertyPage", (sender) =>
            {
                // 打开 Popup，并使用传递的文本内容
                var popup = new PropertyPage()
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                };
                this.ShowPopup(popup);
            });

            //前往项目页
            MessagingCenter.Subscribe<MainPageViewModel>(this, "GoToProjectPage", (sender) =>
            {
                Navigation.PopModalAsync();
            });

            vm = new MainPageViewModel(); // 初始化 ViewModel
        }

        private MainPageViewModel vm;

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            RFIDService.IsMain = false;
            // 取消绑定事件，防止重复订阅
            RFIDService.ReceivedDataEvent -= vm.ReceivedData;

            if (BindingContext is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 重新绑定事件
            RFIDService.ReceivedDataEvent += vm.ReceivedData;
        }
    }
}