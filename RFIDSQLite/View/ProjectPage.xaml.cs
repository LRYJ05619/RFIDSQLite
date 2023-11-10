using CommunityToolkit.Maui.Views;
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

        //ǰ����ҳ
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "GoToMainPage", async (sender) =>
        {
            SQLiteService.SerialLength = SQLiteService.Project.SerialLength;
            SQLiteService.PrjNum = SQLiteService.Project.Id;
            await SQLiteService.InitProperty();

            RFIDService.IsMain = true;

            await Navigation.PushModalAsync(new MainPage());
        });

        //������Ŀ
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            //��popup
            var popup = new AddProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //�����Ŀ����֪ͨ
        MessagingCenter.Subscribe<AddProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //���´������Ŀ����
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenAddProjectPage", (sender) =>
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new AddProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //���´������Ŀ����
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenDeletePage", (sender) =>
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new DeleteProjectPage()
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //���ڴ���ص�
        MessagingCenter.Subscribe<NotifyPageViewModel>(this, "OpenPortDataPageIsPrj", (sender) =>
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new PortsPage(new PortsPageViewModel())
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //�豸����ť
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "OpenPortDataPage", (sender) =>
        {
            //��popup
            var popup = new PortsPage(new PortsPageViewModel())
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //���ڳɹ��ص�
        MessagingCenter.Subscribe<PortsPageViewModel, string>(this, "OpenNotifyPageIsPrj", (sender, message) =>
        {
            // �� Popup����ʹ�ô��ݵ��ı�����
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        //��Ŀҳ��֪ͨ
        MessagingCenter.Subscribe<ProjectPageViewModel, string>(this, "OpenNotifyPage", (sender, message) =>
        {
            //��popup
            var popup = new NotifyPage(new NotifyPageViewModel(message))
            {
                CanBeDismissedByTappingOutsideOfPopup = false
            };
            this.ShowPopup(popup);
        });

        vm = new ProjectPageViewModel(); // ��ʼ�� ViewModel
    }

    private ProjectPageViewModel vm;

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // ȡ�����¼�����ֹ�ظ�����
        RFIDService.ReceivedDataEvent -= vm.ReceivedData;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if(RFIDService.IsMain)
            return;

        var RfidService = new RFIDService();
        // ���°��¼�
        RFIDService.ReceivedDataEvent += vm.ReceivedData;
    }
}