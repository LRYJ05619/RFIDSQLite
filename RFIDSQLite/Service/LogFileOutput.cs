using RFIDSQLite.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Service
{
    public class LogFileOutput
    {
        public static void OutputLog(List<TodoSQLite> TodoList)
        {
            var parent = Path.GetDirectoryName(AppContext.BaseDirectory);

            var grandparent = Path.GetDirectoryName(parent);

            var filePath = Path.Combine(grandparent, "Logs.txt");

            using (StreamWriter file =
                   new StreamWriter(filePath, true))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(DateTime.Now.ToString());
                foreach (var item in TodoList)
                {
                    stringBuilder.AppendLine($"编码: {item.serial}, 值: {item.remark}");
                }
                file.WriteLine(stringBuilder);
            }
        }
    }
}
