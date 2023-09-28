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
            MessagingCenter.Send(this, "ClosePopupMessage");

            if (Message == "请选择串口！")
            {
                MessagingCenter.Send(this, "OpenPortDataPage");
            }

            if (Message == "写入失败，请检查编号！")
            {
                MessagingCenter.Send(this, "OpenAddDataPage");
            }

            if (Message == "请输入编号！")
            {
                MessagingCenter.Send(this, "OpenAddDataPage");
            }

            if (Message == "编号重复，请重新输入！")
            {
                MessagingCenter.Send(this, "OpenAddDataPage");
            }

            if (Message == "写入失败，请检查设备连接！")
            {
                MessagingCenter.Send(this, "OpenAddDataPage");
            }

            if (Message == "请检查芯片位置！")
            {
                MessagingCenter.Send(this, "OpenAddDataPage");
            }

            if (Message == "修改成功！")
            {
                MessagingCenter.Send(this, "RefreshPage");
            }

            if (Message == "保存成功！")
            {
                MessagingCenter.Send(this, "RefreshPage");
            }
        }
    }
}
