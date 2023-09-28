using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using RFIDSQLite.Service;
using RFIDSQLite.View;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<AddDataPage>();
            builder.Services.AddSingleton<PortsPage>();
            builder.Services.AddSingleton<NotifyPage>();
            builder.Services.AddSingleton<DeletePage>();
            builder.Services.AddSingleton<PropertyPage>();
            builder.Services.AddSingleton<ModifyDataPage>();

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<AddDataPageViewModel>();
            builder.Services.AddSingleton<PortsPageViewModel>();
            builder.Services.AddSingleton<NotifyPageViewModel>();
            builder.Services.AddSingleton<DeletePageViewModel>();
            builder.Services.AddSingleton<PropertyPageViewModel>();
            builder.Services.AddSingleton<ModifyDataPageViewModel>();

            builder.Services.AddTransient<DeviceService>();
            builder.Services.AddTransient<OutputService>();
            builder.Services.AddTransient<RFIDService>();
            builder.Services.AddTransient<SQLiteService>();
            builder.Services.AddTransient<TitleGetService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}