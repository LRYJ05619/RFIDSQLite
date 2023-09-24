using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Service
{
    public class TitleGetService
    {
        public static string get()
        {
            var parent = Path.GetDirectoryName(AppContext.BaseDirectory);

            var grandparent = Path.GetDirectoryName(parent);

            var filePath = Path.Combine(grandparent, "Title.txt");

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return "";
        }
    }
}
