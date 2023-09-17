using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RFIDSQLite.Model;
using RFIDSQLite.Service;
using RFIDSQLite.ViewModel.PopUp;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;

namespace RFIDSQLite.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        private List<TodoSQLite> todoList;

        public List<TodoSQLite> TodoList
        {
            get { return todoList; }
            set
            {
                if (SetProperty(ref todoList, value))
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

        public MainPageViewModel(IFileSaver fileSaver)
        {
            var RfidService = new RFIDService();
            //订阅接收事件
            RFIDService.ReceivedDataEvent += ReceivedData;

            this.fileSaver = fileSaver;

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

                    var list = await SQLiteService.GetData();
                    list.Reverse();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Id = i + 1;
                    }
                    TodoList = list;
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
                        search.Reverse();
                        for (int i = 0; i < search.Count; i++)
                        {
                            search[i].Id = i + 1;
                        }
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
                    await SQLiteService.AddData(SQLiteService.Serial, SQLiteService.Remark);

                    SearchQuery = SQLiteService.Serial;

                    SQLiteService.BufferRemark = null;
                    SQLiteService.BufferSerial = null;

                    MessagingCenter.Send(this, "OpenNotifyPage", "写入成功！");

                    var list = await SQLiteService.SearchData(SQLiteService.Serial);
                    list.Reverse();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Id = i + 1;
                    }
                    TodoList = list;

                    return;
                //格式43，未修改PC值前
                case 43:
                    MessagingCenter.Send(this, "OpenNotifyPage", "未找到相应数据！");
                    return;
                //读取和写入失败回调
                case 6:
                    MessagingCenter.Send(this, "OpenNotifyPage", "请检查芯片位置！");
                    return;
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

        //读取信息
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
        void WriteFile()
        {
            if (DeviceService.WriteFile())
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "同步完成！");
            }
            else
            {
                MessagingCenter.Send(this, "OpenNotifyPage", "同步失败！");
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
            var list = await SQLiteService.GetData();
            list.Reverse();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Id = i+1;
            }
            TodoList = list;
        }

        //搜索
        [RelayCommand]
        async Task SearchAsync()
        {
            var list = await SQLiteService.SearchData(SearchQuery);
            list.Reverse();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Id = i + 1;
            }
            TodoList = list;
        }
    }
}
