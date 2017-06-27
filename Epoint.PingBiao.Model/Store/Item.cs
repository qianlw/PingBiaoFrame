using System;

namespace Epoint.PingBiao.Model.Store
{
    public class Item
    {
        public Item() { }
        public int Item_Id { get; set; }
        /// <summary>
        /// 产品主图
        /// </summary>
        public string Item_Img { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 类目
        /// </summary>
        public Category Category { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 商品状态（在售，停售，缺货，下架）
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 显示标示（推荐，非推荐）
        /// </summary>
        public bool Flag { get; set; }
    }
}
