using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.UserSystem
{
    public struct UserDataJPathConfig
    {
        /// <summary>
        /// 用户名路径
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 头像字段
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// 过期时间路径
        /// </summary>
        public string ExpireTime { get; set; }

        /// <summary>
        /// 性别字段
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 年龄字段 
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 用户授权Token
        /// </summary>
        public string Authorization { get; set; }

        public string UserId { get; set; }

        public void Reset()
        {
            Authorization = string.Empty;
            Age = string.Empty;
            UserName= string.Empty;
            Picture= string.Empty;
            ExpireTime = string.Empty;
            Sex = string.Empty;
            UserType= string.Empty;
        }
    }
}
