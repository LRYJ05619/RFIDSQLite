using System;
using System.IO;
using System.Text.Json;

namespace RFIDSQLite.Service
{
    /// <summary>
    /// 配置数据
    /// </summary>
    public class Config
    {
        public bool PasswordVerified { get; set; } = false;
        public string Title { get; set; } = "默认标题";
    }

    /// <summary>
    /// 简单配置服务 - 存储在应用程序目录的 config.json 文件中
    /// </summary>
    public static class SimpleConfigService
    {
        private static Config _config;
        private static readonly string ConfigPath;

        // 静态构造函数：初始化配置文件路径
        static SimpleConfigService()
        {
            var baseDir = AppContext.BaseDirectory;
            var parentDir = Directory.GetParent(baseDir)?.FullName;

            ConfigPath = Path.Combine(parentDir ?? baseDir, "config.json");

            Load();
        }

        /// <summary>
        /// 获取配置文件路径
        /// </summary>
        public static string GetConfigPath() => ConfigPath;

        /// <summary>
        /// 加载配置
        /// </summary>
        private static void Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    _config = JsonSerializer.Deserialize<Config>(json);
                }
                else
                {
                    _config = new Config();
                    Save();
                }
            }
            catch
            {
                _config = new Config();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private static void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_config, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置失败: {ex.Message}");
            }
        }

        // ========== 密码验证 ==========
        public static bool GetPasswordVerified() => _config.PasswordVerified;

        public static void SetPasswordVerified(bool verified)
        {
            _config.PasswordVerified = verified;
            Save();
        }

        public static void ClearPasswordVerified()
        {
            _config.PasswordVerified = false;
            Save();
        }

        // ========== 标题 ==========
        public static string GetTitle() => _config.Title;

        public static void SetTitle(string title)
        {
            _config.Title = title;
            Save();
        }

        // ========== 工具方法 ==========

        /// <summary>
        /// 重置所有配置
        /// </summary>
        public static void Reset()
        {
            _config = new Config();
            Save();
        }

        /// <summary>
        /// 打印配置信息
        /// </summary>
        public static void Print()
        {
            Console.WriteLine($"\n配置文件: {ConfigPath}");
            Console.WriteLine($"密码验证: {_config.PasswordVerified}");
            Console.WriteLine($"标题: {_config.Title}\n");
        }

        /// <summary>
        /// 打开配置文件夹
        /// </summary>
        public static void OpenFolder()
        {
#if WINDOWS
            var folder = Path.GetDirectoryName(ConfigPath);
            System.Diagnostics.Process.Start("explorer.exe", folder);
#endif
        }
    }
}