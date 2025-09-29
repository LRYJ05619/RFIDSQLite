using CommunityToolkit.Maui.Storage;
using RFIDSQLite.View;
using RFIDSQLite.ViewModel;

namespace RFIDSQLite
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage());
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            // 监听窗口真正关闭的事件
            window.Destroying += (s, e) =>
            {
#if WINDOWS
                System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
            };

            return window;
        }
    }

}