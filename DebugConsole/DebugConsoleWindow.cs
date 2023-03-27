using System;
using System.Collections;
using System.Collections.Generic;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [BindSystem(typeof(DebugConsoleSystem))]
    public class DebugConsoleWindow : DataBehaviour
    {
        private Rect windowRect;
        private DebugConsoleSystem _refSys;

        void Start()
        {
            windowRect.x = (Screen.width / 2) - 400;
            windowRect.y = (Screen.height / 2) - 320;
            windowRect.width = 800;
            windowRect.height = 640;

            _refSys = SystemManager.GetSystem<DebugConsoleSystem>();
        }

        private void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, WindowUpdate, "µ÷ÊÔ¿ØÖÆÌ¨(F12)");
        }

        Vector2 scrollView;

        void WindowUpdate(int id)
        {
            if (GUILayout.Button("Clear"))
            {
                _refSys.Clear();
                return;
            }

            GUILayout.BeginVertical("box");

            scrollView = GUILayout.BeginScrollView(scrollView);

            foreach (var node in _refSys.errorInfoNodes)
            {
                switch (node.type)
                {
                    case LogType.Log:
                        GUI.backgroundColor = Color.white;
                        break;
                    case LogType.Warning:
                        GUI.backgroundColor = Color.yellow;
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                        GUI.backgroundColor = Color.red;
                        break;
                }

                GUILayout.TextArea($"{node.time.ToLocalTime()}:\n{node.Conditioin}");

                GUI.backgroundColor = Color.white;
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    }
}