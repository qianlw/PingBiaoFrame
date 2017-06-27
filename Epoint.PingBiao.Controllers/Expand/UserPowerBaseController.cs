using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Epoint.PingBiao.Controllers.Expand
{
    public class UserPowerBaseController : UserBaseController
    {
        /// <summary>
        /// 获取已验证用户信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (base.LoginMember == null) {
                filterContext.Result = new RedirectResult("/Store/Login");
            }
        }

        /// <summary>
        /// 执行前读取站点配置信息
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (base.LoginMember == null)
            {
                filterContext.Result = new RedirectResult("/Store/Login");
            }
        }
    }
}
