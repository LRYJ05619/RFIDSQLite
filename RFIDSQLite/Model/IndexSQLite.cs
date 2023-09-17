using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Model
{
    public class IndexSQLite
    {
        public string serial { get; set; }
        public string remark { get; set; }

        // 添加一个属性来存储索引
        public int Index { get; set; }
    }
}
