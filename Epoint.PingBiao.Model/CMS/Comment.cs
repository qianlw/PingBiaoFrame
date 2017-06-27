﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoint.PingBiao.Model.CMS
{
    public class Comment
    {
        public Comment()
		{
			
		}
        /// <summary>
        /// 
        /// </summary>
        public int Comment_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
