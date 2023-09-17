using SQLite;

namespace RFIDSQLite.Model
{
    public class TodoSQLite
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string serial { get; set; }
        public string remark { get; set; }
    }
}
