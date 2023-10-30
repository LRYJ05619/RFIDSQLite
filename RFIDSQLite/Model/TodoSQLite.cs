using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;

namespace RFIDSQLite.Model
{
    public class TodoSQLite : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string serial { get; set; }

        private string mark;

        public string remark
        {
            get => mark;
            set => SetProperty(ref mark, value);
        }

        private bool isNum;
        public bool IsNum
        {
            get => isNum;
            set => SetProperty(ref isNum, value);
        }

        public decimal Num { get; set; }
    }
}
