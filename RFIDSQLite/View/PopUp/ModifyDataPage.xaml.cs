using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;
using RFIDSQLite.ViewModel;

namespace RFIDSQLite.View.PopUp;

public partial class ModifyDataPage : Popup
{
	public ModifyDataPage(ModifyDataPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;

        Entry.TextChanged += NumericEntry_TextChanged;

        MessagingCenter.Subscribe<ModifyDataPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }

    //限制只能输入数字
    private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        // 获取 Entry 的文本
        string newText = e.NewTextValue;

        // 检查文本是否为空
        if (!string.IsNullOrWhiteSpace(newText))
        {
            // 检查文本中的每个字符
            foreach (char c in newText)
            {
                // 如果字符不是数字，将其从文本中移除
                if (!char.IsDigit(c))
                {
                    Entry.Text = Entry.Text.Remove(Entry.Text.Length - 1);
                    break;
                }
            }
        }
    }
}