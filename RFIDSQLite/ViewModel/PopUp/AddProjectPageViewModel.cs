using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.ViewModel.PopUp
{
    public partial class AddProjectPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string projectName;

        [ObservableProperty]
        private string projectDescribe;

        [ObservableProperty]
        private ObservableCollection<TodoSQLite> propertyList;

        [ObservableProperty]
        private int serialLength;

        public AddProjectPageViewModel()
        {
            PropertyList = new ObservableCollection<TodoSQLite>();

            if (SQLiteService.BufferProject != null)
            {
                ProjectName = SQLiteService.BufferProject.Name;
                ProjectDescribe = SQLiteService.BufferProject.Description;
                SerialLength = SQLiteService.BufferProject.SerialLength;
            }
            else
            {
                ProjectName = "";
                ProjectDescribe = "";
                SerialLength = 6;
            }
        }

        [RelayCommand]
        void AddProperty()
        {
            if (PropertyList.Count >= 20) { return; }
            PropertyList.Add(new TodoSQLite() { Id = PropertyList.Count + 1 });
        }

        [RelayCommand]
        void DeleteProperty()
        {
            if (PropertyList.Count > 0)
                PropertyList.RemoveAt(PropertyList.Count - 1);
        }

        [RelayCommand]
        async Task SavePropertyAsync()
        {
            //Todo 重写页面意外关闭 (已完成)
            SQLiteService.BufferProject = new ProjectSQList()
            {
                SerialLength = SerialLength,
                Name = ProjectName,
                Description = ProjectDescribe,
            };

            if (SerialLength == null || SerialLength > 30 || SerialLength < 6 || SerialLength % 2 != 0)
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                //Todo 注意编写提示界面反馈 (已完成)
                MessagingCenter.Send(this, "OpenNotifyPage", "创建失败，请重新设置编码长度！");
                return;
            }

            if (ProjectName == null || ProjectName == "")
            {
                MessagingCenter.Send(this, "ClosePopupMessage");
                //Todo 注意编写提示界面反馈 (已完成)
                MessagingCenter.Send(this, "OpenNotifyPage", "创建失败，请设置项目名称！");
                return;
            }

            var prj = new ProjectSQList()
            {
                SerialLength = SerialLength,
                Name = ProjectName,
                Description = ProjectDescribe,
                Time = DateTime.Now.ToShortDateString()
            };

            //Todo 重写保存，先保存项目，再保存属性 (已完成)
            //保存项目
            await SQLiteService.AddProject(prj);

            //保存属性列表
            await SQLiteService.ChangeProperty(PropertyList);

            MessagingCenter.Send(this, "ClosePopupMessage");
            MessagingCenter.Send(this, "OpenNotifyPage", "项目创建成功！");
        }

        [RelayCommand]
        void Cancel()
        {
            MessagingCenter.Send(this, "ClosePopupMessage");
        }
    }
}
