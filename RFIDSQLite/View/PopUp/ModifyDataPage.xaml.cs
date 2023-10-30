using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;
using RFIDSQLite.ViewModel;
using System.Text.RegularExpressions;
using RFIDSQLite.Model;
using System;

namespace RFIDSQLite.View.PopUp;

public partial class ModifyDataPage : Popup
{
	public ModifyDataPage(ModifyDataPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;

        MessagingCenter.Subscribe<ModifyDataPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }

    //限制只能输入数字
    private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        string numberPattern = "^[0-9]*(\\.[0-9]*)?$";

        string text = e.NewTextValue;
        if (!Regex.IsMatch(text, numberPattern))
        {
            // 输入无效，阻止更改
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}