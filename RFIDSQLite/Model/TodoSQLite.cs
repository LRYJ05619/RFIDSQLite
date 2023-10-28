using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace RFIDSQLite.Model
{
    public class TodoSQLite : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string serial { get; set; }
        public string remark { get; set; }

        private bool isNum;
        public bool IsNum
        {
            get => isNum;
            set => SetProperty(ref isNum, value);
        }

        public double Num { get; set; }
    }
}
