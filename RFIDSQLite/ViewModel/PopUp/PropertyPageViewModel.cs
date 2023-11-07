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

        [ObservableProperty]
        private ProjectSQList project;

        public PropertyPageViewModel()
        {
            PropertyList = new ObservableCollection<TodoSQLite>(SQLiteService.Property);

            Project = new ProjectSQList()
            {
                Description = SQLiteService.Project.Description,
                Name = SQLiteService.Project.Name,
                SerialLength = SQLiteService.Project.SerialLength,
                Id = SQLiteService.Project.Id,
                Time = SQLiteService.Project.Time
            };
        }

        [RelayCommand]
        async Task SavePropertyAsync()
        {
            //Todo 描述和步进量可更改，注意添加(已完成)
            await SQLiteService.UpdateProject(Project);
            await SQLiteService.UpdateProperty(PropertyList);

            await SQLiteService.InitProperty();

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
