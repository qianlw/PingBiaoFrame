﻿using Autofac;
using Autofac.Integration.Mvc;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.Model.Sys;
using Epoint.PingBiao.Model.Sys.Enums;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Epoint.PingBiao.Controllers.Expand
{
    //基础站点信息
    public class BaseController : Controller
    {

        public ISiteConfigService siteConfigService = IocHelper.AutofacResolveNamed<ISiteConfigService>("SiteConfigService");

        /// <summary>
        /// 站点配置信息
        /// </summary>
        public SiteConfig Config = null;

        /// <summary>
        /// 执行前读取站点配置信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Config = CacheHelper.GetCache(Constant.CacheKey.SiteConfigCacheKey) as SiteConfig;
            if (this.Config == null)
            {
                this.Config = siteConfigService.LoadConfig(Constant.SiteConfigPath);
                CacheHelper.SetCache(Constant.CacheKey.SiteConfigCacheKey, Config);
            }
        }

        /// <summary>
        /// 返回结果前附加数据
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (this.Config != null)
            {
                ViewData["SiteConfig"] = this.Config;
            }
        }

    }
}
