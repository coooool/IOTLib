using IOTLib.Extend;
using System;
using System.Collections.Generic;
using System.Text;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [MonoHeaderHelp("使物体自动锁定到地面，如果没有碰撞则保持不动")]
    [BindSystem(typeof(PhysicsSystem))]
    public class PositionToGroundData :　DataBehaviour
    {
        [Header("触碰到地面后的Y偏移,若模型原点在中心，这通常是一个半径值")]
        public float m_Offset;

        [Header("射线所有层")]
        public bool use_all_layer = false;

        [Header("射线碰撞层，可以剔除不进行检测的物体")]
        public LayerMask m_layer = ~0;
      
        public LayerMask layer
        {
            get
            {
                if (use_all_layer) return LayerUtility.AllMaskLayer;

                return m_layer;
            }
            set
            {
                Debug.Log("通常还需要设置use_all_layer为True才能对指定层屏蔽");
                m_layer = value;
            }
        }

        protected override void OnAwake()
        {
            if(LayerUtility.GetGroundLayer(out var g))
            {
                m_layer = g;
            }
        }

        void OnDrawGizmosSelected()
        {
            var drawPos = transform.position;
            drawPos.y -= m_Offset;
            Gizmos.DrawWireSphere(drawPos, .1f);
        }

        /// <summary>
        /// 设置偏移来自一个GameObject的半径大小
        /// </summary>
        /// <param name="target"></param>
        public void SetOffsetFromModelBoundsRadius(GameObject target)
        {
            var size = BoundsUtility.GetBoundsWithChildren(target);
            m_Offset = size.size.y / 2;
        }

        /// <summary>
        /// 计算目标的包围体后回调一次用户可以自定义修改偏移
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="convert">自定义转换,如果需要半径可以返回Size.y/2</param>
        public void SetOffsetFromModelBoundsRadius(GameObject target, System.Func<Bounds,float> convert)
        {
            var size = BoundsUtility.GetBoundsWithChildren(target);
            m_Offset = convert(size);
        }
    }
}
