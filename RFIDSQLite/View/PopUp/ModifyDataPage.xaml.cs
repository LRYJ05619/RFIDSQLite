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
        Color = Colors.Transparent;
        CanBeDismissedByTappingOutsideOfPopup = false;
        MessagingCenter.Subscribe<ModifyDataPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }

    //限制只能输入数字
    private void DoubleEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        TodoSQLite todo = (TodoSQLite)(((Entry)sender).Parent).BindingContext;
        if (todo.IsNum == false)
            return;

        if (e.NewTextValue == null)
            return;

        string numberPattern = "^[0-9]*(\\.[0-9]*)?$";

        string text = e.NewTextValue;
        if (!Regex.IsMatch(text, numberPattern))
        {
            // 输入无效，阻止更改
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}