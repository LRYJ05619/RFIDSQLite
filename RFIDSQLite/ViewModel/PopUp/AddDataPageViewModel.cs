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

            byte[] SerialData = new byte[16];
            int byteIndex = 0;

            for (int i = 0; i < Serial.Length; i += 2)
            {
                // 从输入字符串中获取两个数字字符
                string digitPair = Serial.Substring(i, 2);

                // 将数字字符解析为字节并存储在字节数组中
                if (byteIndex < SerialData.Length)
                {
                    SerialData[byteIndex] = byte.Parse(digitPair);
                    byteIndex++;
                }
            }

            MessagingCenter.Send(this, "ClosePopupMessage");

            if (!RFIDService.WriteData(SerialData))
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
