using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public class CGGUIUtility
    {

        /// <summary>
        /// һ��֧�����¼���ͼƬ��ť
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="style"></param>
        /// <param name="layoutOptions"></param>
        /// <returns></returns>
        public static bool AutoLayoutDragButton(Texture texture, string style, params GUILayoutOption[] layoutOptions)
        {
            GUILayout.Box(texture, style, layoutOptions);
            
            var control_rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());

            if (Event.current.type == EventType.MouseDown)
            {
                var gui_mousepos = Input.mousePosition;
                gui_mousepos.y = Screen.height - gui_mousepos.y;

                if (control_rect.Contains(gui_mousepos))
                {
                    Event.current.Use();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ������ֻ�ܵ�����GUI��
        /// </summary>
        /// <param name="winRect"></param>
        /// <returns></returns>
        public static bool TestMouseInWindow(Rect winRect)
        {
            var gui_mousepos = Input.mousePosition;
            gui_mousepos.y = Screen.height - gui_mousepos.y;

            if (winRect.Contains(gui_mousepos))
            {
                return true;
            }

            return false;
        }
    }
}