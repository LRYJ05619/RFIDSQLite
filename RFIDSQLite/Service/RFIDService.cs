using System.IO.Ports;
using RFIDSQLite.Model;
using System.Management;

namespace RFIDSQLite.Service
{
    public class RFIDService
    {
        public static List<TodoSQLite> search;
        public static string data;

        public static bool IsMain = false;

        public static bool IsRFID = false;
        public static bool IsPrj = false;

        public static SerialPort serialPort = new();

        // 自定义事件，用于封装串口数据接收事件
        public static event EventHandler<byte[]> ReceivedDataEvent;

        // 设备连接状态变化事件
        public static event EventHandler<bool> DeviceConnectionChanged;

        // 目标设备的VID和PID
        private const string TARGET_VID = "10C4";
        private const string TARGET_PID = "EA60";

        // 目标设备的完整DeviceID（可选，用于更精确的匹配）
        private const string TARGET_DEVICE_ID = "62779D67B1B7E8119C04712BCB5E5982";

        // 存放串口列表
        public static string[] ports;

        // 设备监控对象
        private static ManagementEventWatcher insertWatcher;
        private static ManagementEventWatcher removeWatcher;

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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ReceivedDataEvent?.Invoke(this, buffer);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"接收串口数据时出现异常: {ex.Message}");
            }
        }

        // 获取串口列表
        public static void GetPorts()
        {
            ports = System.IO.Ports.SerialPort.GetPortNames();
        }

        // 查找目标设备的COM口（支持完整DeviceID匹配）
        public static string FindTargetDevice()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%(COM%'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string deviceId = obj["DeviceID"]?.ToString();
                        string caption = obj["Caption"]?.ToString();

                        if (deviceId != null &&
                            deviceId.Contains($"VID_{TARGET_VID}") &&
                            deviceId.Contains($"PID_{TARGET_PID}"))
                        {
                            // 如果指定了完整的DeviceID，则进行更精确的匹配
                            if (!string.IsNullOrEmpty(TARGET_DEVICE_ID))
                            {
                                if (!deviceId.Contains(TARGET_DEVICE_ID))
                                {
                                    continue; // 跳过不匹配的设备
                                }
                            }

                            // 从Caption中提取COM口号
                            var match = System.Text.RegularExpressions.Regex.Match(caption, @"COM(\d+)");
                            if (match.Success)
                            {
                                string comPort = match.Value;
                                Console.WriteLine($"找到目标设备: {comPort}，DeviceID: {deviceId}");
                                return comPort;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找设备时出现异常: {ex.Message}");
            }

            return null;
        }

        // 自动连接目标设备
        public static bool AutoConnectTargetDevice()
        {
            string targetPort = FindTargetDevice();

            if (targetPort != null)
            {
                int result = OpenPortCheck(targetPort);
                if (result == 1)
                {
                    Console.WriteLine($"成功自动连接到设备: {targetPort}");
                    DeviceConnectionChanged?.Invoke(null, true);
                    return true;
                }
                else
                {
                    Console.WriteLine($"尝试连接设备失败，返回码: {result}");
                }
            }
            else
            {
                Console.WriteLine("未找到目标设备");
            }

            return false;
        }

        // 启动设备监控
        public static void StartDeviceMonitoring()
        {
            try
            {
                // 监控USB设备插入
                WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
                insertWatcher = new ManagementEventWatcher(insertQuery);
                insertWatcher.EventArrived += DeviceInsertedEvent;
                insertWatcher.Start();

                // 监控USB设备移除
                WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
                removeWatcher = new ManagementEventWatcher(removeQuery);
                removeWatcher.EventArrived += DeviceRemovedEvent;
                removeWatcher.Start();

                Console.WriteLine("设备监控已启动");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启动设备监控时出现异常: {ex.Message}");
            }
        }

        // 停止设备监控
        public static void StopDeviceMonitoring()
        {
            try
            {
                if (insertWatcher != null)
                {
                    insertWatcher.Stop();
                    insertWatcher.Dispose();
                }

                if (removeWatcher != null)
                {
                    removeWatcher.Stop();
                    removeWatcher.Dispose();
                }

                Console.WriteLine("设备监控已停止");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止设备监控时出现异常: {ex.Message}");
            }
        }

        // 设备插入事件处理
        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            try
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                string deviceId = instance["DeviceID"]?.ToString();

                if (deviceId != null &&
                    deviceId.Contains($"VID_{TARGET_VID}") &&
                    deviceId.Contains($"PID_{TARGET_PID}"))
                {
                    // 如果指定了完整的DeviceID，则进行更精确的匹配
                    if (!string.IsNullOrEmpty(TARGET_DEVICE_ID))
                    {
                        if (!deviceId.Contains(TARGET_DEVICE_ID))
                        {
                            return; // 不是目标设备，忽略
                        }
                    }

                    Console.WriteLine("检测到目标设备插入");

                    // 延迟一小段时间，确保设备完全初始化
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            AutoConnectTargetDevice();
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理设备插入事件时出现异常: {ex.Message}");
            }
        }

        // 设备移除事件处理
        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            try
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                string deviceId = instance["DeviceID"]?.ToString();

                if (deviceId != null &&
                    deviceId.Contains($"VID_{TARGET_VID}") &&
                    deviceId.Contains($"PID_{TARGET_PID}"))
                {
                    // 如果指定了完整的DeviceID，则进行更精确的匹配
                    if (!string.IsNullOrEmpty(TARGET_DEVICE_ID))
                    {
                        if (!deviceId.Contains(TARGET_DEVICE_ID))
                        {
                            return; // 不是目标设备，忽略
                        }
                    }

                    Console.WriteLine("检测到目标设备移除");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (serialPort.IsOpen)
                        {
                            try
                            {
                                serialPort.Close();
                            }
                            catch { }
                        }
                        DeviceConnectionChanged?.Invoke(null, false);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理设备移除事件时出现异常: {ex.Message}");
            }
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
                    serialPort.Close();
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
                        serialPort.BaudRate = 115200;
                        serialPort.DataBits = 8;
                        serialPort.Parity = Parity.None;
                        serialPort.StopBits = StopBits.One;
                        serialPort.RtsEnable = false;
                        serialPort.DtrEnable = false;
                        serialPort.ReceivedBytesThreshold = 1;

                        serialPort.Open();
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

        // 串口发送
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

        // 读取
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

        // 写入标签
        public static bool WriteData(byte[] buf)
        {
            byte[] DATABuffer = new byte[30];
            DATABuffer[0] = 0xA0;
            DATABuffer[1] = 0x1C;
            DATABuffer[2] = 0x01;
            DATABuffer[3] = 0x82;
            DATABuffer[4] = 0x00;
            DATABuffer[5] = 0x00;
            DATABuffer[6] = 0x00;
            DATABuffer[7] = 0x00;
            DATABuffer[8] = 0x01;
            DATABuffer[9] = 0x01;
            DATABuffer[10] = 0x09;
            DATABuffer[11] = 0x40;
            DATABuffer[12] = 0x00;

            for (int i = 0; i < 16; i++)
            {
                DATABuffer[i + 13] = buf[i];
            }

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

        // 校验码
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