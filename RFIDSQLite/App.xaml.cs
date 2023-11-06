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

            MainPage = new NavigationPage(new MainPage());
        }
    }
}