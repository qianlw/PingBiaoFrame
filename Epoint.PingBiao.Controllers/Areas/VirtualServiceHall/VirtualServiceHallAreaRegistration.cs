using System.Web.Mvc;

namespace Epoint.PingBiao.Controllers.Areas.VirtualServiceHall
{
    public class VirtualServiceHallAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VirtualServiceHall";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "VirtualServiceHall_default",
                "VirtualServiceHall/{controller}/{action}/{id}",
                new { controller = "BiaoDuan", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Epoint.PingBiao.Controllers.Areas.VirtualServiceHall.Controllers" }//指定寻找的命名空间
            );
        }
    }
}
