using RFIDSQLite.Model;
using SQLite;


namespace RFIDSQLite.Service
{
    public class SQLiteService
    {
        public static string Serial;
        public static string Remark;

        public static string BufferSerial;
        public static string BufferRemark;

        private static SQLiteAsyncConnection Database;

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

            await Database.CreateTableAsync<TodoSQLite>();
        }

        public static async Task AddData(string serial, string remark)
        {
            await Init();
            var data = new TodoSQLite
            {
                serial = serial,
                remark = remark
            };

            await Database.InsertAsync(data);
        }

        public static async Task RemoveData(int id)
        {
            await Init();
            await Database.DeleteAsync<TodoSQLite>(id);
        }

        public static async Task DeleteAllItemsAsync()
        {
            await Init();
            await Database.DeleteAllAsync<TodoSQLite>();
        }

        public static async Task<List<TodoSQLite>> GetData()
        {
            await Init();
            var DataList = await Database.Table<TodoSQLite>().ToListAsync();
            return DataList;
        }

        public static async Task<List<TodoSQLite>> SearchData(string keyword)
        {
            await Init();
            var DataList = await Database
                .Table<TodoSQLite>()
                .Where(t => t.serial.Contains(keyword) || t.remark.Contains(keyword))
                .ToListAsync();
            return DataList;
        }
    }
}
