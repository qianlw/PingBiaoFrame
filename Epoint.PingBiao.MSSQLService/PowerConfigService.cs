using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epoint.PingBiao.MSSQLService
{
    public class PowerConfigService : IPowerConfigService
    {
        public PowerConfig LoadConfig(string configPath)
        {
            return (PowerConfig)SerializeHelper.DeSerialize(typeof(PowerConfig), configPath);
        }

        private static readonly object LockHelper = new object();
        public void SaveConfig(PowerConfig config, string configPath)
        {
            lock (LockHelper)
            {
                SerializeHelper.Serialize(config, configPath);
            }
        }
    }
}
