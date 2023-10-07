using System.Collections.ObjectModel;
using RFIDSQLite.Model;
using SQLite;


namespace RFIDSQLite.Service
{
    public class SQLiteService
    {
        public static string Serial;
        public static ObservableCollection<TodoSQLite> Property;

        public static string BufferSerial;
        public static ObservableCollection<TodoSQLite> BufferProperty;

        public static SQLiteAsyncConnection Database;

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        static async Task Init()
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

        public static async Task InitProperty()
        {
            await Init();
            await Database.CreateTableAsync<TodoSQLite>();

            var todoList = await Database.Table<TodoSQLite>().ToListAsync();

            Property = new ObservableCollection<TodoSQLite>(todoList);
        }

        public static async Task ChangeProperty(ObservableCollection<TodoSQLite> Attributes)
        {
            if (Attributes != null && Attributes.Count > 0)
            {
                await Init();

                await Database.CreateTableAsync<TodoSQLite>();

                // 删除旧的 Property 表格
                await Database.DropTableAsync<TodoSQLite>();

                //覆盖新的 Property 表格
                await Database.CreateTableAsync<TodoSQLite>();

                foreach (var attribute in Attributes)
                {
                    // 使用 InsertAsync 方法将 Property 对象插入到数据库表格中
                    await Database.InsertAsync(attribute);
                }
            }
            await Database.CloseAsync();
        }

        public static async Task AddData()
        {
            await Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var data = new MultiattributeSQLite
            {
                serial = Serial,
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

        public static async Task RemoveData(int id)
        {
            await Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            await Database.DeleteAsync<MultiattributeSQLite>(id);
            await Database.CloseAsync();
        }

        public static async Task UpdateData(string serial, ObservableCollection<TodoSQLite> attributes)
        {
            await Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var todo = await Database
                .Table<MultiattributeSQLite>().
                Where(t => t.serial == serial).FirstOrDefaultAsync();

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

        public static async Task<List<TodoSQLite>> GetData()
        {
            await Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database.Table<MultiattributeSQLite>().ToListAsync();
            var TodoList = Translate(MultiList);
            await Database.CloseAsync();
            return TodoList;
        }

        public static async Task<List<TodoSQLite>> SearchData(string keyword)
        {
            await Init();
            await Database.CreateTableAsync<MultiattributeSQLite>();

            var MultiList = await Database
                .Table<MultiattributeSQLite>()
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
            return TodoList;
        }

        private static List<TodoSQLite> Translate(List<MultiattributeSQLite> Multi)
        {
            List<TodoSQLite> todo = new();

            foreach (var data in Multi)
            {
                string value = "";

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
                    serial = data.serial,
                    remark = value,
                });
            }

            todo.Reverse();
            for (int i = 0; i < todo.Count; i++)
            {
                todo[i].Id = i + 1;
            }

            return todo;
        }
    }
}
