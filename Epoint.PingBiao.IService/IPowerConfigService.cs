using Epoint.PingBiao.Model.Sys;

namespace Epoint.PingBiao.IService
{
    public interface IPowerConfigService
    {
        /// <summary>
        /// 加载权限配置文件
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        PowerConfig LoadConfig(string configPath);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="config"></param>
        /// <param name="configPath"></param>
        void SaveConfig(PowerConfig config, string configPath);
    }
}
