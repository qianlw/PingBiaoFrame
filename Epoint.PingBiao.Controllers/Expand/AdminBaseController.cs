using Autofac;
using Autofac.Integration.Mvc;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.Model.Sys;
using Epoint.PingBiao.Model.Sys.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace Epoint.PingBiao.Controllers.Expand
{
    //后段管理使用
    public class AdminBaseController : BaseController
    {

        //public IUserServer userServer = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IUserServer>();
        public IUserServer userServer = IocHelper.AutofacResolveNamed<IUserServer>("UserServer");
        /// <summary>
        /// 登录用户信息
        /// </summary>
        protected User LoginUser = null;
        protected string m_token = "";

        /// <summary>
        /// 获取已验证用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            var token = CookieHelper.GetCookieValue("Admin");
            if (!string.IsNullOrEmpty(token))
            {
                m_token = token;
                //没有系统缓存重新登录
                this.LoginUser = CacheHelper.GetCache(Constant.CacheKey.LoginAdminInfoCacheKey + "_" + token) as User;
            }
        }

        /// <summary>
        /// 执行前读取站点配置信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //this.Config = CacheHelper.GetCache(Constant.CacheKey.SiteConfigCacheKey) as SiteConfig;
            //if (this.Config == null)
            //{
            //    this.Config = siteConfigService.LoadConfig(Constant.SiteConfigPath);
            //    CacheHelper.SetCache(Constant.CacheKey.SiteConfigCacheKey, Config);
            //}
        }

        /// <summary>
        /// 返回结果前附加数据
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            //if (this.Config != null)
            //{
            //    ViewData["SiteConfig"] = this.Config;
            //}
        }
    }
}
