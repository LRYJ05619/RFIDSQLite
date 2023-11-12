using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using RFIDSQLite.View;
using RFIDSQLite.View.PopUp;
using RFIDSQLite.ViewModel.PopUp;
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
        private List<ProjectSQLite> projectList;
        public List<ProjectSQLite> ProjectList
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
        private List<ProjectSQLite> visibleList;
        public List<ProjectSQLite> VisibleList
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

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ProjectList = await SQLiteService.InitProject();
            });

            UpdateItems();

            //添加项目成功后刷新页面
            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "RefreshProjectPage", async (sender) =>
            {
                ProjectList = await SQLiteService.InitProject();
            });

            //确认删除
            MessagingCenter.Subscribe<DeleteProjectPageViewModel>(this, "DeleteMessage", async (sender) =>
            {
                if (SelectedList.Count == 0)
                {
                    return;
                }

                else
                {
                    foreach (ProjectSQLite selected in SelectedList)
                    {
                        await SQLiteService.RemoveProject(selected.Id);
                    }

                    Thread.Sleep(100);

                    ProjectList = await SQLiteService.InitProject();

                    MessagingCenter.Send(this, "OpenNotifyPage", "删除成功！");
                }
            });
        }

        //接收数据处理
        public async void ReceivedData(object sender, byte[] Data)
        {
            if (Data.Length < 4)
                return;

            switch (Data[3])
            {
                //读回调
                case 0x81:
                    {
                        if (Data.Length == 6)
                        {
                            MessagingCenter.Send(this, "OpenNotifyPage", "读取失败，请检查芯片位置！");
                            return;
                        }

                        string data = "";

                        //回复的第9字节为编码起始，作为长度位
                        for (int i = 0; i < 16; i++)
                        {
                            var fire = Data[i + 9].ToString();
                            fire = fire.Length == 1 ? "0" + fire : fire;
                            data += fire;
                        }

                        var search = await SQLiteService.SearchData(data);

                        if (search != null)
                        {
                            SQLiteService.SearchResult = search;
                            SQLiteService.Project = await SQLiteService.GetProject(search.PrjNum);

                            MessagingCenter.Send(this, "GoToMainPage");
                        }
                        else
                        {
                            MessagingCenter.Send(this, "OpenNotifyPage", "未找到相应数据！");
                        }

                        return;
                    }
            }
        }

        //读取芯片
        [RelayCommand]
        void ReadData()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            if (!RFIDService.ReadData())
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "读取失败！");
            }
        }

        //设备管理
        [RelayCommand]
        void PortManage()
        {
            RFIDService.IsPrj = true;
            MessagingCenter.Send(this, "OpenPortDataPage");
        }

        //搜索
        [RelayCommand]
        async Task HomeAsync()
        {
            ProjectList = await SQLiteService.InitProject();
        }

        //添加项目
        [RelayCommand]
        void AddPrj()
        {
            MessagingCenter.Send(this, "OpenAddProjectPage");
        }

        //Todo 缺少删除和搜索 (已完成)
        //删除
        [RelayCommand]
        void DeleteData()
        {
            if (SelectedList.Count == 0)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "所选项为空！");
                return;
            }

            MessagingCenter.Send(this, "OpenDeletePage");
        }

        //搜索
        [RelayCommand]
        async Task SearchAsync()
        {
            ProjectList = await SQLiteService.SearchProject(SearchQuery);
        }

        //双击编辑事件
        [RelayCommand]
        void DoubleClick(ProjectSQLite todo)
        {
            if (todo == null)
                return;

            SQLiteService.Project = todo;
            
            MessagingCenter.Send(this, "GoToMainPage");
        }

        //更新页码
        private void UpdateItems()
        {
            if (ProjectList == null)
                return;

            int startIndex = (CurrentPageCount - 1) * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, ProjectList.Count);

            var list = new List<ProjectSQLite>();

            for (int i = startIndex; i < endIndex; i++)
            {
                list.Add(ProjectList[i]);
            }

            VisibleList = new List<ProjectSQLite>(list);
        }
    }
}
