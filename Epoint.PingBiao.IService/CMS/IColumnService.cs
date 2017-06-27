﻿using System;

namespace Epoint.PingBiao.IService.CMS
{
    public interface IColumnService : IBaseService<Epoint.PingBiao.Model.CMS.Column>
	{
        /// <summary>
        /// 判断栏目名是否正在使用
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        bool IsUseColumnName(string columnName, int ignoreId);
	}
}

