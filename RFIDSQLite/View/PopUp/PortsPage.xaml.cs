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
        // 订阅消息，当接收到 "ClosePopupMessage" 消息时，执行关闭 Popup 的操作
        MessagingCenter.Subscribe<PortsPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            // 关闭 Popup
            CloseAsync();
        });

    }
}