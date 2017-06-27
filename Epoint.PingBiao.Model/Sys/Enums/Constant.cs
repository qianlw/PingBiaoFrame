﻿using System.Collections.Generic;
using System.Web;

namespace Epoint.PingBiao.Model.Sys.Enums
{
    public class Constant
    {

        /// <summary>
        /// 站点配置文件路径
        /// </summary>
        public static string SiteConfigPath = HttpContext.Current.Server.MapPath("/Configuration/SiteConfig.config");
        /// <summary>
        /// 权限配置文件路径
        /// </summary>
        public static string PowerConfigPath = HttpContext.Current.Server.MapPath("/Configuration/PowerConfig.config");
        /// <summary>
        /// 商户权限配置文件路径
        /// </summary>
        public static string StoreActionPath = HttpContext.Current.Server.MapPath("/Configuration/StoreAction.config");
        /// <summary>
        /// 站点缓存键集合
        /// </summary>

        public static class CacheKey
        {
            /// <summary>
            /// 站点信息缓存key
            /// </summary>
            public static string SiteConfigCacheKey = "CACHE_SITE_CONFIG";
            /// <summary>
            /// 权限信息缓存key
            /// </summary>
            public static string PowerConfigCacheKey = "CACHE_POWER_CONFIG";
            /// <summary>
            /// 管理员信息缓存key
            /// </summary>
            public static string LoginAdminInfoCacheKey = "CACHE_LOGIN_ADMIN";
            /// <summary>
            /// 会员信息缓存key
            /// </summary>
            public static string LoginMemberInfoCacheKey = "CACHE_LOGIN_MEMBER";

            public static Dictionary<string, string> List = new Dictionary<string, string>();
            static CacheKey() {
                List.Add(SiteConfigCacheKey, "站点信息缓存");
                List.Add(PowerConfigCacheKey, "权限信息缓存");
                List.Add(LoginAdminInfoCacheKey, "管理员信息缓存");
                List.Add(LoginMemberInfoCacheKey, "会员信息缓存");
            }
        }
    }
}
