using Autofac;
using Autofac.Integration.Mvc;
using Cactus.Controllers.Expand;
using Cactus.Controllers.Filters;
using System;
using System.Web.Mvc;
using Cactus.IService.Store;
using Cactus.Model.Other;
using Cactus.IService.CMS;
using System.Web.Security;
using Cactus.Common;
using Cactus.Model.Sys.Enums;

namespace Cactus.Controllers.Controllers
{
    [Exception]
    public class StoreController : UserBaseController
    {
        /// <summary>
        /// 商品
        /// </summary>
        public IItemService itemService = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IItemService>();
        /// <summary>
        /// 文章
        /// </summary>
        public IArticleService articleServer = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IArticleService>();

        public ActionResult Index()
        {
            //推荐
            //普通列表
            int count = 0;
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 8 };
            pageturn.PageIndex = 1;
            var list = this.itemService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, " where State=1", "AddTime DESC", out  count);
            pageturn.CountSize = count;
            ViewData["ItemList"] = list;
            var flaglist = this.itemService.ToPagedList(pageturn.PageIndex.Value, 3, " where Flag=1 and State=1", "AddTime DESC", out  count);
            ViewData["ItemFlagList"] = flaglist;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Login(string user,string pwd) 
        {
            if (base.LoginMember != null)
            {
                return Redirect("/User/Index");
            }
            Cactus.Model.Store.Member member=base.memberServer.CheckLogin(user, pwd);
            if (member != null)
            {
                var cookie = FormsAuthentication.GetAuthCookie("Member", false);
                var ticket = FormsAuthentication.Decrypt(cookie.Value);               
                var newTicket = new FormsAuthenticationTicket(ticket.Version,
                    ticket.Name,
                    DateTime.Now,
                     DateTime.Now.AddDays(1),
                    ticket.IsPersistent, member.Member_Id.ToString());
                cookie.Value = FormsAuthentication.Encrypt(newTicket);
                cookie.Name = "Member";

                base.memberServer.UpdateLastTime(member.Member_Id,DateTime.Now);
                HttpContext.Response.Cookies.Add(cookie);
                CacheHelper.SetCache(Constant.CacheKey.LoginMemberInfoCacheKey + "_Member_" + member.Member_Id, member);
                return Redirect("/User/Index");    
            }
            else {
                ViewData["LoginError"] = "登录失败";
            }
            return View();
        }
        [HttpGet]
        public ActionResult Register() {
            if (base.LoginMember != null)
            {
                return Redirect("/User/Index");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Register(string username,string pwd,string phone)
        {
            try
            {
                if (base.LoginMember != null)
                {
                    return Redirect("/User/Index");
                }
                Model.Store.Member member = new Model.Store.Member();
                member.AddTime = DateTime.Now;
                member.LastTime = DateTime.Now;
                member.LoginName = username;
                member.Password = Common.CryptoHelper.MD5Encrypt(pwd);
                member.Phone = phone;
                member.State = 1;
                member.AvatarPath = base.Config.DefaultAvatar;
                base.memberServer.Insert(member);
                return View("RegisterSuccess");
            }
            catch {
                return View("RegisterFail");
            }
        }

        public ActionResult List(int? page)
        {
            int count = 0;
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            if (page.HasValue) {
                pageturn.PageIndex = page.Value;
            } else {
                pageturn.PageIndex = 1;
            }
            var list = this.articleServer.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, " where IsShow=1 and IsTop=0", "LastTime DESC", out  count);
            pageturn.CountSize = count;
            ViewData["ArticleList"] = list;
            ViewData["PageTurn"] = pageturn;

            var topList = this.articleServer.ToPagedList(1, 3, " where IsShow=1 and IsTop=1", "LastTime DESC", out  count);
            ViewData["TopArticleList"] = topList;
            //
            return View();
        }

        public ActionResult Detail(int? id)
        {
            Model.CMS.Article article = articleServer.Find(id.Value);
            if (article.IsShow)
            {
                ViewData["DetailInfo"] = article;
                return View();
            }
            else {
                return View("/Error/Error");
            }
        }

        //public ActionResult Search(string key,int? page)
        //{
        //    return View();
        //}

        public ActionResult ItemInfo(int? id ) 
        {
            ViewData["ItemInfo"] = itemService.Find(id.Value);
            return View();
        }

        public ActionResult ItemList(int? page)
        {
            int count = 0;
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            if (page.HasValue)
            {
                pageturn.PageIndex = page.Value;
            }
            else
            {
                pageturn.PageIndex = 1;
            }
            var list = this.itemService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "where State!=4", "AddTime DESC", out  count);
            pageturn.CountSize = count;
            ViewData["ItemList"] = list;
            ViewData["PageTurn"] = pageturn;
            return View();
        }

        [ChildActionOnly]
        public ActionResult ZNav()
        {
            return PartialView("_ZNav");
        }

        [ChildActionOnly]
        public ActionResult UserNav()
        {
            return PartialView("_UserNav");
        }
    }
}
