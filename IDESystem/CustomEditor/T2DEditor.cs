using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    internal class T2DEditor : CGCustomEditor
    {
        T2DModel? m_Model;

        public override void OnCGFieldUpdate()
        {
            m_Model.m_Anchor = (byte)Mathf.Clamp(m_Model.m_Anchor, 1, 9);

            if(m_Model.TryGetComponent<ExportCGPrefab>(out var rt))
            {
                UpdateAnchor(rt.UIRecttransform);
                UpdateSize(rt.UIRecttransform);
            }
        }

        void UpdateAnchor(RectTransform rt)
        {
            var oldPos = rt.position;

            T2DModel.SetAnchor(m_Model.m_Anchor, rt);

            rt.position = oldPos;   
        }

        void UpdateSize(RectTransform content)
        {
            //var newSize = content.sizeDelta;

            if (m_Model.m_align_screen_x)
            {
                //newSize.x = Screen.width;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            }

            if(m_Model.m_align_screen_y)
            {
                //newSize.y = Screen.height;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
            }
        }

        public override void OnEnable()
        {
            m_Model = target as T2DModel;
        }

        public override void OnGUI(Behaviour instance, IEnumerable<CGVar> vars)
        {
            GUILayout.Label("布局锚点能自适应屏幕分辨率\n1：左角，2：上，3：右上\n4：左，5：中，6：右\n7：左下，8：下，9：右下");
            base.OnGUI(instance, vars);
        }
    }
}
