using MediaDevices;

namespace RFIDSQLite.Service
{
    public class DeviceService
    {
        public static bool WriteFile()
        {
            try
            {
                // 获取可用的存储设备
                var devices = MediaDevice.GetDevices();

                // 通常，你可以选择第一个设备（内部存储）或根据需求选择其他设备
                var device = devices.FirstOrDefault();

                if (device != null)
                {
                    //连接设备
                    device.Connect();

                    //写入路径
                    var remoteFilePath = "内部共享存储空间\\FZD\\RFID_SQLite.db";

                    //如果文件已经存在，删除文件
                    if (device.FileExists(remoteFilePath))
                    {
                        device.DeleteFile(remoteFilePath);
                    }

                    //被复制文件路径
                    var dbPath = Path.Combine(AppContext.BaseDirectory, "RFID_SQLite.db");

                    //复制文件
                    using (FileStream fileStream = File.OpenRead(dbPath))
                    {
                        device.UploadFile(fileStream, remoteFilePath);
                    }

                    device.Disconnect();

                    return true;
                }
                else
                {
                    // 未找到可用的存储设备
                    return false;
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"写入文件时出现异常: {ex.Message}");
                return false;
            }
        }
    }
}
