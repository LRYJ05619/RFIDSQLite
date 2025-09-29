using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;
using System.Text.RegularExpressions;
using RFIDSQLite.Model;

namespace RFIDSQLite.View.PopUp;

public partial class PropertyPage : Popup
{
	public PropertyPage()
	{
		InitializeComponent();
        Color = Colors.Transparent;
        MessagingCenter.Subscribe<PropertyPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }

    //����ֻ���������ֺ�һ��С����
    private void DoubleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string numberPattern = "^[0-9]*(\\.[0-9]*)?$";

        if (e.NewTextValue == null)
            return;

        string text = e.NewTextValue;
        if (!Regex.IsMatch(text, numberPattern))
        {
            // ������Ч����ֹ����
            ((Entry)sender).Text = e.OldTextValue;
        }
    }

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