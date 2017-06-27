using System;

namespace Epoint.PingBiao.IService.Store
{
    public interface IMemberService : IBaseService<Epoint.PingBiao.Model.Store.Member>
    {
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        bool ResetPassword(int MemberId,string pwd);
        /// <summary>
        /// 状态(允许登陆，不允许登陆)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool UpdateState(int MemberId, int state);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool UpdateLastTime(int Member_Id, DateTime time);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        Epoint.PingBiao.Model.Store.Member CheckLogin(string userName, string pwd);
    }
}
