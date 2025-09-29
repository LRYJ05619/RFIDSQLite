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

    //����ֻ����������
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
            // ������Ч����ֹ����
            ((Entry)sender).Text = e.OldTextValue;
        }
    }
}