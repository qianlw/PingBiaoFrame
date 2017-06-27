using Epoint.PingBiao.Model.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.Model.Sys.Enums;
using System.Web.Routing;

namespace Epoint.PingBiao.Controllers.Filters
{
    public class TemplateAttribute : FilterAttribute, IActionFilter, IExceptionFilter, IResultFilter, IDisposable
    {
        /// <summary>  
        /// HtmlTextWriter  
        /// </summary>  
        private HtmlTextWriter tw;  
        /// <summary>  
        /// StringWriter  
        /// </summary>  
        private StringWriter sw;  
        /// <summary>  
        /// StringBuilder  
        /// </summary>  
        private StringBuilder sb;  
        /// <summary>  
        /// HttpWriter  
        /// </summary>  
        private HttpWriter output;
  
        /// <summary>  
        /// 压缩html代码  
        /// </summary>  
        /// <param name="text">html代码</param>  
        /// <returns></returns>  
        private static string Compress(string text)  
        {  
            Regex reg = new Regex(@"\s*(</?[^\s/>]+[^>]*>)\s+(</?[^\s/>]+[^>]*>)\s*");  
            text = reg.Replace(text, m => m.Groups[1].Value + m.Groups[2].Value);  
  
            reg = new Regex(@"(?<=>)\s|\n|\t(?=<)");  
            text = reg.Replace(text, string.Empty);  
  
            return text;  
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            sb = new StringBuilder();
            sw = new StringWriter(sb);
            tw = new HtmlTextWriter(sw);
            output = (HttpWriter)filterContext.RequestContext.HttpContext.Response.Output;
            filterContext.RequestContext.HttpContext.Response.Output = tw;//覆盖原先的输出流
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Exception!=null) {return;}
            //生成的html
            try
            {
                string filename = filterContext.Controller.ViewData["FileName"].CastTo<string>();
                bool IsCompress = (bool)filterContext.Controller.ViewData["IsCompress"];
                string response = "";
                if (IsCompress)
                {
                    response = Compress(sb.ToString());
                }
                else
                {
                    response = sb.ToString();
                }
                string _t = "html";
                string _e = ".html";
                string _f = filename + _e;
                string _dir = Path.Combine(MyPath.AppPath, _t);
                if (Directory.Exists(_dir) == false)
                {
                    Directory.CreateDirectory(_dir);
                }
                string webUrl = "/" + _t + "/" + _f;
                string target_path = Path.Combine(MyPath.AppPath, _t, _f);
                IOHelper.WriteInfoToFile(response, target_path, Encoding.UTF8, false);
                ResultModel _result = null;
                if (File.Exists(target_path))
                {
                    _result = new ResultModel
                    {
                        msg = "重新生成",
                        pass = true,
                        append = new { url = webUrl }
                    };
                }
                else {
                    _result = new ResultModel
                    {
                        msg = "创建成功",
                        pass = true,
                        append = new { url = webUrl }
                    };
                }
                string s = filterContext.HttpContext.Response.Status;
                filterContext.HttpContext.Response.ContentType = "application/json";
                output.Write(JsonConvert.SerializeObject(_result));
                filterContext.RequestContext.HttpContext.Response.Output = output;
            }
            catch (Exception e)
            {
                ResultModel _result = new ResultModel
                {
                    msg = "创建失败，" + e.Message,
                    pass = false
                };
                filterContext.HttpContext.Response.ContentType = "application/json";
                output.Write(JsonConvert.SerializeObject(_result));
                filterContext.RequestContext.HttpContext.Response.Output = output;
            }
            finally {
                if (sw != null)
                {
                    sw.Dispose();
                }
                if (tw != null)
                {
                    tw.Dispose();
                }
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled == false)
            {
                IOHelper.WriteLog(filterContext.Exception);
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult()
                    {
                        ContentEncoding = Encoding.UTF8,
                        Data = new ResultModel
                        {
                            pass = false,
                            msg = "创建失败-" + filterContext.Exception.Message
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    RouteValueDictionary _dic = new RouteValueDictionary();
                    _dic.Add("controller", "Error");
                    _dic.Add("action", "Page404");
                    filterContext.Result = new RedirectToRouteResult("Default_Html", _dic);
                }

                filterContext.ExceptionHandled = true;
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                sw.Close();
                tw.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}