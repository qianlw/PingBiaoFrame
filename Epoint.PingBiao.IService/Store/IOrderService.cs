using System.Collections.Generic;

namespace Epoint.PingBiao.IService.Store
{
    public interface IOrderService : IBaseService<Epoint.PingBiao.Model.Store.Order>
    {
        Epoint.PingBiao.Model.Store.Order Find(int OrderId, int MemberId);
        /// <summary>
        /// 交易状态(下单成功，卖家发货，交易成功，交易失败)
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateState(int OrderId,int state);
        /// <summary>
        /// 是否发货
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="on_off"></param>
        /// <param name="TrackingNumber">快递单号</param>
        /// <returns></returns>
        bool BIsDeliver(int OrderId,string TrackingNumber);
        /// <summary>
        /// 是否收货
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="on_off"></param>
        /// <param name="Commont">评价</param>
        /// <returns></returns>
        bool BIsReceipt(int OrderId, string Comment, bool isOk);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereStr"></param>
        /// <param name="keySelector"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<Epoint.PingBiao.Model.Store.Order> ToPagedList(int pageIndex, int pageSize, string whereStr, string keySelector, out int count);
    }
}
