using System;

namespace Epoint.PingBiao.Model.Store
{
    public class Order
    {
        public Order() { }
        public int Order_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Item Item { get; set; }
        /// <summary>
        /// 当时购买价格（单价）
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 购买数
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 留言
        /// </summary>
        public string Des { get; set; }
        /// <summary>
        /// 买家地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 买家联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 交易状态(下单成功1，卖家发货2，交易成功3，交易失败4)
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否发货
        /// </summary>
        public bool IsDeliver { get; set; }
        /// <summary>
        /// 快递查询号
        /// </summary>
        public string TrackingNumber { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime DeliverTime { get; set; }
        /// <summary>
        /// 是否收货
        /// </summary>
        public bool IsReceipt { get; set; }
        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime ReceiptTime { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 会员
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Member Member { get; set; }
    }
}
