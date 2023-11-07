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

        //前往主页
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "GoToMainPage", (sender) =>
        {
            Navigation.PushModalAsync(new MainPage());
            
        });

        //新增项目
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            //打开popup
            var popup = new AddProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //添加项目界面通知
        MessagingCenter.Subscribe<AddProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //重新打开添加项目界面
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new AddProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //重新打开添加项目界面
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenDeletePage", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new DeleteProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //串口错误回调
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPortDataPageIsPrj", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new PortsPage(new PortsPageViewModel())
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //设备管理按钮
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenPortDataPage", (sender) =>
        {
            //打开popup
            var popup = new PortsPage(new PortsPageViewModel())
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //串口成功回调
        MessagingCenter.Subscribe<PortsPageViewModel, string>(this, "OpenNotifyPageIsPrj", (sender, message) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //项目页面通知
        MessagingCenter.Subscribe<ProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            //打开popup
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });
    }
}