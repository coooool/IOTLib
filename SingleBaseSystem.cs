using System;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 单一的实例系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleBaseSystem<T> : BaseSystem where T: class
    {
        private static T? m_Install = null;

        protected static T Install
        {
            get
            {
                if(m_Install == null)
                {
                    throw new InvalidProgramException(String.Format("实例:{0}还没有初始化。", typeof(T).Name));
                }

                return m_Install;
            }
        }

        /// <summary>
        /// 系统实例
        /// </summary>
        /// <returns></returns>
        public static T GetInstall()
        {
            return Install;
        }

        public override void OnCreate()
        {
            m_Install = this as T;
        }
    }
}
