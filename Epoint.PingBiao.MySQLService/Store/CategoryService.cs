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
    public class CategoryService : ICategoryService
    {
        public List<Model.Store.Category> GetList(int groudId)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string query = "select a.* from store_category as a WHERE a.GroupCode = @id";
                return conn.Query<Model.Store.Category>(query, new { id = groudId });
            }
        }

        public bool Insert(Model.Store.Category entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                int i = conn.Execute("INSERT INTO store_category(GroupCode,CategoryName)" +
                    "VALUES(@GroupCode,@CategoryName)", entity);
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool InsertBatch(List<Model.Store.Category> datas)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.Store.Category entity)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute("UPDATE store_category SET GroupCode=@GroupCode,CategoryName=@CategoryName WHERE Category_Id =@Category_Id", entity);
            }
        }

        public void Delete(string ids)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                conn.Execute(string.Format("DELETE FROM store_category WHERE Category_Id in ({0})", ids));
            }
        }

        public List<Model.Store.Category> GetAll()
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                return conn.Query<Model.Store.Category>("select * from store_category").ToList();
            }
        }

        public List<Model.Store.Category> ToPagedList(int pageIndex, int pageSize, string keySelector, out int count)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string sql = "store_category";
                string sql01 = "select count(*) from store_category";
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," +pageIndex* pageSize;
                return conn.Query<Model.Store.Category>(query).ToList();
            }
        }

        public Model.Store.Category Find(int id)
        {
            using (IDbConnection conn = SqlString.GetMySqlConnection())
            {
                string query = "select a.* from store_category as a WHERE a.Category_Id = @id";
                return conn.Query<Model.Store.Category>(query, new { id = id }).SingleOrDefault();
            }
        }
    }
}
