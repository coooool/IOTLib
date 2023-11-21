using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 2D的CG资源塑料的 
    /// </summary>
    [CGEditor(nameof(T2DModel), "2DLayer")]
    [CGCustomEditor(typeof(T2DEditor))]
    [RequireComponent(typeof(ExportCGPrefab))]
    public class T2DModel : MonoBehaviour, IDEPropertyEvent
    {
        // 锚点
        [CGVar("布局坐标锚点")]
        public byte m_Anchor = 1;

        // 对齐屏幕宽度， 例如顶部菜单栏可能会使用这个
        [CGVar("对齐屏幕宽")]
        public bool m_align_screen_x;
        // 对齐屏幕高度
        [CGVar("对齐屏幕高")]
        public bool m_align_screen_y;
        
        #region 编辑器
        ExportCGPrefab CGPrefab { get; set; }
        #endregion

        public void Create2DPos(ExportCGPrefab cgPrefab)
        {
            if (cgPrefab.UIRecttransform == null)
            {
                Debug.LogError("恢复一个空的变换");
                return;
            }

            cgPrefab.UIRecttransform.sizeDelta = cgPrefab.transform.localScale;
            SetAnchor(this.m_Anchor, cgPrefab.UIRecttransform);
            cgPrefab.UIRecttransform.anchoredPosition = cgPrefab.transform.position;
        }

        /// <summary>
        /// 根据锚点设置坐标
        /// </summary>
        /// <param name="anchor">1-9之间</param>
        /// <param name="rt">目标</param>
        public static void SetAnchor(int anchor, RectTransform rt)
        {
            switch (anchor)
            {
                case 1:
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 2:
                    rt.anchorMin = new Vector2(0.5f, 1); ;
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 3:
                    rt.anchorMin = Vector3.one;
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 4:
                    rt.anchorMin = new Vector2(0, .5f);
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 5:
                    rt.anchorMin = new Vector2(.5f, .5f);
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 6:
                    rt.anchorMin = new Vector2(1, .5f); ;
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 7:
                    rt.anchorMin = Vector3.zero;
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 8:
                    rt.anchorMin = new Vector2(0.5f, 0);
                    rt.anchorMax = rt.anchorMin;
                    break;

                case 9:
                    rt.anchorMin = new Vector2(1, 0);
                    rt.anchorMax = rt.anchorMin;
                    break;
            }
        }

        void OnEnable()
        {
            CGPrefab = GetComponent<ExportCGPrefab>();
        }

        void OnDestroy()
        {
            CGPrefab.UIRecttransform = null;
        }

        void OnDragUpdate(Vector2 mouse_pos)
        {
            CGPrefab.UIRecttransform.position = mouse_pos;
        }

        /// <summary>
        /// 这个tar是3d映射物体
        /// </summary>
        /// <param name="tar"></param>
        public void OnTransformUpdate(Transform tar)
        {
            OnDragUpdate(tar.position);
        }
    }
}
