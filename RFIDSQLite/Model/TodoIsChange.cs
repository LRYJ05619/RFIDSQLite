using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Model
{
    public class TodoIsChange : ObservableObject
    {
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

        public int PrjNum { get; set; }
        public bool Ischange { get; set; }
    }
}
