using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Service;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class PortsPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        List<string> portList;

        [ObservableProperty]
        string port;

        [ObservableProperty]
        bool portState;

        public PortsPageViewModel()
        {
            PortState = RFIDService.serialPort.IsOpen;
            RFIDService.GetPorts();
            if (!PortState)
            {
                PortList = RFIDService.ports.ToList();
            }
            else
            {
                PortList = new List<string>();
                PortList.Add(RFIDService.serialPort.PortName);
                Port = PortList.FirstOrDefault();
            }
        }

        [RelayCommand]
        void SelectPort()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
            switch (RFIDService.OpenPortCheck(Port))
            {
                case 0:
                    MessagingCenter.Send(this, "OpenNotifyPage", "串口已关闭！");
                    break;
                case 1:
                    MessagingCenter.Send(this, "OpenNotifyPage", "串口已打开！");
                    break;
                case 2:
                    MessagingCenter.Send(this, "OpenNotifyPage", "串口异常！");
                    break;
                case 3:
                    MessagingCenter.Send(this, "OpenNotifyPage", "请选择串口！");
                    break;
            }
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
