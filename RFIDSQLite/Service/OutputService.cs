using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage.Pickers;
using RFIDSQLite.Model;
using Windows.Storage;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;

namespace RFIDSQLite.Service
{
    public class OutputService
    {
        public static async Task SaveCSVFile(CancellationToken cancellationToken, ObservableCollection<object> list)
        {
            try
            {
                // 创建一个 StringBuilder 用于构建 CSV 字符串
                StringBuilder csv = new();

                // 添加 CSV 头部（可选，根据需要）
                csv.AppendLine("Id,设备编号,属性");

                // 遍历 TodoList 并将每个项添加到 CSV 字符串
                foreach (TodoSQLite item in list)
                {
                    csv.AppendLine($"{item.Id},'{item.serial},{item.remark}");
                }

                // 创建包含 CSV 数据的 MemoryStream
                using var stream = new MemoryStream(Encoding.Default.GetBytes(csv.ToString()));

                // 请求权限并选择文件夹进行保存，传入 CancellationToken 以便可以取消操作
                await FileSaver.Default.SaveAsync("导出文件.csv", stream, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 如果操作被取消，则抛出 OperationCanceledException
                throw;
            }
            catch (Exception ex)
            {
                // 处理任何可能的异常
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
