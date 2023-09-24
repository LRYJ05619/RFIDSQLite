using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class AddDataPage : Popup
{
    public AddDataPage(AddDataPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        Entry.TextChanged += NumericEntry_TextChanged;

        MessagingCenter.Subscribe<MainPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });

        MessagingCenter.Subscribe<AddDataPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            CloseAsync();
        });
    }

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