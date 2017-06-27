using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper.Common;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.IService.Store;

namespace Epoint.PingBiao.MySQLService.Store
{
    public class OrderService : IOrderService
    {
        public bool UpdateState(int OrderId, int state)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_order SET State=@State WHERE Order_Id =@OrderId", new { OrderId = OrderId, State = state });
                if (i > 0) { return true; } else { return false; }
            }
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="TrackingNumber"></param>
        /// <returns></returns>
        public bool BIsDeliver(int OrderId, string TrackingNumber)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_order SET IsDeliver=@IsDeliver,IsReceipt=@IsReceipt,TrackingNumber=@TrackingNumber,DeliverTime=@DeliverTime,State=@State WHERE Order_Id =@OrderId",
                    new { OrderId = OrderId, State = 2, IsDeliver = true, IsReceipt=false, TrackingNumber = TrackingNumber, DeliverTime = DateTime.Now });
                if (i > 0) { return true; } else { return false; }
            }
        }
        /// <summary>
        /// 收货
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="Commont"></param>
        /// <param name="isOk"></param>
        /// <returns></returns>
        public bool BIsReceipt(int OrderId, string Comment, bool isOk)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_order SET IsReceipt=@IsReceipt,Comment=@Comment,ReceiptTime=@ReceiptTime,State=@State WHERE Order_Id =@OrderId",
                    new { OrderId = OrderId, IsReceipt = true, Comment = Comment, ReceiptTime = DateTime.Now, State = isOk ? 3 : 4 });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool Insert(Model.Store.Order entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("INSERT INTO store_order(ItemId,MemberId,Price,Num,Des,Address,Phone,State,AddTime,IsDeliver,TrackingNumber,DeliverTime,IsReceipt)" +
                    "VALUES(@ItemId,@MemberId,@Price,@Num,@Des,@Address,@Phone,@State,@AddTime,@IsDeliver,@TrackingNumber,@DeliverTime,@IsReceipt)", entity);
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool InsertBatch(List<Model.Store.Order> datas)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.Store.Order entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute("UPDATE store_order SET ItemId=@ItemId,Price=@Price,Num=@Num,Des=@Des,Address=@Address,Phone=@Phone,State=@State,AddTime=@AddTime,IsDeliver=@IsDeliver,TrackingNumber=@TrackingNumber,DeliverTime=@DeliverTime,IsReceipt=@IsReceipt WHERE Order_Id =@Order_Id", entity);
            }
        }

        public void Delete(string ids)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute(string.Format("DELETE FROM store_order WHERE Order_Id in ({0})", ids));
            }
        }

        public List<Model.Store.Order> GetAll()
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                return conn.Query<Model.Store.Order>("select * from store_order").ToList();
            }
        }

        public List<Model.Store.Order> ToPagedList(int pageIndex, int pageSize, string keySelector, out int count)
        {
            //using (IDbConnection conn = SqlString.GetMySqlConnection())
            //{
            //    string sql = "select a.*,b.Item_Id,b.Title,d.Member_Id,d.LoginName from store_order as a LEFT JOIN store_item as b on a.ItemId=b.Item_Id LEFT JOIN store_member as d on a.MemberId=d.Member_Id";
            //    string sql01 = "select count(*) from store_order";
            //    count = conn.Query<int>(sql01).SingleOrDefault();
            //    string query = "SELECT * from (" + sql + ")as c ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize;
            //    return conn.Query<Model.Store.Order, Model.Store.Item, Model.Store.Member, Model.Store.Order>(query, (order, item, member) =>
            //    {
            //        if (item != null)
            //            order.Item = item;
            //        if (member != null)
            //            order.Member = member;
            //        return order;
            //    }, null, null, "Item_Id,Member_Id", null, null).ToList();
            //}
            return this.ToPagedList(pageIndex, pageSize, "", keySelector, out count);
        }

        public List<Model.Store.Order> ToPagedList(int pageIndex, int pageSize,string whereStr, string keySelector, out int count)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string sql = "select a.*,b.Item_Id,b.Title,d.Member_Id,d.LoginName from store_order as a LEFT JOIN store_item as b on a.ItemId=b.Item_Id LEFT JOIN store_member as d on a.MemberId=d.Member_Id";
                string sql01 = "select count(*) from store_order";
                if (string.IsNullOrEmpty(whereStr) == false) {
                    sql01 += " where " + whereStr;
                }
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c {0} ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," +pageIndex* pageSize;
                if (string.IsNullOrEmpty(whereStr) == false)
                {
                    query = string.Format(query, " where " + whereStr);
                }
                else {
                    query = string.Format(query, "");
                }
                return conn.Query<Model.Store.Order, Model.Store.Item, Model.Store.Member, Model.Store.Order>(query, (order, item, member) =>
                {
                    if (item != null)
                        order.Item = item;
                    if (member != null)
                        order.Member = member;
                    return order;
                }, null, null, "Item_Id,Member_Id", null, null).ToList();
            }
        }

        public Model.Store.Order Find(int id)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string query = "select a.*,b.Item_Id,b.Title,d.Member_Id,d.LoginName from store_order as a LEFT JOIN store_item as b on a.ItemId=b.Item_Id LEFT JOIN store_member as d on a.MemberId=d.Member_Id where a.Order_Id = @id";
                return conn.Query<Model.Store.Order, Model.Store.Item, Model.Store.Member, Model.Store.Order>(query,(order, item, member) =>
                {
                    if (item != null)
                        order.Item = item;
                    if (member != null)
                        order.Member = member;
                    return order;
                }, new { id = id }, null, "Item_Id,Member_Id", null, null).SingleOrDefault();
            }
        }

        public Model.Store.Order Find(int id, int MemberId)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string query = "select a.*,b.Item_Id,b.Title,d.Member_Id,d.LoginName from store_order as a LEFT JOIN store_item as b on a.ItemId=b.Item_Id LEFT JOIN store_member as d on a.MemberId=d.Member_Id where a.Order_Id = @id and a.MemberId=@MemberId";
                return conn.Query<Model.Store.Order, Model.Store.Item, Model.Store.Member, Model.Store.Order>(query, (order, item, member) =>
                {
                    if (item != null)
                        order.Item = item;
                    if (member != null)
                        order.Member = member;
                    return order;
                }, new { id = id, MemberId = MemberId }, null, "Item_Id,Member_Id", null, null).SingleOrDefault();
            }
        }
    }
}
