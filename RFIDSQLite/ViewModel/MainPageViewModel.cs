using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using RFIDSQLite.ViewModel.PopUp;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using System;

namespace RFIDSQLite.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private bool ReadState = false;

        //每页数据数量
        private int ItemsPerPage = 12;

        [ObservableProperty]
        string title;
        
        //原始数据
        private List<TodoSQLite> todoList;
        public List<TodoSQLite> TodoList
        {
            get { return todoList; }
            set
            {
                if (SetProperty(ref todoList, value))
                {
                    // 检查列表是否为空
                    if (TodoList == null || TodoList.Count == 0)
                    {
                        TotalPages = 1; // 如果为空，设置 TotalPages 为最小页数
                        UpdateItems();
                    }
                    else
                    {
                        //重新计算分页数
                        TotalPages = (int)Math.Ceiling((double)TodoList.Count / ItemsPerPage);
                        UpdateItems();
                    }
                }
            }
        }

        //绑定数据
        private List<TodoSQLite> visibleList;
        public List<TodoSQLite> VisibleList
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

        readonly IFileSaver fileSaver;

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


        public MainPageViewModel(IFileSaver fileSaver)
        {
            //获取标签
            Title = TitleGetService.get();

            //初始化属性列表
            _ = SQLiteService.InitProperty();

            TotalPages = 1;
            CurrentPageCount = 1;

            var RfidService = new RFIDService();
            //订阅接收事件
            RFIDService.ReceivedDataEvent += ReceivedData;

            this.fileSaver = fileSaver;

            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "RefreshPage", async (sender) =>
            {
                await HomePageAsync();
            });

            MessagingCenter.Subscribe<AddDataPageViewModel>(this, "RefreshPage", async (sender) =>
            {
                await HomePageAsync();
            });

            MessagingCenter.Subscribe<DeletePageViewModel>(this, "DeleteMessage", async (sender) =>
            {
                if(SelectedList.Count == 0)
                {
                    MessagingCenter.Send(this, "OpenNotifyPage", "所选项为空！");
                }
                else
                {
                    foreach (TodoSQLite selected in SelectedList)
                    {
                        var result = await SQLiteService.SearchData(selected.serial);
                        foreach (TodoSQLite delete in result)
                        {
                            await SQLiteService.RemoveData(delete.Id);
                        }
                    }

                    TodoList = await SQLiteService.GetData();
                    MessagingCenter.Send(this, "OpenNotifyPage", "删除成功！");
                }
            });
        }

        //接收数据处理
        private async void ReceivedData(object sender, byte[] Data)
        {
            switch (Data.Length)
            {
                //格式47，标准格式,读数据库判断是否存在
                case 47:
                    string data = "";

                    for (int i = 0; i < 6; i++)
                    {
                        var fire = Data[i + 9].ToString();
                        fire = fire.Length == 1 ? "0" + fire : fire;
                        data += fire;
                    }

                    var search = await SQLiteService.SearchData(data);

                    if (search.Count != 0)
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "读取成功！");
                        TodoList = search;
                    }
                    else
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "未找到相应数据！");
                    }

                    SearchQuery = data;
                    return;

                //格式31，写入成功回调
                case 31:

                    SearchQuery = SQLiteService.Serial;

                    //序号自增
                    long number = long.Parse(SQLiteService.Serial);
                    number++;
                    SQLiteService.WriteSerial = number.ToString("D12");

                    await SQLiteService.SignData();

                    MessagingCenter.Send(this, "OpenNotifyPage", "写入成功！");

                    TodoList = await SQLiteService.SearchData(SQLiteService.Serial);

                    return;

                //格式43，未修改PC值前
                case 43:
                    MessagingCenter.Send(this, "OpenNotifyPage", "未找到相应数据！");
                    return;

                //读取和写入失败回调
                case 6:
                    if (ReadState == true)
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "读取失败，请检查芯片位置！");
                    }
                    else
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查芯片位置！");
                    }
                    return;
            }
        }

        //新增
        [RelayCommand]
        void AddData()
        {
            MessagingCenter.Send(this, "OpenAddDataPage");
        }

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

        //读取芯片
        [RelayCommand]
        void ReadData()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            ReadState = true;

            if (!RFIDService.ReadData())
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "读取失败！");
            }
        }

        //写入芯片
        [RelayCommand]
        void WriteChip()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            MessagingCenter.Send(this, "OpenWriteChipPage");
        }

        //数据导出
        [RelayCommand]
        async Task OutputCsvAsync()
        {
            if (SelectedList.Count == 0)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "所选项为空！");
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // 调用 SaveCSVFile 方法，并传入 CancellationToken
                await OutputService.SaveCSVFile(cancellationTokenSource.Token, SelectedList);
            }
            catch (OperationCanceledException)
            {
                // 处理操作被取消的情况
                MessagingCenter.Send(this, "OpenNotifyPage", "取消导出！");
            }
            finally
            {
                // 释放 CancellationTokenSource 资源
                cancellationTokenSource.Dispose();
            }
        }

        //数据同步
        [RelayCommand]
        async Task WriteFileAsync()
        {
            await SQLiteService.Database.CloseAsync();

            if (DeviceService.ReplaceFile())
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "同步完成！");
            }
            else
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "同步失败，请重新连接设备");
            }
        }

        //设备管理
        [RelayCommand]
        void PortManage()
        {
            MessagingCenter.Send(this, "OpenPortDataPage");
        }

        //主页
        [RelayCommand]
        async Task HomePageAsync()
        {
            TodoList = await SQLiteService.GetData();
        }

        //待写芯片
        [RelayCommand]
        async Task GetUnsignDataAsync()
        {
            TodoList = await SQLiteService.GetUnsignData();
        }

        //搜索
        [RelayCommand]
        async Task SearchAsync()
        {
            TodoList = await SQLiteService.SearchData(SearchQuery);
        }

        //属性管理按钮
        [RelayCommand]
        void Property()
        {
            MessagingCenter.Send(this, "OpenManagerPage");
        }

        //双击编辑事件
        [RelayCommand]
        void DoubleClick(TodoSQLite todo)
        {
            if (todo == null)
                return;

            MessagingCenter.Send(this, "OpenModifyDataPage", todo);
        }

        private void UpdateItems()
        {
            if (TodoList == null)
                return;

            int startIndex = (CurrentPageCount - 1) * ItemsPerPage;
            int endIndex = Math.Min(startIndex + ItemsPerPage, TodoList.Count);

            var list = new List<TodoSQLite>();

            for (int i = startIndex; i < endIndex; i++)
            {
                list.Add(TodoList[i]);
            }

            VisibleList = new List<TodoSQLite>(list);
        }
    }
}
