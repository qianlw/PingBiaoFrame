
using System;
namespace Epoint.PingBiao.Model.Sys
{
    /// <summary>
    /// 角色
    /// </summary>
    [Serializable()]
    public class Role
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Role_Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        public string ActionIds { get; set; }
    }
}
