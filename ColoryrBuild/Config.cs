using Lib.Build;

namespace SendBuild
{
    public class ConfigOBJ
    {
        /// <summary>
        /// 密匙
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string User { get; set; }
    }
    class Config
    {
        public static string FilePath = App.RunLocal + @"MainConfig.json";
        /// <summary>
        /// 读配置文件
        /// </summary>
        public Config()
        {
            App.Config = ConfigSave.Config<ConfigOBJ>(null, FilePath);
        }
        public static void Save()
        {
            ConfigSave.Save(App.Config, FilePath);
        }
    }
}
