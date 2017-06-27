using Autofac;
using Autofac.Integration.Mvc;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.Controllers.Expand;
using Epoint.PingBiao.Controllers.Filters;
using Epoint.PingBiao.IService.CMS;
using Epoint.PingBiao.Model.Other;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace Epoint.PingBiao.Controllers.Controllers
{
    [Exception]
    public class BlogController : UserBaseController
    {
        //public IArticleService articleServer = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IArticleService>();
        //public IColumnService columnService = AutofacDependencyResolver.Current.ApplicationContainer.Resolve<IColumnService>();
        // 以后根据autofac.config中的name生成
        public IColumnService columnService = IocHelper.AutofacResolveNamed<IColumnService>("CMS.ColumnService");
        public IArticleService articleServer = IocHelper.AutofacResolveNamed<IArticleService>("CMS.ArticleService");
        //
        // GET: /Blog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? p, int? c)
        {
            int count = 0;
            if (p.HasValue == false) { p = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = p;
            if (c.HasValue)
            {
                ViewData["BlogList"] = articleServer.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "where ColumnId=" + c, " CreateTime DESC", out count);
            }
            else {
                ViewData["BlogList"] = articleServer.ToSearchList(pageturn.PageIndex.Value, pageturn.ItemSize, "", 1, out count);
            }
            pageturn.CountSize = count;
            ViewData["BlogPageCount"] = pageturn.PageCount;
            ViewData["BlogPage"] = p.Value;
            return View();
        }

        public ActionResult Search(string k, int? p)
        {
            if (string.IsNullOrEmpty(k)) { k = ""; };
            int count = 0;
            if (p.HasValue == false) { p = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = p;
            ViewData["BlogList"] = articleServer.ToSearchList(pageturn.PageIndex.Value, pageturn.ItemSize, k, 1, out count);
            ViewData["BlogCount"] = count;
            ViewData["BlogPage"] = pageturn.PageCount;
            ViewData["BlogKey"] = k;
            return View();
        }

        public ActionResult Detail(int? id) 
        {
            if (id.HasValue)
            {
                Epoint.PingBiao.Model.CMS.Article article=articleServer.Find(id.Value);
                if (article == null)
                {
                    return View("404");
                }
                else
                {
                    article.ArticleContent = System.Web.HttpUtility.HtmlDecode(article.ArticleContent);
                    ViewData["ArticleInfo"] = article;
                    return View();
                }
            }
            else {
                return View("404");
            }
        }

        public ActionResult Introduce() {
            return View();
        }
        //public ActionResult DownloadFile(string token) {
        //    if (token == "afdb6f6742cb41cc8044ba72771c4cbf")
        //    {
        //        string filepath = Model.Sys.Enums.MyPath.AppPath + "DownloadFile" + Path.DirectorySeparatorChar + "Epoint.PingBiao1.5.rar";
        //        //WebHelper.DownLoadFile(filepath);
        //        //记录下载数
        //        string recordpath = Model.Sys.Enums.MyPath.AppPath + "DownloadFile" + Path.DirectorySeparatorChar + "record.txt";
        //        string record=IOHelper.ReadTxt(recordpath, Encoding.UTF8);
        //        if (string.IsNullOrEmpty(record))
        //        {
        //            record = "1";
        //        }
        //        else {
        //            record = (Convert.ToInt32(record)+1).ToString();
        //        }
        //        IOHelper.WriteInfoToFile(record, recordpath, Encoding.UTF8, false);
        //        return File(new FileStream(filepath, FileMode.Open), "application/octet-stream", Server.UrlEncode("Epoint.PingBiao1.5.rar"));
        //    }
        //    else {
        //        //Response.Write("参数错误，下载失败");
        //        //Response.End();
        //        return Content("参数错误，下载失败");
        //    }
        //}
    }
}
