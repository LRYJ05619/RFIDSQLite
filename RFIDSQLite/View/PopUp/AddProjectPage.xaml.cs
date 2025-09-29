using RFIDSQLite.Model;
using System.Text.RegularExpressions;
using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;
using RFIDSQLite.ViewModel;

namespace RFIDSQLite.View.PopUp;

public partial class AddProjectPage : Popup
{
	public AddProjectPage()
	{
		InitializeComponent();
        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;
        MessagingCenter.Subscribe<ProjectPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });

        MessagingCenter.Subscribe<AddProjectPageViewModel>(this, "ClosePopupMessage", (sender) =>
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
        if (todo.IsNum == false)
            return;

        if (e.NewTextValue == null)
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