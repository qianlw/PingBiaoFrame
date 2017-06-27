﻿using Epoint.PingBiao.Common;
using Dapper.Common;
using Epoint.PingBiao.IService.CMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Epoint.PingBiao.SQLiteService.CMS
{
    public class ColumnService : IColumnService
	{
        public ColumnService()
		{
		}
        public bool IsUseColumnName(string columnName, int ignoreId)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Query<int>("SELECT c.Column_Id FROM cms_column as c WHERE c.ColumnName==@columnName and c.Column_Id not in (@ignoreId)  LIMIT 0,1", new { columnName = columnName, ignoreId = ignoreId }).SingleOrDefault();
                //是否存在
                if (i > 0) { return true; } else { return false; }
            }
        }
        public bool Insert(Model.CMS.Column entity)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Execute("INSERT INTO cms_column(Sort,ColumnName)" +
                    "VALUES(@Sort,@ColumnName)", entity);
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool InsertBatch(System.Collections.Generic.List<Model.CMS.Column> datas)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.CMS.Column entity)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                conn.Execute("UPDATE cms_column SET Sort=@Sort,ColumnName=@ColumnName WHERE Column_Id =@Column_Id", entity);
            }
        }

        public void Delete(string ids)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                conn.Execute(string.Format("DELETE FROM cms_column WHERE Column_Id in ({0})", ids));
            }
        }

        public System.Collections.Generic.List<Model.CMS.Column> GetAll()
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                return conn.Query<Model.CMS.Column>("select * from cms_column").ToList();
            }
        }

        public System.Collections.Generic.List<Model.CMS.Column> ToPagedList(int pageIndex, int pageSize, string keySelector, out int count)
        {
            /*
                * firstIndex:起始索引
                * pageSize:每页显示的数量
                * orderColumn:排序的字段名
                * sql:可以是简单的单表查询语句，也可以是复杂的多表联合查询语句
            */
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                string sql = "cms_column";
                string sql01 = "select count(*) from cms_column";
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize;
                return conn.Query<Model.CMS.Column>(query).ToList();
            }
        }

        public Model.CMS.Column Find(int id)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                string query = "select a.* from cms_column as a WHERE a.Column_Id = @id";
                return conn.Query<Model.CMS.Column>(query,new { id = id }).SingleOrDefault();
            }
        }
    }
}

