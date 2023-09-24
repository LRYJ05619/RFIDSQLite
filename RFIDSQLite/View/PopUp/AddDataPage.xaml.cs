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
        // ��ȡ Entry ���ı�
        string newText = e.NewTextValue;

        // ����ı��Ƿ�Ϊ��
        if (!string.IsNullOrWhiteSpace(newText))
        {
            // ����ı��е�ÿ���ַ�
            foreach (char c in newText)
            {
                // ����ַ��������֣�������ı����Ƴ�
                if (!char.IsDigit(c))
                {
                    Entry.Text = Entry.Text.Remove(Entry.Text.Length - 1);
                    break;
                }
            }
        }
    }
}