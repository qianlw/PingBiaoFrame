using System;

namespace Epoint.PingBiao.Model.Store
{
    public class Member
    {
        public Member() { }
        public int Member_Id { get; set; }
        /// <summary>
        /// 登陆账户名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 最好登陆时间
        /// </summary>
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarPath { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 状态(允许登陆，不允许登陆)
        /// </summary>
        public int State { get; set; }
    }
}
