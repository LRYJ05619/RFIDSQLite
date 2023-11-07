using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class NotifyPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        string message;

        public NotifyPageViewModel(string message)
        {
            Message = message;
        }

        [RelayCommand]
        void Cancel()
        {
            switch (Message)
            {
                case "请选择串口！":
                    MessagingCenter.Send(this, "OpenPortDataPage");
                    break;

                case "写入失败，请检查编号！":
                    if (RFIDService.IsRFID)
                    {
                        RFIDService.IsRFID = false;
                        MessagingCenter.Send(this, "OpenWriteChipPage");
                    }
                    else
                    {
                        MessagingCenter.Send(this, "OpenAddDataPage");
                    }
                    break;

                case "请输入编号！":
                    if (RFIDService.IsRFID)
                    {
                        RFIDService.IsRFID = false;
                        MessagingCenter.Send(this, "OpenWriteChipPage");
                    }
                    else
                    {
                        MessagingCenter.Send(this, "OpenAddDataPage");
                    }
                    break;

                case "编号重复，请重新输入！":
                    MessagingCenter.Send(this, "OpenAddDataPage");
                    break;

                case "写入失败，请检查芯片位置！":
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "修改成功！":
                    MessagingCenter.Send(this, "RefreshPage");
                    break;

                case "保存成功！":
                    MessagingCenter.Send(this, "RefreshPage");
                    break;

                case "写入失败，请检查设备连接！":
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "请先新增编号！":
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "保存失败，请重新设置编码长度！":
                    MessagingCenter.Send(this, "OpenPropertyPage");
                    break;
            }
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
