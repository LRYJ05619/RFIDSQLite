using CommunityToolkit.Maui.Views;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
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
        SubscribeMessages();

        vm = new ProjectPageViewModel(); // 初始化 ViewModel
    }

    protected void SubscribeMessages()
    {
        //前往主页
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "GoToMainPage", async (sender) =>
        {
            SQLiteService.SerialLength = SQLiteService.Project.SerialLength;
            SQLiteService.PrjNum = SQLiteService.Project.Id;
            await SQLiteService.InitProperty();

            RFIDService.IsMain = true;

            await Navigation.PushModalAsync(new MainPage());
        });

        //返回开始页
        MessagingCenter.Subscribe<TitlePageViewModel>(this, "GoToStartPage", async (sender) =>
        {
            await Navigation.PushModalAsync(new StartPage());
        });

        //修改标题
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenTitlePage", (sender) =>
        {
            //打开popup
            var popup = new TitlePage();
            this.ShowPopup(popup);
        });

        //新增项目
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            //打开popup
            var popup = new AddProjectPage();
            this.ShowPopup(popup);
        });

        //添加项目界面通知
        MessagingCenter.Subscribe<AddProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new NotifyPage(new NotifyPageViewModel(message));
            this.ShowPopup(popup);
        });

        //重新打开添加项目界面
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new AddProjectPage();
            this.ShowPopup(popup);
        });

        //打开删除项目界面
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenDeletePage", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new DeleteProjectPage();
            this.ShowPopup(popup);
        });

        //串口错误回调
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPortDataPageIsPrj", (sender) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new PortsPage(new PortsPageViewModel());
            this.ShowPopup(popup);
        });

        //设备管理按钮
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenPortDataPage", (sender) =>
        {
            //打开popup
            var popup = new PortsPage(new PortsPageViewModel());
            this.ShowPopup(popup);
        });

        //串口成功回调
        MessagingCenter.Subscribe<PortsPageViewModel, string>(this, "OpenNotifyPageIsPrj", (sender, message) =>
        {
            // 打开 Popup，并使用传递的文本内容
            var popup = new NotifyPage(new NotifyPageViewModel(message));
            this.ShowPopup(popup);
        });

        //项目页面通知
        MessagingCenter.Subscribe<ProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            //打开popup
            var popup = new NotifyPage(new NotifyPageViewModel(message));
            this.ShowPopup(popup);
        });

        //修改标题密码错误
        MessagingCenter.Subscribe<TitlePageViewModel, string>(this, "WrongPassword", (sender, message) =>
        {
            //打开popup
            var popup = new NotifyPage(new NotifyPageViewModel(message));
            this.ShowPopup(popup);
        });

        //修改标题密码错误,重新打开标题修改页
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenTitlePage", (sender) =>
        {
            //打开popup
            var popup = new TitlePage();
            this.ShowPopup(popup);
        });
    }

    private ProjectPageViewModel vm;

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // 取消绑定串口接受事件，防止重复订阅
        RFIDService.ReceivedDataEvent -= vm.ReceivedData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if(RFIDService.IsMain)
            return;

        var RfidService = new RFIDService();
        // 重新绑定串口接受事件
        RFIDService.ReceivedDataEvent += vm.ReceivedData;
    }
}