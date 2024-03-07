using System;
using System.Collections.ObjectModel;
using Microsoft.Maui;
using RFIDSQLite.Model;
using RFIDSQLite.View.PopUp;
using SQLite;


namespace RFIDSQLite.Service
{
    public class SQLiteService
    {
        public static ProjectSQLite BufferProject;

        public static MultiattributeSQLite SearchResult;

        //Todo 注意填充 (已完成)
        public static ProjectSQLite Project;
        public static int PrjNum;
        public static int SerialLength;

        public static string Serial;
        public static ObservableCollection<TodoSQLite> Property;

        public static string BufferSerial;
        public static ObservableCollection<TodoSQLite> BufferProperty;

        public static string WriteSerial;

        public static SQLiteAsyncConnection Database;

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            //全双工
            SQLite.SQLiteOpenFlags.FullMutex |
            //共享缓存enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        //数据库初始化
        static void Init()
        {
            if (Database != null)
                return;

            var dbPath = AppContext.BaseDirectory;

            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            // Get an absolute path to the database file
            var databasePath = Path.Combine(dbPath, "RFID_SQLite.db");

            Database = new SQLiteAsyncConnection(databasePath, Flags);
        }

        //初始化项目列表
        public static async Task<List<ProjectSQLite>> InitProject()
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            var prjList = await Database.Table<ProjectSQLite>().ToListAsync();

            await Database.CloseAsync();

            return prjList;
        }

