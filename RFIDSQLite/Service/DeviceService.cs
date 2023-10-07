using MediaDevices;
using Microsoft.Maui.Controls;

namespace RFIDSQLite.Service
{
    public class DeviceService
    {
        private static bool isTry = false;
        public static bool ReplaceFile()
        {
            if (isTry) return false;

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

                    var subdirectories = device.GetDirectories("/");
                    var RootPath = subdirectories.First().TrimStart('\\');

                    var remotePath = $"{RootPath}/Android/data/com.companyname.rfid_android";

                    var remoteFilePath = $"{RootPath}/Android/data/com.companyname.rfid_android/RFID_SQLite.db";

                    if (!device.DirectoryExists(remotePath))
                    {
                        device.CreateDirectory(remotePath);
                    }

                    //如果文件已经存在，删除文件
                    if (device.FileExists(remoteFilePath))
                    {
                        device.DeleteFile(remoteFilePath);
                        isTry = true;
                        return false;
                    }

                    //被复制文件路径
                    var dbPath = Path.Combine(AppContext.BaseDirectory, "RFID_SQLite.db");

                    //复制文件
                    using (FileStream fileStream = File.OpenRead(dbPath))
                    {
                        device.UploadFile(fileStream, remoteFilePath);
                    }

                    device.Disconnect();
                    isTry = true;
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
                Console.WriteLine($"写入文件时出现异常: {ex.Message}");
                return false;
            }
        }
    }

}



