using Epoint.PingBiao.Controllers.Expand;
using Epoint.PingBiao.Controllers.Filters;
using Epoint.PingBiao.IService.CMS;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Epoint.PingBiao.Model.Other;
using Epoint.PingBiao.Model.Sys.Enums;
using Epoint.PingBiao.Common;
using System.IO;
using System.Collections.Generic;
using Epoint.PingBiao.Model.CMS;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Epoint.PingBiao.Controllers.Areas.Admin.Controllers
{
    [Exception]
    [Group(GroupName = "文章管理", NoGroupId = "Blog2016", IsShow = true, Icon = "fa-file")]
    public class BlogController : PowerBaseController
    {
        public IArticleService articleServer = IocHelper.AutofacResolveNamed<IArticleService>("CMS.ArticleService");
        public IColumnService columnService = IocHelper.AutofacResolveNamed<IColumnService>("CMS.ColumnService");
        public ITempPageService tempPageService = IocHelper.AutofacResolveNamed<ITempPageService>("CMS.TempPageService");
        public IStaticPageService staticPageService = IocHelper.AutofacResolveNamed<IStaticPageService>("CMS.StaticPageService");
        public ITagService tagService = IocHelper.AutofacResolveNamed<ITagService>("CMS.TagService");
        public ICommentService commentService = IocHelper.AutofacResolveNamed<ICommentService>("CMS.CommentService");

        //
        // GET: /Admin/Blog/
        #region Blog

        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_A101", Icon = "fa-book", PowerName = "文章管理", PowerDes = "文章管理")]
        public ActionResult BlogIndex()
        {
            return View();
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A01", PowerName = "文章管理", PowerDes = "文章管理")]
        public ActionResult BlogList(int? page,string title)
        {
            if (!page.HasValue) { page = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = page;
            int count = 0;
            var list = this.articleServer.ToSearchList(pageturn.PageIndex.Value, pageturn.ItemSize,title, 1, out  count).Select(t => new
            {
                t.Article_Id,
                t.Title,
                t.Column.ColumnName,
                LastTime=t.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                t.IsShow,
                t.IsTop
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel{ rows = list, pagination = pageturn, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A102", PowerName = "文章添加", PowerDes = "对文章添加")]
        public ActionResult BlogAdd() {
            ViewData["ColumnList"] = this.columnService.GetAll();
            return View();
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A102", PowerName = "文章添加", PowerDes = "对文章添加")]
        public ActionResult BlogAdd(Model.CMS.Article art )
        {
            if (this.articleServer.IsUseTitle(art.Title,0))
            {
                return Json(new ResultModel { msg = "标题与其他文章相同", pass = false });
            }
            else
            {
                art.LastTime = DateTime.Now;
                art.CreateTime = art.LastTime;
                art.Title = art.Title.Trim();
                int aid=this.articleServer.InsertForInt(art);
                if (string.IsNullOrEmpty(art.Tags) == false)
                {
                    bool b = tagService.InsertToMapBatch(aid, art.TagIds);
                    if (b == false)
                    {
                        return Json(new ResultModel { msg = "插入失败", pass = true });
                    }
                }
                if (aid > 0)
                {
                    return Json(new ResultModel { msg = "添加成功", pass = true });
                }
                else
                {
                    return Json(new ResultModel { msg = "添加失败", pass = false });
                }
            }
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A103", PowerName = "文章修改", PowerDes = "文章修改")]
        public ActionResult BlogUpdate(int id)
        {
            Model.CMS.Article art=this.articleServer.Find(id);
            art.ArticleContent = System.Web.HttpUtility.HtmlDecode(art.ArticleContent);
            ViewData["Blog"] = art;
            ViewData["ColumnList"] = this.columnService.GetAll();
            return View();
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A103", PowerName = "文章修改", PowerDes = "文章修改")]
        public ActionResult BlogUpdate(Model.CMS.Article art)
        {
            try
            {
                if (this.articleServer.IsUseTitle(art.Title, art.Article_Id))
                {
                    return Json(new ResultModel { msg = "标题重复", pass = false });
                }
                else
                {
                    Model.CMS.Article model = this.articleServer.Find(art.Article_Id);
                    model.Article_Id = art.Article_Id;
                    model.ArticleContent = art.ArticleContent;
                    model.Author = art.Author;
                    model.ColumnId = art.ColumnId;
                    if (art.Tags != model.Tags) 
                    {
                        tagService.DeteteToMap(art.Article_Id);
                        bool b=tagService.InsertToMapBatch(art.Article_Id,art.TagIds);
                        if (b == false) {
                            return Json(new ResultModel { msg = "插入失败", pass = true });
                        }
                    }
                    model.Tags = art.Tags;
                    model.TagIds = art.TagIds;
                    model.Title = art.Title;
                    model.IsShow = art.IsShow;
                    model.IsTop = art.IsTop;
                    model.LastTime = DateTime.Now;
                    this.articleServer.Update(model);
                    return Json(new ResultModel { msg = "修改成功", pass = true });
                }
            }
            catch
            {
                return Json(new ResultModel { msg = "修改失败", pass = false });
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A104", PowerName = "文章删除", PowerDes = "文章删除")]
        public ActionResult BlogDelete(string ids)
        {
            try
            {
                this.articleServer.Delete(ids);
                return Json(new ResultModel { msg = "删除成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A105", PowerName = "文章置顶", PowerDes = "可以让文章置顶")]
        public ActionResult BlogIsTop(int id, bool on_off)
        {
            try
            {
                this.articleServer.IsTop(id, on_off);
                return Json(new ResultModel { msg = "置顶成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "置顶失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A106", PowerName = "文章显示", PowerDes = "可以让文章显示或隐藏")]
        public ActionResult BlogIsShow(int id,bool on_off)
        {
            try
            {
                this.articleServer.IsShow(id, on_off);
                return Json(new ResultModel { msg = "操作成功",pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "操作失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A107", PowerName = "栏目管理", PowerDes = "管理文章的栏目")]
        public ActionResult ColumnManage()
        {
            ViewData["ColumnList"] = this.columnService.GetAll();
            return View();
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A108", PowerName = "栏目添加", PowerDes = "栏目添加")]
        public ActionResult ColumnAdd()
        {
            var list = this.columnService.GetAll();
            ViewData["ColumnList"] = list;
            return View();
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A108", PowerName = "栏目添加", PowerDes = "栏目添加")]
        public ActionResult ColumnAdd(Model.CMS.Column column)
        {
            if (this.columnService.IsUseColumnName(column.ColumnName,0))
            {
                column.ColumnName = column.ColumnName.Trim();
                var b = this.columnService.Insert(column);
                if (b)
                {
                    return Json(new ResultModel { msg = "添加成功", pass = true });
                }
                else
                {
                    return Json(new ResultModel { msg = "添加失败", pass = false });
                }
            }
            else { return Json(new ResultModel { msg = "类目名已经存在", pass = false }); }
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A109", PowerName = "栏目修改", PowerDes = "栏目修改")]
        public ActionResult ColumnUpdate(int id)
        {
            ViewData["Column"] = this.columnService.Find(id);
            return View();    
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A109", PowerName = "栏目修改", PowerDes = "栏目修改")]
        public ActionResult ColumnUpdate(Model.CMS.Column column)
        {
            try
            {
                if (this.columnService.IsUseColumnName(column.ColumnName, column.Column_Id))
                {
                    return Json(new ResultModel { msg = "栏目名重复", pass = false });
                }
                else {
                    column.ColumnName = column.ColumnName.Trim();
                    this.columnService.Update(column);
                    return Json(new ResultModel { msg = "修改成功", pass = true });
                }
            }
            catch
            {
                return Json(new ResultModel { msg = "修改失败", pass = false });
            }
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_A110", PowerName = "栏目删除", PowerDes = "栏目删除")]
        public ActionResult ColumnDelete(string ids)
        {
            try
            {
                int id = Convert.ToInt32(ids);
                if (this.articleServer.IsUseColumn(id))
                {
                    return Json(new ResultModel { msg = "删除失败,该栏目正在使用", pass = false }, JsonRequestBehavior.AllowGet);
                }
                else {
                    this.columnService.Delete(ids);
                    return Json(new ResultModel { msg = "删除成功",pass = true }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Transfer(string view)
        {
            return View(view);
        }
        #endregion

        #region UploadImg
        //编辑器上传图片
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_B101", PowerName = "编辑器上传图片", PowerDes = "编辑器上传图片")]
        public ActionResult UploadImg()
        {
            HttpFileCollectionBase files = Request.Files;
            if (files.Count <= 0)
            {
                return Content("error|file is null");
            }
            HttpPostedFileBase file = files[0];
            if (file == null)
            {
                return Content("error|file is null");
            }
            else
            {
                string originalFileName = file.FileName;
                string fileExtension = originalFileName.Substring(originalFileName.LastIndexOf('.'), originalFileName.Length - originalFileName.LastIndexOf('.'));
                string currentFileName = StringHelper.GetTimeStamp() + fileExtension;  //文件名中不要带中文
                if (!System.IO.Directory.Exists(MyPath.UploadFilePath))
                {
                    System.IO.Directory.CreateDirectory(MyPath.UploadFilePath);
                }
                string imagePath = MyPath.EditUploadPath + Path.DirectorySeparatorChar + currentFileName;
                if (WebHelper.saveUploadFile(file, imagePath, Config.ImgExtensions, MyPath.fileSize))
                {
                    //获取图片url地址
                    string imgUrl = MyPath.Web_EditUploadPath + "/" + currentFileName;
                    return Content(imgUrl);
                }
                else {
                    return Content("error|save file is error");
                }
            }
        }

        #endregion

        #region 文件管理

        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_C101", Icon = "fa-file", PowerName = "文件管理", PowerDes = "文件管理")]
        public ActionResult FileManage()
        {
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            if (Directory.Exists(_root)==false) {
                Directory.CreateDirectory(_root);
            }
            return View();
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_C101", PowerName = "文件管理", PowerDes = "文件管理")]
        public ActionResult FileList(string currentDir,string viewDir)
        {
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            string target = "";
            if (string.IsNullOrEmpty(viewDir)) {
                target = _root + Path.DirectorySeparatorChar + currentDir;//目标目录
            } 
            else {
                target = _root + Path.DirectorySeparatorChar + currentDir + Path.DirectorySeparatorChar + viewDir;//目标目录
            }
            string fullpath = "";
            try
            {
                fullpath = Path.GetFullPath(target);//完整绝对路径
            }
            catch(Exception e){
                //防止出现权限等文件常识性错误
                ResultModel _result = new ResultModel
                {
                    msg = "获取失败," + e.Message,
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            bool isRoot = false;
            if (fullpath.StartsWith(_root))//满足根目录（不允许访问其他目录）
            {
                if (_root == fullpath) {
                    isRoot = true;
                }
                else if ((_root+@"\") == fullpath)
                {
                    isRoot = true;
                }
                string lastPath = fullpath.Remove(0, _root.Length);
                if (System.IO.File.Exists(fullpath))//防止访问的是一个单独文件
                {
                    ResultModel _result = new ResultModel
                    {
                        msg = "获取失败,参数错误,不允许访问单独文件",
                        pass = false,
                        append = new { dir_list = "", f_list = "", isRoot = isRoot, RootPath = isRoot ? Path.DirectorySeparatorChar.ToString() : lastPath }
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
                if (Directory.Exists(fullpath))
                {
                    DirectoryInfo[] dir_list = IOHelper.GetDirectory(fullpath);
                    List<Model.CMS.Ext.DInfo> d_list = new List<Model.CMS.Ext.DInfo>();
                    for (int i = 0; i < dir_list.Length; i++)
                    {
                        d_list.Add(new Model.CMS.Ext.DInfo(dir_list[i]));
                    }
                    FileInfo[] file_list = IOHelper.GetFiles(fullpath);
                    List<Model.CMS.Ext.FInfo> f_list = new List<Model.CMS.Ext.FInfo>();
                    for (int i = 0; i < file_list.Length; i++)
                    {
                        f_list.Add(new Model.CMS.Ext.FInfo(file_list[i]));
                    }
                    ResultModel _result = new ResultModel
                    {
                        msg = "获取成功",
                        pass = true,
                        append = new { dir_list = d_list, f_list = f_list, isRoot = isRoot, RootPath = isRoot ? Path.DirectorySeparatorChar.ToString() : lastPath }
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ResultModel _result = new ResultModel
                    {
                        msg = "获取成功",
                        pass = true,
                        append = new { dir_list = new { }, f_list = new { }, isRoot = isRoot, RootPath = isRoot ? Path.DirectorySeparatorChar.ToString() : lastPath }
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
            }
            else {
                ResultModel _result = new ResultModel
                {
                    msg = "获取失败,参数错误，不能访问其他目录",
                    pass = false,
                    append = new { dir_list = new { }, f_list = new { }, isRoot = isRoot, RootPath = fullpath }
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_C102", PowerName = "文件删除", PowerDes = "文件管理，文件删除")]
        public ActionResult FileDel(string currentDir, string fileOrDir)
        {
            if (string.IsNullOrEmpty(fileOrDir))
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            string target = _root + Path.DirectorySeparatorChar + currentDir + Path.DirectorySeparatorChar + fileOrDir;//目标目录或者文件
            string fullpath = "";
            try
            {
                fullpath = Path.GetFullPath(target);//完整绝对路径
            }
            catch (Exception e)
            {
                //防止出现权限等文件常识性错误
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误," + e.Message,
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (fullpath.StartsWith(_root))//满足根目录（不允许访问其他目录）
            {
                if (System.IO.File.Exists(fullpath))
                {
                    try
                    {
                        System.IO.File.Delete(fullpath);
                        ResultModel _result = new ResultModel
                        {
                            msg = "删除成功",
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "删除失败," + e.Message,
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    if (System.IO.Directory.Exists(fullpath))
                    {
                        try
                        {
                            System.IO.Directory.Delete(fullpath, true);
                            ResultModel _result = new ResultModel
                            {
                                msg = "删除成功",
                                pass = false
                            };
                            return Json(_result, JsonRequestBehavior.AllowGet);
                        }
                        catch(Exception e){
                            ResultModel _result = new ResultModel
                            {
                                msg = "删除失败," + e.Message,
                                pass = true
                            };
                            return Json(_result, JsonRequestBehavior.AllowGet);
                        }
                        
                    }
                    else
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "文件或者目录未找到",
                            pass = false
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_C103", PowerName = "文件重命名", PowerDes = "文件管理，文件重命名")]
        public ActionResult FileRename(string currentDir, string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(newName))
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            string target_old = _root + Path.DirectorySeparatorChar + currentDir + Path.DirectorySeparatorChar + oldName;//目标目录或者文件
            string target_new = _root + Path.DirectorySeparatorChar + currentDir + Path.DirectorySeparatorChar + newName;//目标目录或者文件
            string oldpath = "", newPath="";
            try
            {
                oldpath = Path.GetFullPath(target_old);//完整绝对路径
                newPath = Path.GetFullPath(target_new);//完整绝对路径
            }
            catch (Exception e)
            {
                //防止出现权限等文件常识性错误
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误," + e.Message,
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (oldpath.StartsWith(_root) && newPath.StartsWith(_root))//满足根目录（不允许访问其他目录）
            {
                if (System.IO.File.Exists(oldpath))
                {
                    try
                    {
                        System.IO.File.Move(oldpath, newPath);
                        ResultModel _result = new ResultModel
                        {
                            msg = "命名成功",
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "命名失败," + e.Message,
                            pass = false
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(oldpath))
                    {
                        try
                        {
                            System.IO.Directory.Move(oldpath, newPath);
                            ResultModel _result = new ResultModel
                            {
                                msg = "命名成功",
                                pass = true
                            };
                            return Json(_result, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception e)
                        {
                            ResultModel _result = new ResultModel
                            {
                                msg = "命名失败," + e.Message,
                                pass = false
                            };
                            return Json(_result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "文件或者目录未找到",
                            pass = false
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_C104", PowerName = "新建目录", PowerDes = "文件管理，新建目录")]
        public ActionResult FileNewDir(string currentDir, string newDirName) {
            if (string.IsNullOrEmpty(newDirName))
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (newDirName.IndexOf('.') > 0)
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            string target = _root + Path.DirectorySeparatorChar + currentDir + Path.DirectorySeparatorChar + newDirName;//目标目录或者文件
            string fullpath = "";
            try
            {
                fullpath = Path.GetFullPath(target);//完整绝对路径
            }
            catch (Exception e)
            {
                //防止出现权限等文件常识性错误
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误," + e.Message,
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (fullpath.StartsWith(_root))//满足根目录（不允许访问其他目录）
            {
                if (System.IO.Directory.Exists(fullpath))
                {
                    ResultModel _result = new ResultModel
                    {
                        msg = "目录已经存在",
                        pass = false
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(fullpath);
                        ResultModel _result = new ResultModel
                        {
                            msg = "创建成功",
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "创建失败," + e.Message,
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_C105", PowerName = "上传文件", PowerDes = "文件管理，上传文件")]
        public ActionResult FileUpload(string currentDir) {
            string _root = MyPath.AppPath + "File";//文件主目录（根目录）
            string target = _root + Path.DirectorySeparatorChar + currentDir;//目标目录或者文件
            string fullpath = "";
            try
            {
                fullpath = Path.GetFullPath(target);//完整绝对路径
            }
            catch (Exception e)
            {
                //防止出现权限等文件常识性错误
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误," + e.Message,
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
            if (fullpath.StartsWith(_root))//满足根目录（不允许访问其他目录）
            {
                HttpFileCollectionBase files = Request.Files;
                if (files.Count <= 0)
                {
                    ResultModel _result = new ResultModel
                    {
                        msg = "参数错误",
                        pass = false
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
                HttpPostedFileBase file = files[0];
                if (file == null)
                {
                    ResultModel _result = new ResultModel
                    {
                        msg = "参数错误",
                        pass = false
                    };
                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string filePath = fullpath;
                    if (!filePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        filePath = filePath + Path.DirectorySeparatorChar;
                    }

                    filePath = filePath + file.FileName;
                    if (WebHelper.saveUploadFile(file, filePath, 1024 * 4))
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "上传成功",
                            pass = true
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ResultModel _result = new ResultModel
                        {
                            msg = "上传失败",
                            pass = false
                        };
                        return Json(_result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else {
                ResultModel _result = new ResultModel
                {
                    msg = "参数错误",
                    pass = false
                };
                return Json(_result, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region 静态管理
        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_D101",Icon="fa-file-text", PowerName = "页面管理", PowerDes = "用来生成静态单页")]
        public ActionResult StaticHtmlManage()
        {
            return View();
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D101", PowerName = "页面管理", PowerDes = "用来生成静态单页")]
        public ActionResult StaticHtmlManage(int? page)
        {
            if (!page.HasValue) { page = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = page;
            int count = 0;
            var list = this.staticPageService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "CreateTime DESC", out  count).Select(t => new
            {
                t.Page_Id,
                t.PageName,
                TempName=t.TempPage.TempName,
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                LastTime = t.LastTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel { rows = list, pagination = pageturn, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D102", PowerName = "添加页面", PowerDes = "添加页面")]
        public ActionResult StaticHtmlAdd()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D102", PowerName = "添加页面", PowerDes = "添加页面")]
        public ActionResult StaticHtmlAdd(string PageName, string PageParameter, int TempPageId)
        {
            try {
                if (string.IsNullOrEmpty(PageName)) { return Json(new ResultModel { msg = "添加失败", pass = false }, JsonRequestBehavior.AllowGet); }
                if (string.IsNullOrEmpty(PageParameter)) { return Json(new ResultModel { msg = "添加失败", pass = false }, JsonRequestBehavior.AllowGet); }
                StaticPage _page = new StaticPage();
                _page.CreateTime = DateTime.Now;
                _page.LastTime = _page.CreateTime;
                _page.PageName = PageName;
                _page.PageParameter = PageParameter;
                _page.TempPageId = TempPageId;
                this.staticPageService.Insert(_page);
                return Json(new ResultModel { msg = "添加成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch {
                return Json(new ResultModel { msg = "添加失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
         [HttpGet]
         [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D103", PowerName = "修改页面", PowerDes = "修改页面")]
        public ActionResult StaticHtmlUpdate(int id)
        {
            ViewData["staticPage"] = this.staticPageService.Find(id);
             return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D103", PowerName = "修改页面", PowerDes = "修改页面")]
        public ActionResult StaticHtmlUpdate(int Page_Id, string PageName, string PageParameter)
        {
            try {
                if (string.IsNullOrEmpty(PageName)) { return Json(new ResultModel { msg = "修改失败", pass = false }, JsonRequestBehavior.AllowGet); }
                if (string.IsNullOrEmpty(PageParameter)) { return Json(new ResultModel { msg = "修改失败", pass = false }, JsonRequestBehavior.AllowGet); }
                StaticPage _page = this.staticPageService.Find(Page_Id);
                _page.PageName = PageName;
                _page.PageParameter = PageParameter;
                _page.LastTime = DateTime.Now;
                this.staticPageService.Update(_page);
                return Json(new ResultModel { msg = "修改成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch {
                return Json(new ResultModel { msg = "修改失败", pass = false }, JsonRequestBehavior.AllowGet); 
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D104", PowerName = "删除页面", PowerDes = "删除页面")]
        public ActionResult StaticHtmlDelete(string ids)
        {
            try
            {
                this.staticPageService.Delete(ids);
                return Json(new ResultModel { msg = "删除成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [Template]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D105", PowerName = "生成页面", PowerDes = "生成页面")]
        public ActionResult StaticHtmlRender(int id) {
            StaticPage _page=this.staticPageService.Find(id);
            JObject j_obj = new JObject();
            JArray j_arry= Newtonsoft.Json.Linq.JArray.Parse(_page.PageParameter);
            for (int i = 0; i<j_arry.Count ;i++)
            {
                j_obj.Add(j_arry[i]["key"].ToString(), j_arry[i]["value"]);
            }
            ViewData["Data"] = j_obj;
            ViewData["FileName"] = _page.PageName;
            ViewData["IsCompress"] = true;
            return View(_page.TempPage.TempPath);
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D106", PowerName = "模板管理", PowerDes = "模板管理")]
        public ActionResult TemplateManage() {
            return View();
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D106", PowerName = "模板管理", PowerDes = "模板管理")]
        public ActionResult TemplateManage(int? page)
        {
            if (!page.HasValue) { page = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 100 };
            pageturn.PageIndex = page;
            int count = 0;
            var list = this.tempPageService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "CreateTime DESC", out  count).Select(t => new
            {
                t.TempPage_Id,
                t.TempName,
                CreateTime=t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                LastTime=t.LastTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel { rows = list, pagination = null, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D106", PowerName = "模板管理", PowerDes = "模板管理")]
        public ActionResult TemplateList() {
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 100 };
            pageturn.PageIndex = 1;
            int count = 0;
            var list = this.tempPageService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "CreateTime DESC", out  count).Select(t => new
            {
                t.TempPage_Id,
                t.TempName,
                t.TempByname
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel { rows = list, pagination = null, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D106", PowerName = "模板管理", PowerDes = "模板管理")]
        public ActionResult TemplateFind(int id)
        {
            Epoint.PingBiao.Model.CMS.TempPage t = this.tempPageService.Find(id);
            if (t != null)
            {
                return Json(new ObjResultModel {  pass = true, obj = new { TempPage_Id = t.TempPage_Id, TempParameter = t.TempParameter } }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("数据错误");
            }    
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D107", PowerName = "添加模板", PowerDes = "添加模板")]
        public ActionResult TemplateAdd()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D107", PowerName = "添加模板", PowerDes = "添加模板")]
        public ActionResult TemplateAdd(string TempName, string TempByname, string TempParameter, string TempContent)
        {
            if (string.IsNullOrEmpty(TempName)) { return Json(new ResultModel { msg = "添加失败", pass = false }); }
            if (string.IsNullOrEmpty(TempParameter)) { return Json(new ResultModel { msg = "添加失败", pass = false }); }
            if (string.IsNullOrEmpty(TempContent)) { return Json(new ResultModel { msg = "添加失败", pass = false }); }
            try {
                TempPage tempPage = new TempPage();
                tempPage.TempName = TempName;
                tempPage.TempByname = TempByname;
                tempPage.TempParameter = TempParameter;
                tempPage.TempContent = TempContent;
                tempPage.CreateTime = DateTime.Now;
                tempPage.LastTime = DateTime.Now;                
                string tempName="Template";
                string tempPath = Path.Combine(MyPath.AppPath, tempName);
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                string _path = "/"+tempName+"/" + TempName+".cshtml";
                tempPage.TempPath = _path;
                string sysPath=IOHelper.SysPathParse(MyPath.AppPath, _path, true);
                IOHelper.WriteInfoToFile(TempContent, sysPath, Encoding.UTF8, true);
                this.tempPageService.Insert(tempPage);
                return Json(new ResultModel { msg = "添加成功", pass = true });
            }
            catch {
                return Json(new ResultModel { msg = "添加失败", pass = false });
            }
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D108", PowerName = "更新模板", PowerDes = "更新模板")]
        public ActionResult TemplateUpdate(int id)
        {
            Epoint.PingBiao.Model.CMS.TempPage t = this.tempPageService.Find(id);
            if (t != null) {
                ViewData["TempPage"] = t;
                return View();
            } else {
                return Content("数据错误");
            }            
        }
        [ValidateInput(false)]
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D108", PowerName = "更新模板", PowerDes = "更新模板")]
        public ActionResult TemplateUpdate(int TempPage_Id,string TempByname, string TempParameter, string TempContent)
        {
            if (string.IsNullOrEmpty(TempByname)) { return Json(new ResultModel { msg = "修改失败", pass = false }); }
            if (string.IsNullOrEmpty(TempParameter)) { return Json(new ResultModel { msg = "修改失败", pass = false }); }
            if (string.IsNullOrEmpty(TempContent)) { return Json(new ResultModel { msg = "修改失败", pass = false }); }
            try
            {
                TempPage tempPage = tempPageService.Find(TempPage_Id);
                tempPage.TempByname = TempByname;
                tempPage.TempParameter = TempParameter;
                tempPage.TempContent = TempContent;
                tempPage.LastTime = DateTime.Now;
                string sysPath = IOHelper.SysPathParse(MyPath.AppPath, tempPage.TempPath, true);
                IOHelper.WriteInfoToFile(TempContent, sysPath, Encoding.UTF8, false);
                this.tempPageService.Update(tempPage);
                return Json(new ResultModel { msg = "修改成功", pass = true });
            }
            catch
            {
                return Json(new ResultModel { msg = "修改失败", pass = false });
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D109", PowerName = "删除模板", PowerDes = "删除模板")]
        public ActionResult TemplateDelete(string ids)
        {
            try
            {
                this.tempPageService.Delete(ids); 
                return Json(new ResultModel { msg = "删除成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_D110", PowerName = "模板生成", PowerDes = "模板生成")]
        public ActionResult TemplateCSHtmlRender(int id) 
        {
            TempPage tempPage = this.tempPageService.Find(id);
            string sysPath = IOHelper.SysPathParse(MyPath.AppPath, tempPage.TempPath, true);
            if (System.IO.File.Exists(sysPath))
            {
                IOHelper.WriteInfoToFile(tempPage.TempContent, sysPath, Encoding.UTF8, false);
                return Json(new ResultModel { msg = "重新生成", pass = true });
            }
            else
            {
                IOHelper.WriteInfoToFile(tempPage.TempContent, sysPath, Encoding.UTF8, false);
                return Json(new ResultModel { msg = "生成成功", pass = true });
            }
        }
        #endregion

        #region 标签管理
        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_E101", Icon = "fa-tag", PowerName = "标签管理", PowerDes = "标签管理")]
        public ActionResult TagManage() { return View(); }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E101", PowerName = "标签管理", PowerDes = "标签管理")]
        public ActionResult TagList(int? page) {
            if (!page.HasValue) { page = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = page;
            int count = 0;
            var list = this.tagService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "CreateTime DESC", out  count).Select(t => new
            {
                t.Tag_Id,
                t.TagName,
                t.TagDes,
                LastTime = t.LastTime.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel { rows = list, pagination = pageturn, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E102", PowerName = "标签删除", PowerDes = "标签删除")]
        public ActionResult TagDelete(string ids) {
            try
            {
                this.tagService.Delete(ids);
                return Json(new ResultModel { msg = "删除成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E103", PowerName = "更新标签", PowerDes = "更新标签")]
        public ActionResult TagUpdate(Tag tag) {

            if (this.tagService.IsUseTagName(tag.TagName, tag.Tag_Id))
            {
                return Json(new ResultModel { msg = "标签名与其他标签相同", pass = false });
            }
            else
            {
                try
                {
                    Model.CMS.Tag model = this.tagService.Find(tag.Tag_Id);
                    model.Tag_Id = tag.Tag_Id;
                    model.LastTime = DateTime.Now;
                    model.TagName = tag.TagName.Trim();
                    model.TagDes = tag.TagDes;
                    this.tagService.Update(model);
                    return Json(new ResultModel { msg = "修改成功", pass = true });
                }
                catch {
                    return Json(new ResultModel { msg = "修改失败", pass = false });
                }
            }
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E103", PowerName = "更新标签", PowerDes = "更新标签")]
        public ActionResult TagUpdate(int id) {
            ViewData["Tag"] = this.tagService.Find(id);
            return View();    
        }
        [HttpPost]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E104", PowerName = "添加标签", PowerDes = "添加标签")]
        public ActionResult TagAdd(Tag tag) {
            if (this.tagService.IsUseTagName(tag.TagName, 0))
            {
                return Json(new ResultModel { msg = "标签名与其他标签相同", pass = false });
            }
            else
            {
                tag.CreateTime = DateTime.Now;
                tag.LastTime = tag.CreateTime;
                tag.TagName = tag.TagName.Trim();
                var b = this.tagService.Insert(tag);
                if (b)
                {
                    return Json(new ResultModel { msg = "添加成功", pass = true });
                }
                else
                {
                    return Json(new ResultModel { msg = "添加失败", pass = false });
                }
            }
        }
        [HttpGet]
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_E104", PowerName = "添加标签", PowerDes = "添加标签")]
        public ActionResult TagAdd() { return View(); }
        #endregion

        #region 评论管理
        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_F101", Icon = "fa-comment", PowerName = "评论管理", PowerDes = "评论管理")]
        public ActionResult CommentManage()
        {
            return View();
        }
        [Power(IsSuper = false, IsShow = true, PowerId = "Blog_F101", PowerName = "评论管理", PowerDes = "评论管理")]
        public ActionResult CommentList(int? page) 
        {
            if (!page.HasValue) { page = 1; }
            PageTurnModel pageturn = new PageTurnModel() { ItemSize = 10 };
            pageturn.PageIndex = page;
            int count = 0;
            var list = this.commentService.ToPagedList(pageturn.PageIndex.Value, pageturn.ItemSize, "CreateTime DESC", out  count).Select(t => new
            {
                t.Comment_Id,
                t.Content,
                t.Email,
                t.Nickname,
                CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                t.Article.Title
            });
            pageturn.CountSize = count;
            return Json(new RowResultModel { rows = list, pagination = pageturn, pass = true }, JsonRequestBehavior.AllowGet);
        }
        [Power(IsSuper = false, IsShow = false, PowerId = "Blog_F102", PowerName = "评论管理", PowerDes = "评论管理")]
        public ActionResult CommentDelete(string ids)
        {
            try
            {
                this.commentService.Delete(ids);
                return Json(new ResultModel { msg = "删除成功", pass = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new ResultModel { msg = "删除失败", pass = false }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
