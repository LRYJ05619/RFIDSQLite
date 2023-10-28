using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;
using System.Text.RegularExpressions;

namespace RFIDSQLite.View.PopUp;

public partial class AddDataPage : Popup
{
    public AddDataPage(AddDataPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<MainPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });

        MessagingCenter.Subscribe<AddDataPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }


    //����ֻ����������
    private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // ������ʽ��ֻ������������
        string newText = Regex.Replace(e.NewTextValue, "[^0-9]", "");

        if (newText != e.NewTextValue)
        {
            entry.Text = newText; // ������벻�����֣�������������ַ�
        }
    }
}