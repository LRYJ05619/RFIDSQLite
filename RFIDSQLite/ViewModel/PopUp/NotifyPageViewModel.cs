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
                    if (RFIDService.IsPrj)
                    {
                        MessagingCenter.Send(this, "OpenPortDataPageIsPrj");
                    }
                    else
                    {
                        MessagingCenter.Send(this, "OpenPortDataPage");
                    }
                    break;

                case "绑定失败，请检查编号！":
                    RFIDService.IsRFID = false;
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "保存失败，请检查编号！":
                    MessagingCenter.Send(this, "OpenAddDataPage");
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

                case "绑定失败，请检查芯片位置！":
                    MessagingCenter.Send(this, "OpenAddDataPage");
                    break;

                case "修改成功！":
                    MessagingCenter.Send(this, "RefreshPage");
                    break;

                case "保存成功！":
                    MessagingCenter.Send(this, "RefreshPage");
                    break;

                case "绑定失败，请检查设备连接！":
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "请先新增编号！":
                    MessagingCenter.Send(this, "OpenWriteChipPage");
                    break;

                case "保存失败，请重新设置编码长度！":
                    MessagingCenter.Send(this, "OpenPropertyPage");
                    break;
                case "创建失败，请重新设置编码长度！":
                    MessagingCenter.Send(this, "OpenAddProjectPage");
                    break;
                case "创建失败，请设置项目名称！":
                    MessagingCenter.Send(this, "OpenAddProjectPage");
                    break;
                case "项目创建成功！":
                    MessagingCenter.Send(this, "RefreshProjectPage");
                    break;

                case "读取成功！":
                    MessagingCenter.Send(this, "ReadSuccess");
                    break;
            }
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
