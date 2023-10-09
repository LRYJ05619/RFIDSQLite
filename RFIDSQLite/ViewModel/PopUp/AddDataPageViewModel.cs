using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using System.Collections.ObjectModel;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class AddDataPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        string serial;

        [ObservableProperty]
        ObservableCollection<TodoSQLite> attributes;

        public AddDataPageViewModel()
        {
            if (SQLiteService.BufferSerial != null || SQLiteService.BufferProperty != null)
            {
                Serial = SQLiteService.BufferSerial;
                Attributes = SQLiteService.BufferProperty;
            }
            else
            {
                Attributes = new ObservableCollection<TodoSQLite>(SQLiteService.Property);
            }
        }

        [RelayCommand]
        async Task PopUpAddDataAsync()
        {
            SQLiteService.BufferSerial = Serial;
            SQLiteService.BufferProperty = Attributes;

            if (Serial == null || Serial == "")
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "请输入编号！");
                return;
            }

            // 检查 Serial 的长度是否为额定值
            if (Serial.Length < 12)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查编号！");
                return;
            }

            var searchsSerial = await SQLiteService.SearchData(Serial);

            if (searchsSerial.Count != 0)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "编号重复，请重新输入！");
                return;
            }

            MessagingCenter.Send(this, "ClosePopupMessage");

            if (!RFIDService.WriteData(Serial))
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查设备连接！");
            }
            else
            {
                SQLiteService.Serial = Serial;
                SQLiteService.Property = Attributes;
            }
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
