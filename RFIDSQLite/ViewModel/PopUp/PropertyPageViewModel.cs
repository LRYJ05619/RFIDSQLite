using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using System.Text.Json;
using RFIDSQLite.Service;
using System.Collections.ObjectModel;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class PropertyPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<TodoSQLite> propertyList;

        public PropertyPageViewModel()
        {
            PropertyList = SQLiteService.Property;
        }

        [RelayCommand]
        void AddProperty()
        {

            PropertyList.Add(new TodoSQLite() { Id = PropertyList.Count + 1 });
        }

        [RelayCommand]
        void DeleteProperty()
        {
            if(PropertyList.Count > 0)
              PropertyList.RemoveAt(PropertyList.Count - 1);
        }

        [RelayCommand]
        void SaveProperty()
        {
            //保存属性列表
            SQLiteService.ChangeProperty(PropertyList);

            MessagingCenter.Send(this, "ClosePopupMessage");
            MessagingCenter.Send(this, "OpenNotifyPage", "保存成功！");
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
