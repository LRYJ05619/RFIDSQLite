using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Service;
using System.Xml.Linq;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class WriteChipPageViewModel : ObservableObject
    {
        [ObservableProperty] 
        string serial;

        public WriteChipPageViewModel()
        {
            if (SQLiteService.WriteSerial != null)
            {
                Serial = new string(SQLiteService.WriteSerial);
            }
        }

        [RelayCommand]
        async Task WriteSerialAsync()
        {
            SQLiteService.WriteSerial = Serial;

            if (Serial == null || Serial == "")
            {
                RFIDService.IsRFID = true;
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "请输入编号！");
                return;
            }

            // 检查 Serial 的长度是否为额定值
            if (Serial.Length < 12)
            {
                RFIDService.IsRFID = true;
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查编号！");
                return;
            }

            var searchsSerial = await SQLiteService.SearchData(Serial);

            if (searchsSerial.Count == 0)
            {
                RFIDService.IsRFID = true;
                MessagingCenter.Send(this, "ClosePopupMessage");
                MessagingCenter.Send(this, "OpenNotifyPage", "请先新增编号！");
                return;
            }

            //创建待写数组
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

            //写入
            if (!RFIDService.WriteData(SerialData))
            {
                RFIDService.IsRFID = true;
                MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查设备连接！");
            }
            else
            {
                SQLiteService.Serial = Serial;
            }
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
