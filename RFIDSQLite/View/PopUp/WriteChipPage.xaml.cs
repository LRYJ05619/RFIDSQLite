using CommunityToolkit.Maui.Views;
using RFIDSQLite.ViewModel.PopUp;

namespace RFIDSQLite.View.PopUp;

public partial class WriteChipPage : Popup
{
	public WriteChipPage(WriteChipPageViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;

        Entry.TextChanged += NumericEntry_TextChanged;

        // ������Ϣ�������յ� "ClosePopupMessage" ��Ϣʱ��ִ�йر� Popup �Ĳ���
        MessagingCenter.Subscribe<WriteChipPageViewModel>(this, "ClosePopupMessage", (sender) =>
        {
            // �ر� Popup
            CloseAsync();
        });
    }

    //����ֻ����������
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