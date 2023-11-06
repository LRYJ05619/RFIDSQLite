using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using RFIDSQLite.View;
using Windows.Storage.Streams;

namespace RFIDSQLite.ViewModel
{
    public partial class ProjectPageViewModel : ObservableObject
    {
        //每页数据数量
        private int ItemsPerPage = 12;

        [ObservableProperty]
        string title;

        //原始数据
        private List<ProjectSQList> projectList;
        public List<ProjectSQList> ProjectList
        {
            get { return projectList; }
            set
            {
                if (SetProperty(ref projectList, value))
                {
                    // 检查列表是否为空
                    if (ProjectList == null || ProjectList.Count == 0)
                    {
                        TotalPages = 1; // 如果为空，设置 TotalPages 为最小页数
                        UpdateItems();
                    }
                    else
                    {
                        //重新计算分页数
                        TotalPages = (int)Math.Ceiling((double)ProjectList.Count / ItemsPerPage);
                        UpdateItems();
                    }
                }
            }
        }

        //绑定数据
        private List<ProjectSQList> visibleList;
        public List<ProjectSQList> VisibleList
        {
            get { return visibleList; }
            set
            {
                if (SetProperty(ref visibleList, value))
                {
                    // 当 TodoList 发生变化时，手动清空 SelectedList
                    SelectedList.Clear();
                }
            }
        }

        [ObservableProperty]
        string searchQuery;

        ObservableCollection<object> selectedList = new();
        public ObservableCollection<object> SelectedList
        {
            get
            {
                return selectedList;
            }
            set
            {
                if (selectedList != value)
                {
                    selectedList = value;
                }
            }
        }

        //当前页码
        private int currentPageCount;
        public int CurrentPageCount
        {
            get => currentPageCount;
            set
            {
                if (SetProperty(ref currentPageCount, value))
                {
                    UpdateItems();
                }
            }
        }

        //总页数
        private int totalPages;

        public int TotalPages
        {
            get => totalPages;
            set
            {
                if (SetProperty(ref totalPages, value))
                {
                    CurrentPageCount = 1;
                }
            }
        }

        public ProjectPageViewModel()
        {
            //获取标题
            Title = TitleGetService.get();

            ProjectList = new List<ProjectSQList>()
            {
                new ProjectSQList() { Name = "项目1", Description = "测试" }
            };

            UpdateItems();
        }

        //双击编辑事件
        [RelayCommand]
        void DoubleClick(ProjectSQList todo)
        {
            if (todo == null)
                return;

            MessagingCenter.Send(this, "GoToMainPage");
        }

        //更新页码
        private void UpdateItems()
        {
            if (ProjectList == null)
                return;

            int startIndex = (CurrentPageCount - 1) * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, ProjectList.Count);

            var list = new List<ProjectSQList>();

            for (int i = startIndex; i < endIndex; i++)
            {
                list.Add(ProjectList[i]);
            }

            VisibleList = new List<ProjectSQList>(list);
        }
    }
}
