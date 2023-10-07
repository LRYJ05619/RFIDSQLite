using System.IO.Ports;

namespace RFIDSQLite.Service
{
    public class RFIDService
    {
        public static SerialPort serialPort = new();

        // 自定义事件，用于封装串口数据接收事件
        public static event EventHandler<byte[]> ReceivedDataEvent;

        //存放串口列表
        public static string[] ports;

        public RFIDService()
        {
            // 订阅串口的数据接收事件
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        // 串口数据接收事件处理
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // 从串口读取接收到的数据
                byte[] buffer = new byte[serialPort.BytesToRead];
                serialPort.Read(buffer, 0, buffer.Length);

                // 在主线程上触发自定义事件，通知主程序接收到数据
                Device.BeginInvokeOnMainThread(() =>
                {
                    ReceivedDataEvent?.Invoke(this, buffer);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"接收串口数据时出现异常: {ex.Message}");
            }
        }

        //获取串口列表
        public static void GetPorts()
        {
            ports = System.IO.Ports.SerialPort.GetPortNames();
        }

        /*状态判断  返回值0 串口已打开
                    返回值1 串口正常打开
                    返回值2 串口打开失败，出现异常
                    返回值3 输入串口为空*/
        public static int OpenPortCheck(string portName)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();//串口关闭
                }
                catch { }
                return 0;
            }
            else
            {
                if (!string.IsNullOrEmpty(portName))
                {
                    try
                    {
                        serialPort.PortName = portName;

                        serialPort.BaudRate = 115200; // 设置波特率
                        serialPort.DataBits = 8; // 设置数据位
                        serialPort.Parity = Parity.None; // 设置校验位
                        serialPort.StopBits = StopBits.One; // 设置停止位

                        serialPort.RtsEnable = false;
                        serialPort.DtrEnable = false;

                        serialPort.ReceivedBytesThreshold = 1;

                        serialPort.Open(); // 打开串口
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"连接到串口时出现异常: {ex.Message}");
                        return 2;
                    }
                }
                else
                {
                    Console.WriteLine("没有可用的串口。");
                    return 3;
                }
            }
        }

        //串口发送
        private static bool DataSent(byte[] buffer)
        {
            try
            {
                serialPort.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送串口数据时出现异常: {ex.Message}");
                return false;
            }
        }

        //读取
        public static bool ReadData()
        {
            byte[] Data = new byte[8];
            Data[0] = 0xA0;
            Data[1] = 0x06;
            Data[2] = 0x01;
            Data[3] = 0x81;
            Data[4] = 0x01;
            Data[5] = 0x02;
            Data[6] = 0x08;
            Data[7] = 0xCD;
            if (DataSent(Data))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //写入标签
        public static bool WriteData(byte[] buf)
        {
            byte[] DATABuffer = new byte[30];
            //Head(头字符）
            DATABuffer[0] = 0xA0;
            //Len
            DATABuffer[1] = 0x1C;
            //address
            DATABuffer[2] = 0x01;
            //cmd
            DATABuffer[3] = 0x82;
            //密码
            DATABuffer[4] = 0x00;
            DATABuffer[5] = 0x00;
            DATABuffer[6] = 0x00;
            DATABuffer[7] = 0x00;
            //MemBank(储存区域默认EPC）
            DATABuffer[8] = 0x01;
            //WordAdd(数据首地址)
            DATABuffer[9] = 0x01;
            //WordCnt(写入长度)
            DATABuffer[10] = 0x09;
            //更改PC值
            DATABuffer[11] = 0x40;
            DATABuffer[12] = 0x00;

            for (int i = 0; i < 16; i++)
            {
                DATABuffer[i + 13] = buf[i];
            }

            //校验
            DATABuffer[29] = CheckSum(DATABuffer, 29);

            if (DataSent(DATABuffer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //校验码
        public static byte CheckSum(byte[] uBuff, byte uBuffLen)
        {
            byte i = 0;
            byte uSum = 0;
            for (i = 0; i < uBuffLen; i++)
            {
                uSum = (byte)(uSum + uBuff[i]);
            }
            uSum = (byte)((~uSum) + 1);
            return uSum;
        }

    }
}
