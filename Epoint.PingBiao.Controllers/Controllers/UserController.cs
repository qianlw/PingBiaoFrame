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
using System.IO;

namespace Cactus.Controllers.Controllers
{
    [Exception]
    public class UserController : UserPowerBaseController
    {
        //用户中心
        public IItemService itemService = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IItemService>();

        public IOrderService orderService = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IOrderService>();

        public IMemberService memberService = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IMemberService>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginOut()
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(CookieHelper.GetCookieValue("Member"));

            //移除缓存的登录用户信息
            CacheHelper.RemoveAllCache(Constant.CacheKey.LoginUserInfoCacheKey  +"_Member_" +ticket.UserData);

            //用户注销
            CookieHelper.ClearCookie("Member");

            if (HttpContext.Request.IsAjaxRequest())
            {
                return Json(new
                {
                    pass = true,
                    msg = "退出成功"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Redirect("/Store/Index");
            }
        }
        [HttpGet]
        public ActionResult Settlement() {
            return View("Error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Settlement(int itemid, int num)
        {
            if (num <= 0) {
                return View("Error");
            }
            ViewData["orderItem"]=itemService.Find(itemid);
            ViewData["orderNum"] = num;
            return View("OnOrder");
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Purchase(int itemid, int num, string address, string phone, string des)
        {
            if (string.IsNullOrEmpty(address)) {
                return View("OnOrderFail");
            }
            if (string.IsNullOrEmpty(phone))
            {
                return View("OnOrderFail");
            }
            Model.Store.Item item = itemService.Find(itemid);
            if (item.State != 1) {
                return View("OnOrderFail");
            }
            if (item.Stock >= num) {
                itemService.UpdateStock(item.Item_Id, item.Stock - num);
                Model.Store.Order order = new Model.Store.Order();
                order.Address = address;
                order.AddTime = DateTime.Now;
                order.Des = des;
                order.IsDeliver = false;
                order.IsReceipt = false;
                order.ItemId = item.Item_Id;
                order.MemberId = base.LoginMember.Member_Id;
                order.Num = num;
                order.Phone = phone;
                order.Price = item.Price;
                order.State = 1;
                orderService.Insert(order);
                return View("OnOrderSuccess");
            }
            return View("OnOrderFail");
        }
        [HttpGet]
        public ActionResult Purchase()
        {
            return View("Error");
        }

        public ActionResult AlterPwd()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AlterPwd(string oldpwd,string newpwd)
        {
            if (base.LoginMember.Password == Common.CryptoHelper.MD5Encrypt(oldpwd)) {
                memberService.ResetPassword(base.LoginMember.Member_Id, Common.CryptoHelper.MD5Encrypt(newpwd));
                ViewData["execResult"] = "修改成功，下次登录后生效";
                return View();
            }
            ViewData["execResult"] = "修改失败，密码错误";
            return View();
        }

        public ActionResult AlterFace() 
        { 
            return View();
        }
        [HttpPost]
        public ActionResult AlterFaceHandle()
        {
            var avatarFile = Request.Files["FaceFile"];
            if (avatarFile != null && avatarFile.ContentLength > 0)
            {
                if (avatarFile.ContentLength <= MyPath.fileSize * 1024)
                {
                    var avatarName = avatarFile.FileName;
                    var avatarExt = Path.GetExtension(avatarName);

                    if (!String.IsNullOrEmpty(avatarExt)
                        && Config.ImgExtensions.Contains(avatarExt))
                    {
                        if (!System.IO.Directory.Exists(MyPath.AvatarPath))
                        {
                            System.IO.Directory.CreateDirectory(MyPath.AvatarPath);
                        }
                        //保存原图
                        string imgPath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + avatarExt;
                        var savePath = Path.Combine(MyPath.AvatarPath, imgPath);
                        avatarFile.SaveAs(savePath);

                        Model.Store.Member u = this.memberServer.Find(this.LoginMember.Member_Id);
                        u.AvatarPath = MyPath.Web_AvatarPath + "/" + imgPath;
                        this.memberServer.Update(u);

                        ViewData["AlterFaceResult"] = "上传成功";
                    }
                    else
                    {
                        ViewData["AlterFaceResult"] = "上传文件类型错误";
                    }
                }
                else
                {
                    ViewData["AlterFaceResult"] = "上传文件大小超出限制," + MyPath.fileSize + "k以内";
                }
            }
            else
            {
                ViewData["AlterFaceResult"] = "上传失败";
            }
            return View("AlterFace");
        }

        public ActionResult OrderManager(int? page)
        {
            if (page.HasValue == false) { page = 1; }
            int count = 0;
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = 1;
            var list = this.orderService.ToPagedList(
                page.Value,
                pageturn.ItemSize,
                "MemberId=" + base.LoginMember.Member_Id + " and State<=2",
                "AddTime DESC",
                out  count);
            pageturn.CountSize = count;
            ViewData["OrderList"] = list;
            ViewData["Pageturn"] = pageturn;
            return View();
        }
        
        public ActionResult OrderInfo(int id) {
            ViewData["OrderInfo"] = this.orderService.Find(id,base.LoginMember.Member_Id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult OrderComment(int orderid, string comment, string state)
        {
            if (string.IsNullOrEmpty(comment)) { return View("OrderCommentFail"); }
            if (string.IsNullOrEmpty(state)) { return View("OrderCommentFail"); }
            if (this.orderService.BIsReceipt(orderid, comment, state == "true")) {
                return View("OrderCommentSuccess");
            }
            return View("OrderCommentFail");
        }

        public ActionResult BuyLog(int? page)
        {
            if (page.HasValue == false) { page = 1; }
            int count = 0;
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = 1;
            var list = this.orderService.ToPagedList(
                page.Value,
                pageturn.ItemSize,
                "MemberId=" + base.LoginMember.Member_Id + " and State>2",
                "AddTime DESC",
                out  count);
            pageturn.CountSize = count;
            ViewData["OrderList"] = list;
            ViewData["Pageturn"] = pageturn;
            return View();
        }
    }
}
