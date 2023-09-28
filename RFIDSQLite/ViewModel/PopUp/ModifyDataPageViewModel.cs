using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using System.Collections.ObjectModel;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class ModifyDataPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        string serial;

        [ObservableProperty]
        ObservableCollection<TodoSQLite> attributes;

        public ModifyDataPageViewModel(TodoSQLite todo)
        {
            Attributes = SQLiteService.Property;

            Serial = todo.serial;
            string[] parts = todo.remark.Split('；');

            // 计算要循环的次数，取两者中较小的值，以确保不会出现索引越界
            int count = Math.Min(parts.Length, Attributes.Count);

            for (int i = 0; i < count; i++)
            {
                // 获取当前部分并创建一个新的 TodoSQLite 对象
                Attributes[i].remark = parts[i];
            }
        }

        [RelayCommand]
        async Task UpdateAsync()
        {
            await SQLiteService.UpdateData(Serial, Attributes);

            MessagingCenter.Send(this, "ClosePopupMessage");
            MessagingCenter.Send(this, "OpenNotifyPage","修改成功！");
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
