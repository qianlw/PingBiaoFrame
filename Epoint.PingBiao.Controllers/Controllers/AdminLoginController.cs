using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.Security;
using Epoint.PingBiao.Controllers.Expand;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.Model.Sys.Enums;
using Epoint.PingBiao.Model.Sys;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.Controllers.Filters;
using Epoint.PingBiao.Model.Other;

namespace Epoint.PingBiao.Controllers.Controllers
{

    [Exception]
    public class AdminLoginController : AdminBaseController
    {

        [HttpGet]
        public ActionResult Index()
        {
            if (this.LoginUser != null)
            {
                return Redirect("/Admin/");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Index(string VCode, string UserName, string Password)
        {
            VCode = VCode.ToLower().Trim();
            string name = "Epoint.PingBiao_";
            VCode = name + VCode;
            //最后判断来源
            if (CryptoHelper.MD5Encrypt(VCode) == CookieHelper.GetCookieValue("code"))
            {
                User u = userServer.CheckLogin(UserName.Trim(), Password.Trim());
                if (u == null)
                {
                    return Json(new ResultModel { msg = "用户不存在", pass = false });
                }
                else
                {
                    string sid="Epoint.PingBiao_"+StringHelper.GetGuidString();
                    CookieHelper.SetCookie("Admin", sid);
                    //更新用户登录信息
                    u.LastLoginTime = DateTime.Now;
                    u.LastLoginIp = HttpHelper.GetClientIPAddress();
                    userServer.Update(u);
                    //加入缓存
                    //CacheHelper.SetCache(Constant.CacheKey.LoginAdminInfoCacheKey + "_" + sid, u,new DateTimeOffset().AddDays(1));
                    CacheHelper.SetCache(Constant.CacheKey.LoginAdminInfoCacheKey + "_" + sid, u);
                    //清除验证码code
                    CookieHelper.ClearCookie("code");
                    return Json(new ResultModel { msg = "登陆成功",pass = true });
                }
            }
            else
            {
                return Json(new ResultModel { msg = "验证码错误", pass = false });
            }
        }
        [HttpGet]
        public void VCode()
        {
            string code = "";
            ValidateCode.CreateValidateGraphic(out code, 4, 140, 40, 24);
            string name = "Epoint.PingBiao_";
            code = CryptoHelper.MD5Encrypt(name + code);
            CookieHelper.SetCookie("code", code);
        }
    }
}
