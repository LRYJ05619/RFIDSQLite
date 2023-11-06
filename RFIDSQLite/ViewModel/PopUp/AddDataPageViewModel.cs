using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using System.Collections.ObjectModel;
using System.Numerics;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class AddDataPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        string serial;

        [ObservableProperty]
        int serialLength;

        [ObservableProperty]
        ObservableCollection<TodoSQLite> attributes;

        public AddDataPageViewModel()
        {
            if (SQLiteService.BufferSerial != null || SQLiteService.BufferProperty != null)
            {
                Serial = new string(SQLiteService.BufferSerial);
                Attributes = new ObservableCollection<TodoSQLite>(SQLiteService.BufferProperty);
            }
            else
            {
                Attributes = new ObservableCollection<TodoSQLite>(SQLiteService.Property);
            }

            SerialLength = SQLiteService.SerialLength;
        }

        [RelayCommand]
        async Task PopUpAddDataAsync()
        {
            SQLiteService.BufferSerial = Serial;
            SQLiteService.BufferProperty = Attributes;

            if (Serial == "" || Serial == null)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "请输入编号！");
                return;
            }

            // 检查 Serial 的长度是否为额定值
            if (Serial.Length != SerialLength)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "绑定失败，请检查编号！");
                return;
            }

            if (await SQLiteService.CheckSerial(Serial))
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "编号重复，请重新输入！");
                return;
            }


            //剩下部分补0
            SQLiteService.Serial = Serial;
            SQLiteService.Property = Attributes;

            await SQLiteService.AddData();

            //序号自增
            BigInteger number = BigInteger.Parse(SQLiteService.Serial);
            number++;
            SQLiteService.BufferSerial = number.ToString();

            if (SQLiteService.BufferSerial.Length != SQLiteService.SerialLength)
            {
                SQLiteService.BufferSerial = "0" + SQLiteService.BufferSerial;
            };

            MessagingCenter.Send(this, "ClosePopupMessage");
            MessagingCenter.Send(this, "RefreshPage");
            MessagingCenter.Send(this, "OpenNotifyPage", "保存成功！");
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
