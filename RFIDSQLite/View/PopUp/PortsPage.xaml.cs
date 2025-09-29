using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class PortsPage : Popup
{
    public PortsPage(PortsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;
        // ������Ϣ�������յ� "ClosePopupMessage" ��Ϣʱ��ִ�йر� Popup �Ĳ���
        MessagingCenter.Subscribe<PortsPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            // �ر� Popup
            CloseAsync();
        });

    }
}