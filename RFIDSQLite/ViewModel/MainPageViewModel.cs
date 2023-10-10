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
        [ObservableProperty]
        private bool isScanning = false;

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

        public MainPageViewModel()
        {
            //获取标签
            Title = TitleGetService.get();

            //初始化属性列表
            _ = SQLiteService.InitProperty();

            //实例化RFIDService，不能删除，否则会接收不到回调
            var RfidService = new RFIDService();

            TodoList = new List<TodoSQLite>();

            //订阅接收事件
            RFIDService.ReceivedDataEvent += ReceivedData;

            MessagingCenter.Subscribe<NotifyPageViewModel>(this, "RefreshPage", async (sender) =>
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
            if (Data.Length < 4) 
                return;

            switch (Data[3])
            {
                //写回调
                case 0x82:
                {
                    if (Data.Length == 6)
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "写入失败，请检查芯片位置！");
                        return;
                    }
                    var length = Data.Length - 4;

                    if (Data[length] == 0x10)
                    {
                        await SQLiteService.AddData();

                        SearchQuery = SQLiteService.Serial;

                        long number = long.Parse(SQLiteService.Serial);
                        number++;
                        SQLiteService.BufferSerial = number.ToString("D12");

                        MessagingCenter.Send(this, "OpenNotifyPage", "写入成功！");

                        TodoList = await SQLiteService.SearchData(SQLiteService.Serial);
                        return;
                    }

                    return;
                }
                //盘存回调
                case 0x89:
                {
                    if (Data.Length == 6)
                    {
                        MessagingCenter.Send(this, "OpenNotifyPage", "扫秒失败，请检查设备连接！");
                        return;
                    }

                    if (Data[1] == 0x0A)
                    {
                        return;
                    }

                    string data = "";

                    for (int i = 0; i < 6; i++)
                    {
                        var fire = Data[i + 7].ToString();
                        fire = fire.Length == 1 ? "0" + fire : fire;
                        data += fire;
                    }

                    TodoList.Add(new TodoSQLite() { serial = data });

                    var originalList = TodoList;

                        // 使用 LINQ 查询来过滤重复的项
                    TodoList = originalList
                        .GroupBy(item => item.serial)
                        .Select(group => group.First())
                        .ToList();

                    for (int i = 0; i < TodoList.Count; i++)
                    {
                        TodoList[i].Id = i + 1;
                    }

                    return;
                }
            }
        }

        //新增
        [RelayCommand]
        void AddData()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }
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

        // 用于更新按钮状态的方法
        private void UpdateScanButtonState(bool isScanning)
        {
            // 在此方法中更新按钮状态
            IsScanning = isScanning;
        }

        private Task scanningTask;
        //扫描
        [RelayCommand]
        void ScanData()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            if (IsScanning)
            {
                // 如果正在扫描，则停止扫描
                IsScanning = false;
                return;
            }

            IsScanning = true;

            TodoList.Clear();

            scanningTask = Task.Run(async () =>
            {
                while (IsScanning)
                {
                    if (!RFIDService.ResetAntenna())
                    {
                        IsScanning = false;
                        MessagingCenter.Send(this, "OpenNotifyPage", "扫描失败！");
                        break;
                    }

                    await Task.Delay(10); // 在新线程中延迟一段时间

                    if (!RFIDService.Inventory())
                    {
                        IsScanning = false;
                        MessagingCenter.Send(this, "OpenNotifyPage", "扫描失败！");
                        break;
                    }

                    await Task.Delay(100); // 在新线程中延迟一段时间
                }

                // 扫描完成后，确保在 UI 线程上更新界面
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsScanning = false;
                });
            });
        }

        //点亮
        [RelayCommand]
         void Light()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            if (SelectedList.Count == 0)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "所选项为空！");
                return;
            }

            foreach (TodoSQLite todo in SelectedList)
            {
                RFIDService.Lock(todo.serial);
                Thread.Sleep(10);

                for (int i = 0; i < 5; i++)
                {
                    RFIDService.Light();
                    Thread.Sleep(10);
                }

                RFIDService.UnLock();
                Thread.Sleep(150);
            }
        }

        //闪烁
        [RelayCommand]
        void Blink()
        {
            if (!RFIDService.serialPort.IsOpen)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "请先打开串口！");
                return;
            }

            if (SelectedList.Count == 0)
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "所选项为空！");
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                foreach (TodoSQLite todo in SelectedList)
                {
                    RFIDService.Lock(todo.serial);
                    Thread.Sleep(10);
                    RFIDService.Light();
                    Thread.Sleep(10);
                    RFIDService.UnLock();
                    Thread.Sleep(150);
                }
                Thread.Sleep(1000);
            }
        }

        //双击编辑事件
        [RelayCommand]
        void DoubleClick(TodoSQLite todo)
        {
            if (todo == null)
                return;

            MessagingCenter.Send(this, "OpenModifyDataPage", todo);
        }
    }
}
