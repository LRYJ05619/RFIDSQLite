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
        private ObservableCollection<TodoIsChange> propertyList;

        [ObservableProperty]
        private ProjectSQLite project;

        public PropertyPageViewModel()
        {
            PropertyList = new ObservableCollection<TodoIsChange>();

            foreach (var todo in SQLiteService.Property)
            {
                PropertyList.Add(new TodoIsChange()
                {
                    Id = todo.Id,
                    IsNum = todo.IsNum,
                    Num = todo.Num,
                    PrjNum = todo.PrjNum,
                    remark = todo.remark,
                    serial = todo.serial,
                    Ischange = false
                });   
            }

            Project = new ProjectSQLite()
            {
                Description = SQLiteService.Project.Description,
                Name = SQLiteService.Project.Name,
                SerialLength = SQLiteService.Project.SerialLength,
                Id = SQLiteService.Project.Id,
                Time = SQLiteService.Project.Time
            };
        }

        [RelayCommand]
        void AddProperty()
        {
            if (PropertyList.Count >= 20) { return; }
            PropertyList.Add(new TodoIsChange() { Ischange = true});
        }

        [RelayCommand]
        void DeleteProperty()
        {
            if (PropertyList.Count > SQLiteService.Property.Count)
                PropertyList.RemoveAt(PropertyList.Count - 1);
        }



        [RelayCommand]
        async Task SavePropertyAsync()
        {
            //Todo 描述和步进量可更改，注意添加(已完成)
            var originList = new ObservableCollection<TodoSQLite>();
            var addList = new ObservableCollection<TodoSQLite>();

            int i = 0;

            foreach (var todo in PropertyList)
            {
                if (i < SQLiteService.Property.Count)
                {
                    originList.Add(new TodoSQLite()
                    {
                        Id = todo.Id,
                        IsNum = todo.IsNum,
                        Num = todo.Num,
                        PrjNum = todo.PrjNum,
                        remark = todo.remark,
                        serial = todo.serial,
                    });
                }
                else
                {
                    addList.Add(new TodoSQLite()
                    {
                        IsNum = todo.IsNum,
                        Num = todo.Num,
                        PrjNum = todo.PrjNum,
                        remark = todo.remark,
                        serial = todo.serial,
                    });
                }
                i++;
            }

            await SQLiteService.UpdateProject(Project);
            await SQLiteService.UpdateProperty(originList);
            await SQLiteService.ChangeProperty(addList);

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
