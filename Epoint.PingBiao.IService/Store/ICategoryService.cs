
namespace Epoint.PingBiao.IService.Store
{
    public interface ICategoryService : IBaseService<Epoint.PingBiao.Model.Store.Category>
    {
        /// <summary>
        /// 根据分组编号获取分类
        /// </summary>
        /// <param name="groudId"></param>
        /// <returns></returns>
        System.Collections.Generic.List<Model.Store.Category> GetList(int groudId);
    }
}