        //新增项目
        public static async Task AddProject(ProjectSQLite prj)
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            await Database.InsertAsync(prj);
            PrjNum = prj.Id;
            await Database.CloseAsync();
        }

        //删除项目
        public static async Task RemoveProject(int id)
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            await Database.DeleteAsync<ProjectSQLite>(id);
            await Database.CloseAsync();
        }

        //修改项目
        public static async Task UpdateProject(ProjectSQLite prj)
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            await Database.UpdateAsync(prj);
            await Database.CloseAsync();
        }

        //通过Id获取项目
        public static async Task<ProjectSQLite> GetProject(int id)
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            var prj = await Database
                .Table<ProjectSQLite>()
                .Where(t => t.Id.Equals(id))
                .FirstOrDefaultAsync();

            await Database.CloseAsync();

            return prj;
        }

        //搜索项目
        public static async Task<List<ProjectSQLite>> SearchProject(string keyword)
        {
            Init();
            await Database.CreateTableAsync<ProjectSQLite>();

            var prj = await Database
                .Table<ProjectSQLite>()
                .Where(t => t.Description.Contains(keyword)
                            || t.Name.Contains(keyword)
                            || t.Time.Contains(keyword))
                .ToListAsync();

            await Database.CloseAsync();

            return prj;
        }

        //Todo 重写为检索项目ID (已完成)
        //初始化属性列表
        public static async Task InitProperty()
        {
            Init();
            await Database.CreateTableAsync<TodoSQLite>();

            var todoList = await Database
                .Table<TodoSQLite>()
                .Where(t => t.PrjNum.Equals(PrjNum))
                .ToListAsync();

            await Database.CloseAsync();

            Property = new ObservableCollection<TodoSQLite>(todoList);
        }

        //Todo 不再更改属性列表，只做添加处理 (已完成)
        //更改属性列表
        public static async Task ChangeProperty(ObservableCollection<TodoSQLite> Attributes)
        {
            if (Attributes != null && Attributes.Count > 0)
            {
                Init();
                await Database.CreateTableAsync<TodoSQLite>();

                foreach (var attribute in Attributes)
                {
                    attribute.PrjNum = PrjNum;
                    // 使用 InsertAsync 方法将 Property 对象插入到数据库表格中
                    await Database.InsertAsync(attribute);
                }
            }
            await Database.CloseAsync();
        }

        //Todo 更改属性列表 (修改步进量)
        public static async Task UpdateProperty(ObservableCollection<TodoSQLite> Attributes)
        {
            if (Attributes != null && Attributes.Count > 0)
            {
                Init();
                await Database.CreateTableAsync<TodoSQLite>();

                foreach (var attribute in Attributes)
                {
                    // 更改属性列表
                    await Database.UpdateAsync(attribute);
                }
            }
            await Database.CloseAsync();
        }

        //Todo 添加上项目Id属性 (已完成)
        //新增条目
        public static async Task AddData()
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var data = new MultiattributeSQLite
            {
                serial = TranSerial(Serial),
                iswrite = 0,
                PrjNum = PrjNum,
            };

            for (int i = 0; i < Property.Count && i < 20; i++)
            {
                var propertyName = $"property{i + 1}"; // 构造属性名称，例如 property1, property2, ...
                var propertyInfo = typeof(MultiattributeSQLite).GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    // 获取 Attributes[i] 的 value 属性值
                    var value = Property[i].remark;

                    // 使用反射设置 Multiattribute 的属性值
                    propertyInfo.SetValue(data, value);
                }
            }

            await Database.InsertAsync(data);
            await Database.CloseAsync();
        }

        //删除条目
        public static async Task RemoveData(int id)
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            await Database.DeleteAsync<MultiattributeSQLite>(id);
            await Database.CloseAsync();
        }

        //更改条目
        public static async Task UpdateData(string serial, ObservableCollection<TodoSQLite> attributes)
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var target = TranSerial(serial);

            var todo = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.serial.Equals(target))
                .FirstOrDefaultAsync();

            for (int i = 0; i < 20; i++)
            {
                var propertyName = $"property{i + 1}"; // 构造属性名称，例如 property1, property2, ...
                var propertyInfo = typeof(MultiattributeSQLite).GetProperty(propertyName);

                if (i < Property.Count)
                {
                    if (propertyInfo != null)
                    {
                        // 获取 Attributes[i] 的 value 属性值
                        var value = attributes[i].remark;

                        // 使用反射设置 Multiattribute 的属性值
                        propertyInfo.SetValue(todo, value);
                    }
                }
                else
                {
                    propertyInfo.SetValue(todo, "");
                }
            }
            await Database.UpdateAsync(todo);
            await Database.CloseAsync();
        }

        //Todo 需要添加项目Id (已完成)
        //获取项目的所有条目
        public static async Task<List<TodoSQLite>> GetData()
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.PrjNum.Equals(PrjNum))
                .ToListAsync();
            var TodoList = Translate(MultiList);

            TodoList.Reverse();
            for (int i = 0; i < TodoList.Count; i++)
            {
                TodoList[i].Id = i + 1;
            }

            await Database.CloseAsync();
            return TodoList;
        }

        //Todo 所有项目
        //搜索条目，所有项目
        public static async Task<MultiattributeSQLite> SearchData(string keyword)
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.serial.Equals(keyword))
                .FirstOrDefaultAsync();

            await Database.CloseAsync();

            if (MultiList == null)
            {
                return null;
            }
            return MultiList;
        }

        //Todo 需要添加项目Id限制 (已完成)
        //搜索条目单一项目
        public static async Task<List<TodoSQLite>> SearchDataInPrj(string keyword)
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.PrjNum.Equals(PrjNum))
                .Where(t => t.serial.Contains(keyword) 
                            || t.property1.Contains(keyword)
                            || t.property2.Contains(keyword)
                            || t.property3.Contains(keyword)
                            || t.property4.Contains(keyword)
                            || t.property5.Contains(keyword)
                            || t.property6.Contains(keyword)
                            || t.property7.Contains(keyword)
                            || t.property8.Contains(keyword)
                            || t.property9.Contains(keyword)
                            || t.property10.Contains(keyword)
                            || t.property11.Contains(keyword)
                            || t.property12.Contains(keyword)
                            || t.property13.Contains(keyword)
                            || t.property14.Contains(keyword)
                            || t.property15.Contains(keyword)
                            || t.property16.Contains(keyword)
                            || t.property17.Contains(keyword)
                            || t.property18.Contains(keyword)
                            || t.property19.Contains(keyword)
                            || t.property20.Contains(keyword))
                .ToListAsync();
            var TodoList = Translate(MultiList);
            await Database.CloseAsync();

            TodoList.Reverse();
            for (int i = 0; i < TodoList.Count; i++)
            {
                TodoList[i].Id = i + 1;
            }

            return TodoList;
        }

        //Todo 重写为查找编码
        public static async Task<List<TodoSQLite>> CheckSerial(string keyword)
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var str = TranSerial(keyword);

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.serial.Equals(str))
                .ToListAsync();

            var TodoList = Translate(MultiList);

            await Database.CloseAsync();

            return TodoList;
        }

        //标记条目已写入芯片
        public static async Task SignData()
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var str = TranSerial(Serial);

            var todo = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.serial.Equals(str))
                .FirstOrDefaultAsync();

            todo.iswrite = 1;

            await Database.UpdateAsync(todo);
            await Database.CloseAsync();
        }

        //Todo 添加项目Id限制 (已完成)
        //获取所有未被标记的条目
        public static async Task<List<TodoSQLite>> GetUnsignData()
        {
            Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
                .Where(t => t.PrjNum.Equals(PrjNum))
                .Where(t => t.iswrite.Equals(0))
                .ToListAsync();

            var TodoList = Translate(MultiList);

            TodoList.Reverse();
            for (int i = 0; i < TodoList.Count; i++)
            {
                TodoList[i].Id = i + 1;
            }

            await Database.CloseAsync();
            return TodoList;
        }

        //将MultiattributeSQLite转换为TodoSQLite
        private static List<TodoSQLite> Translate(List<MultiattributeSQLite> Multi)
        {
            List<TodoSQLite> todo = new();

            foreach (var data in Multi)
            {
                string value = "";

                int length = Convert.ToInt32(data.serial.Substring(0, 2));
                length = length * 2;

                var serial = data.serial.Substring(4, length);

                for (int i = 0; i < Property.Count; i++)
                {
                    var propertyName = $"property{i + 1}"; // 构造属性名称，例如 property1, property2, ...
                    var propertyInfo = typeof(MultiattributeSQLite).GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        // 使用反射获取 Multiattribute 的属性值
                        var PropertyValue = propertyInfo.GetValue(data);

                        value = value + PropertyValue + "；";
                    }
                }

                todo.Add(new TodoSQLite()
                {
                    Id = data.Id,
                    serial = serial,
                    remark = value,
                    PrjNum = PrjNum,
                });
            }

            return todo;
        }

        //将编码转换为标准格式 长度+项目Id+编码
        private static string TranSerial(string str)
        {
            var length = (SerialLength / 2) < 10 ? ("0" + (SerialLength / 2)) : (SerialLength / 2).ToString();
            var prj = PrjNum < 10 ? "0" + PrjNum : PrjNum.ToString();

            var serial = length + prj + str;

            serial = serial.PadRight(32, '0');

            return serial;
        }
    }
}
