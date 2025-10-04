using CommunityToolkit.Maui.Storage;
using RFIDSQLite.View;
using RFIDSQLite.ViewModel;
using RFIDSQLite.Service;

namespace RFIDSQLite
{
    public partial class App : Application
    {
        public static string NewTitle { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage());

            // 在应用启动时初始化设备监控
            InitializeDeviceMonitoring();
        }

        private void InitializeDeviceMonitoring()
        {
            try
            {
                // 启动设备监控
                RFIDService.StartDeviceMonitoring();

                // 尝试自动连接已存在的目标设备
                Task.Run(async () =>
                {
                    // 延迟一小段时间，确保应用完全启动
                    await Task.Delay(500);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        bool connected = RFIDService.AutoConnectTargetDevice();
                        if (connected)
                        {
                            Console.WriteLine("应用启动时成功自动连接到RFID设备");
                        }
                        else
                        {
                            Console.WriteLine("应用启动时未找到RFID设备，将等待设备插入");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化设备监控时出现异常: {ex.Message}");
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // 监听窗口真正关闭的事件
            window.Destroying += (s, e) =>
            {
                // 停止设备监控
                RFIDService.StopDeviceMonitoring();

                // 关闭串口
                if (RFIDService.serialPort != null && RFIDService.serialPort.IsOpen)
                {
                    try
                    {
                        RFIDService.serialPort.Close();
                    }
                    catch { }
                }

#if WINDOWS
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
            };

            return window;
        }
    }
}