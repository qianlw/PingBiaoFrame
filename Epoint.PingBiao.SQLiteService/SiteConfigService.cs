using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epoint.PingBiao.SQLiteService
{
    public class SiteConfigService : ISiteConfigService
    {

        private static readonly object LockHelper = new object();
        public void SaveConfig(SiteConfig config, string configPath)
        {
            lock (LockHelper)
            {
                SerializeHelper.Serialize(config, configPath);
            }
        }

        public SiteConfig LoadConfig(string configPath)
        {
            return (SiteConfig)SerializeHelper.DeSerialize(typeof(SiteConfig), configPath);
        }
    }
}
