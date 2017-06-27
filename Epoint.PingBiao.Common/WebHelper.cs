using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Epoint.PingBiao.Common
{
    public static class WebHelper
    {
        /// <summary>
        /// 使用OutputStream.Write分块提供下载文件，参数为文件绝对路径
        /// </summary>
        /// <param name="FileName"></param>
        public static void DownLoadFile(string filePath)
        {
            //指定块大小
            long chunkSize = 204800;//1024*2*100 200kb
            //建立一个200K的缓冲区
            byte[] buffer = new byte[chunkSize];
            //已读的字节数
            long dataToRead = 0;
            FileStream stream = null;
            try
            {
                //打开文件
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataToRead = stream.Length;
                //添加Http头
                System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                string _fileName = HttpUtility.UrlEncode(Path.GetFileName(filePath), Encoding.UTF8);
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachement;filename=" + _fileName);
                System.Web.HttpContext.Current.Response.AddHeader("Content-Length", dataToRead.ToString());
                while (dataToRead > 0)
                {
                    if (System.Web.HttpContext.Current.Response.IsClientConnected)
                    {
                        int length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));//返回实际的流大小
                        System.Web.HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        try
                        {
                            System.Web.HttpContext.Current.Response.Flush();
                        }
                        catch {
                            //防止client失去连接
                            dataToRead = -1;
                        }
                        buffer = new Byte[chunkSize];
                        dataToRead = dataToRead - length;//减去实际传送的长度
                    }
                    else
                    {
                        //防止client失去连接
                        dataToRead = -1;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
                System.Web.HttpContext.Current.Response.Close();
            }
        }
        /// <summary>
        /// 将文件保存到目录
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="savePath">保存地址</param>
        public static bool saveUploadFile(HttpPostedFileBase file, string savePath)
        {
            return saveUploadFile(file, savePath, "", 0);
        }
        /// <summary>
        /// 将文件保存到目录
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="Extensions">文件类型</param>
        /// <returns></returns>
        public static bool saveUploadFile(HttpPostedFileBase file, string savePath, string Extensions)
        {
            return saveUploadFile(file, savePath, Extensions, 0);
        }
        /// <summary>
        /// 将文件保存到目录
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="fileSize">文件大小（单位是kb，等于0是不检测）</param>
        /// <returns></returns>
        public static bool saveUploadFile(HttpPostedFileBase file, string savePath, int fileSize)
        {
            return saveUploadFile(file, savePath, "", fileSize);
        }
        /// <summary>
        /// 将文件保存到目录
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="savePath">保存地址</param>
        /// <param name="Extensions">文件类型</param>
        /// <param name="fileSize">文件大小（单位是kb，等于0是不检测）</param>
        /// <returns></returns>
        public static bool saveUploadFile(HttpPostedFileBase file, string savePath, string Extensions, int fileSize)
        {
            bool _b = true;
            if (fileSize != 0)
            {
                if (file.ContentLength <= fileSize * 1024)
                {
                    _b = true;
                }
                else
                {
                    _b = false;
                }
            }
            if (_b)
            {
                var avatarName = file.FileName;
                var avatarExt = Path.GetExtension(avatarName);
                if (!String.IsNullOrEmpty(avatarExt)
                                        && (string.IsNullOrEmpty(Extensions) ? true : Extensions.Contains(avatarExt)))
                {
                    try
                    {
                        file.SaveAs(savePath);
                        return true;
                    }
                    catch {
                        return false;
                    }
                    
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val">参数</param>
        /// <param name="callback">回调函数</param>
        public static void Http(string url, string method, string val, AsyncCallback callback)
        {
            Http(url, method, val, "", "", Encoding.UTF8, 10 * 1000, callback);
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val">参数</param>
        /// <param name="cookie"></param>
        /// <param name="referer"></param>
        /// <param name="callback">回调函数</param>
        public static void Http(string url, string method, string val, string cookie, string referer, AsyncCallback callback)
        {
            Http(url, method, val, cookie, referer, Encoding.UTF8, 10 * 1000, callback);
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val"></param>
        /// <param name="cookie"></param>
        /// <param name="referer"></param>
        /// <param name="encoding">编码</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="callback">回调函数</param>
        public static void Http(string url, string method, string val, string cookie, string referer, Encoding encoding, int timeout, AsyncCallback callback)
        {

            NameValueCollection nav = new NameValueCollection();
            nav.Add("cookie", cookie);
            nav.Add("referer", referer);
            Http(url, method, val, nav, encoding, timeout, callback);
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val"></param>
        /// <param name="nav">请求头集合</param>
        /// <param name="callback">回调函数</param>
        public static void Http(string url, string method, string val, NameValueCollection nav, AsyncCallback callback)
        {
            Http(url, method, val, nav, Encoding.UTF8, 10 * 1000, callback);
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val"></param>
        /// <param name="nav">请求头集合</param>
        /// <param name="encoding"></param>
        /// <param name="timeout">超时时间</param>
        /// <param name="callback">回调函数</param>
        public static void Http(string url, string method, string val, NameValueCollection nav, Encoding encoding, int timeout, AsyncCallback callback)
        {
            Http(url, method, val, nav, Encoding.UTF8, 10 * 1000, callback, delegate { return new IPEndPoint(IPAddress.Parse("0.0.0.0"), 50000); });
        }
        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">请求类型</param>
        /// <param name="val"></param>
        /// <param name="nav">请求头集合</param>
        /// <param name="encoding"></param>
        /// <param name="timeout">超时时间</param>
        /// <param name="callback">回调函数</param>
        /// <param name="point">ip端口</param>
        public static void Http(string url, string method, string val, NameValueCollection nav, Encoding encoding, int timeout, AsyncCallback callback, BindIPEndPoint point)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = method;
            req.ServicePoint.Expect100Continue = false;
            req.ServicePoint.BindIPEndPointDelegate = point;
            for (int i = 0; i < nav.AllKeys.Length; i++)
            {
                SetHeaderValue(req.Headers, nav.AllKeys[i], nav[nav.AllKeys[i]]);
            }
            if (string.IsNullOrEmpty(val) == false)
            {
                byte[] byteArray = encoding.GetBytes(val);
                req.ContentLength = byteArray.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();
            }
            req.Timeout = timeout;
            req.BeginGetResponse(callback, req);
        }
        /// <summary>
        /// 添加http头
        /// </summary>
        /// <param name="header"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
}
