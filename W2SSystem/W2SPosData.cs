using System;
using System.Collections.Generic;
using System.Text;
using UMOD;
using UnityEngine;

namespace IOTLib.UIPlus
{
    /// <summary>
    /// 世界坐标转屏幕坐标的代理
    /// </summary>
    [BindSystem(typeof(W2SPosSystem))]
    [MonoHeaderHelp("从世界坐标转换到到屏幕坐标，通常用于UI图标定位")]
    public class W2SPosData : DataBehaviour
    {
        public enum W2SPosTypeEnum
        {
            // 世界到屏幕
            W2S = 1,
            // 屏幕到世界
            S2W
        }

        /// <summary>
        /// 转换方法
        /// </summary>
        public W2SPosTypeEnum ConvertMethod = W2SPosTypeEnum.S2W;

        /// <summary>
        /// 看不见时隐藏UI
        /// </summary>
        [Header("未出现在摄像机时禁用物体")]
        public bool CullingHide = true;

        /// <summary>
        /// 根据这个位置进行转换
        /// </summary>
        private Vector3 m_Position;
        
        /// <summary>
        /// 参考位置来源
        /// </summary>
        public Vector3 Position
        {
            get
            {
                if(PositionSource == null)
                {
                    return m_Position + Offset;
                }

                return PositionSource.position + Offset;
            }
            set
            {
                m_Position = value;
            }
        }

        /// <summary>
        /// 如果设置了来源，则会使用此目标的位置
        /// </summary>
        public Transform PositionSource;

        /// <summary>
        /// 坐标偏移，在转换前进行偏移
        /// </summary>
        public Vector3 Offset;

        //void Update()
        //{
        //    if(ConvertMethod == W2SPosTypeEnum.W2S)
        //    {
        //        var viewPos = UnityEngine.Camera.main.WorldToViewportPoint(Position);

        //        if(viewPos.x< 0 || viewPos.x > 1) 
        //        {
        //            gameObject.SetActive(false);
        //        }
        //        else if (viewPos.y < 0 || viewPos.y > 1)
        //        {
        //            gameObject.SetActive(false);
        //        }
        //    }
        //}

        //void UpdatePos()
        //{
        //    if (CullingHide && gameObject.activeInHierarchy)
        //    {
        //        gameObject.SetActive(false);
        //    }
        //    else if (CullingHide && !gameObject.activeInHierarchy)
        //    {
        //        gameObject.SetActive(true);
        //    }
        //}

        //void OnBecameInvisible()
        //{
        
            
        //}

        //void OnBecameVisible()
        //{
          
        //}
    }
}
