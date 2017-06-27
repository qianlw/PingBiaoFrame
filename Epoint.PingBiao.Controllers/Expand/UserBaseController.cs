using Autofac;
using Autofac.Integration.Mvc;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.Controllers.Expand;
using Epoint.PingBiao.IService.CMS;
using Epoint.PingBiao.IService.Store;
using Epoint.PingBiao.Model.Sys.Enums;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Epoint.PingBiao.Controllers.Expand
{
    public class UserBaseController : BaseController
    {

        //public IMemberService memberServer = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IMemberService>();
        
        protected Epoint.PingBiao.Model.Store.Member LoginMember = null;

        /// <summary>
        /// 获取已验证用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);
            //var userName = CookieHelper.GetCookieValue("Member");
            //if (!String.IsNullOrEmpty(userName))
            //{
            //    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(userName);
            //    string userId = ticket.UserData;

            //    this.LoginMember = CacheHelper.GetCache(Constant.CacheKey.LoginMemberInfoCacheKey + "_Member_" + userId) as Epoint.PingBiao.Model.Store.Member;
            //    if (this.LoginMember == null)
            //    {

            //        int id = int.Parse(userId);
            //        var member = memberServer.Find(id);
            //        if (member != null)
            //        {
            //            CacheHelper.SetCache(Constant.CacheKey.LoginMemberInfoCacheKey + "_Member_" + member.Member_Id, member);
            //            this.LoginMember = member;
            //        }
            //    }
            //}
        }
       
        /// <summary>
        /// 执行前读取站点配置信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (base.Config.SiteStatus==false) //关闭站点
            {
                filterContext.Result = new RedirectResult("/Error/SiteClose");
            }
            if (this.LoginMember != null)
            {
                ViewData["LoginMember"] = this.LoginMember;
            }
        }

        /// <summary>
        /// 返回结果前附加数据
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}
