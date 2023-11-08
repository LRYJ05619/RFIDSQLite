using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Model
{
    public class ProjectSQLite
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SerialLength { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
    }
}
