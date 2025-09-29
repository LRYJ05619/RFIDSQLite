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

    //限制只能输入数字和一个小数点
    private void DoubleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string numberPattern = "^[0-9]*(\\.[0-9]*)?$";

        if (e.NewTextValue == null)
            return;

        string text = e.NewTextValue;
        if (!Regex.IsMatch(text, numberPattern))
        {
            // 输入无效，阻止更改
            ((Entry)sender).Text = e.OldTextValue;
        }
    }

    private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // 正则表达式，只允许输入数字
        string newText = Regex.Replace(e.NewTextValue, "[^0-9]", "");

        if (newText != e.NewTextValue)
        {
            entry.Text = newText; // 如果输入不是数字，则清除非数字字符
        }
    }
}