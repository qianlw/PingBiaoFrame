using System.Collections.Generic;

namespace Epoint.PingBiao.IService.Store
{
    public interface IItemService : IBaseService<Epoint.PingBiao.Model.Store.Item>
    {
        /// <summary>
        /// 显示标示（推荐，非推荐）
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        bool UpdateFlag(int ItemId,bool flag);
        /// <summary>
        /// 商品状态（在售，停售，缺货，下架）
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        bool UpdateState(int ItemId, int state);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateStock(int ItemId, int Stock);
        /// <summary>
        /// 是否在使用CategoryId
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        bool IsUseCategory(int CategoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="where"></param>
        /// <param name="keySelector"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<Epoint.PingBiao.Model.Store.Item> ToPagedList(int pageIndex, int pageSize, string where, string keySelector, out int count);
    }
}
