using System.Web.Mvc;

namespace Epoint.PingBiao.Controllers.Areas.PingBiao
{
    public class PingBiaoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PingBiao";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PingBiao_default",
                "PingBiao/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Epoint.PingBiao.Controllers.Areas.PingBiao.Controllers" }//指定寻找的命名空间
            );
        }
    }
}
