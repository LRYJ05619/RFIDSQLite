using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;
using System.Text.RegularExpressions;
using RFIDSQLite.Model;

namespace RFIDSQLite.View.PopUp;

public partial class AddDataPage : Popup
{
    public AddDataPage()
    {
        InitializeComponent();
        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;
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

    //����ֻ�����븡����
    private void DoubleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        TodoSQLite todo = (TodoSQLite)(((Entry)sender).Parent).BindingContext;
        if(todo.IsNum == false)
            return;

        if(e.NewTextValue == null)
            return;

        string text = e.NewTextValue;

        string numberPattern = "^[0-9]*(\\.[0-9]*)?$";

        if (!Regex.IsMatch(text, numberPattern))
        {
            // ������Ч����ֹ����
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}