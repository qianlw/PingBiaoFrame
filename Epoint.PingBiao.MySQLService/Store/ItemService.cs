using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Common;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService;
using Epoint.PingBiao.IService.Store;
using System.Data;

namespace Epoint.PingBiao.MySQLService.Store
{
    public class ItemService : IItemService
    {
        public bool UpdateFlag(int ItemId, bool flag)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_item SET Flag=@Flag WHERE Item_Id =@ItemId", new { ItemId = ItemId, Flag = flag });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool UpdateState(int ItemId, int state)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_item SET State=@State WHERE Item_Id =@ItemId", new { ItemId = ItemId, State = state });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool UpdateStock(int ItemId, int Stock)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("UPDATE store_item SET Stock=@Stock WHERE Item_Id =@ItemId", new { ItemId = ItemId, Stock = Stock });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool IsUseCategory(int CategoryId)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i=conn.Query<int>("SELECT CategoryId FROM store_item as a WHERE a.CategoryId=@CategoryId LIMIT 0, 1", new { CategoryId = CategoryId }).SingleOrDefault();
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool Insert(Model.Store.Item entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("INSERT INTO store_item(Item_Img,Title,CategoryId,Content,Price,Stock,AddTime,State,Flag)" +
                    "VALUES(@Item_Img,@Title,@CategoryId,@Content,@Price,@Stock,@AddTime,@State,@Flag)", entity);
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool InsertBatch(List<Model.Store.Item> datas)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.Store.Item entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute("UPDATE store_item SET Item_Img=@Item_Img,Title=@Title,CategoryId=@CategoryId,Content=@Content,Price=@Price,Stock=@Stock,AddTime=@AddTime,State=@State,Flag=@Flag WHERE Item_Id =@Item_Id", entity);
            }
        }

        public void Delete(string ids)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute(string.Format("DELETE FROM store_item WHERE Item_Id in ({0})", ids));
            }
        }

        public List<Model.Store.Item> GetAll()
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                return conn.Query<Model.Store.Item>("select * from store_item").ToList();
            }
        }

        public List<Model.Store.Item> ToPagedList(int pageIndex, int pageSize, string keySelector, out int count)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string sql = "SELECT * FROM store_item as a LEFT JOIN store_category as b on a.CategoryId=b.Category_Id";
                string sql01 = "select count(*) from store_item";
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageIndex* pageSize;
                return conn.Query<Model.Store.Item, Model.Store.Category, Model.Store.Item>(query, (item, category) =>
                {
                    if (category != null)
                        item.Category = category;
                    return item;
                }, null, null, "Category_Id", null, null).ToList();
            }
        }

        public List<Epoint.PingBiao.Model.Store.Item> ToPagedList(int pageIndex, int pageSize, string where, string keySelector, out int count)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string sql = "SELECT * FROM store_item as a LEFT JOIN store_category as b on a.CategoryId=b.Category_Id";
                string sql01 = "select count(*) from store_item " + where;
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c "+where+" ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," +pageIndex* pageSize;
                return conn.Query<Model.Store.Item, Model.Store.Category, Model.Store.Item>(query, (item, category) =>
                {
                    if (category != null)
                        item.Category = category;
                    return item;
                }, null, null, "Category_Id", null, null).ToList();
            }
        }

        public Model.Store.Item Find(int id)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string query = "select a.* from store_item as a WHERE a.Item_Id = @id";
                return conn.Query<Model.Store.Item>(query, new { id = id }).SingleOrDefault();
            }
        }
    }
}
