using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper.Common;
using Epoint.PingBiao.Common;
using Epoint.PingBiao.IService.Store;

namespace Epoint.PingBiao.SQLiteService.Store
{
    public class MemberService : IMemberService
    {
        public Epoint.PingBiao.Model.Store.Member CheckLogin(string userName, string pwd)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                string query = "select a.* from store_member as a WHERE a.LoginName = @Name and a.Password=@Password and a.State=1";
                return conn.Query<Model.Store.Member>(query, new { Name = userName, Password = CryptoHelper.MD5Encrypt(pwd) }).SingleOrDefault();
            }
        }

        public bool ResetPassword(int MemberId, string pwd)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Execute("UPDATE store_member SET Password=@Password WHERE Member_Id =@Member_Id", new { Member_Id = MemberId, Password = pwd });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool UpdateState(int MemberId, int state)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Execute("UPDATE store_member SET State=@State WHERE Member_Id =@Member_Id", new { Member_Id = MemberId, State = state });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool Insert(Model.Store.Member entity)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Execute("INSERT INTO store_member(LoginName,Password,AddTime,LastTime,AvatarPath,Phone,State)" +
                    "VALUES(@LoginName,@Password,@AddTime,@LastTime,@AvatarPath,@Phone,@State)", entity);
                if (i > 0) { return true; } else { return false; }
            }
        }

        public bool InsertBatch(List<Model.Store.Member> datas)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.Store.Member entity)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                conn.Execute("UPDATE store_member SET AvatarPath=@AvatarPath,Phone=@Phone,Password=@Password,AddTime=@AddTime,State=@State,LastTime=@LastTime,LoginName=@LoginName WHERE Member_Id =@Member_Id", entity);
            }
        }

        public bool UpdateLastTime(int Member_Id, DateTime time)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                int i = conn.Execute("UPDATE store_member SET LastTime=@LastTime WHERE Member_Id =@Member_Id", new { Member_Id = Member_Id, LastTime = time });
                if (i > 0) { return true; } else { return false; }
            }
        }

        public void Delete(string ids)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                conn.Execute(string.Format("DELETE FROM store_member WHERE Member_Id in ({0})", ids));
            }
        }

        public List<Model.Store.Member> GetAll()
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                return conn.Query<Model.Store.Member>("select * from store_member").ToList();
            }
        }

        public List<Model.Store.Member> ToPagedList(int pageIndex, int pageSize, string keySelector, out int count)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                string sql = "store_member";
                string sql01 = "select count(*) from store_member";
                count = conn.Query<int>(sql01).SingleOrDefault();

                string query = "SELECT * from (" + sql + ")as c ORDER BY " + keySelector + " LIMIT " + (pageIndex - 1) * pageSize + "," + pageSize;
                return conn.Query<Model.Store.Member>(query).ToList();
            }
        }

        public Model.Store.Member Find(int id)
        {
            using (IDbConnection conn = SqlString.GetSQLiteConnection())
            {
                string query = "select a.* from store_member as a WHERE a.Member_Id = @id";
                return conn.Query<Model.Store.Member>(query, new { id = id }).SingleOrDefault();
            }
        }
    }
}
