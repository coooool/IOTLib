using Cysharp.Threading.Tasks;
using IOTLib.Configure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [GameSystem(AlwaysRun = true)]
    [SystemDescribe(Author = "昊",
       Dependent = "无",
       Describe = "内联调试输出控制台,在变量系统中添加debug_log为true开启本系统",
       Name = "DebugConsole",
       Version = "0.1")]
    public class DebugConsoleSystem : BaseSystem
    {
        internal struct ErrorInfoNode
        {
            public string Conditioin;
            public LogType type;
            public DateTime time;
        }

        internal Stack<ErrorInfoNode> errorInfoNodes = new Stack<ErrorInfoNode>(6);

        void ReceiveLog(string condition, string stackTrace, LogType type)
        {
            if(errorInfoNodes.Count > 99)
            {
                errorInfoNodes.Clear();
            }

            errorInfoNodes.Push(new ErrorInfoNode()
            {
                Conditioin = condition,
                type = type,
                time = DateTime.Now
            });

            if (type == LogType.Error)
            {
                ShowWindow(false);
            }
        }

        void ShowWindow(bool allowClose)
        {
            var dcw = GetBindData<DebugConsoleWindow>();
            if(dcw == null)
            {
                var consoleDebugWindow = new GameObject("_debug_console_");
                consoleDebugWindow.AddComponent<DebugConsoleWindow>();
                consoleDebugWindow.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
                if (allowClose)
                {
                    GameObject.Destroy(dcw.gameObject);
                }
            }
        }

        internal void Clear()
        {
            errorInfoNodes.Clear();
        }

        public override void OnUpdate()
        {
            if (!Application.isEditor)
            {
                if (Input.GetKeyUp(KeyCode.F12))
                {
                    ShowWindow(true);
                }
            }
        }

        public override void OnDrop()
        {
            if (!Application.isEditor)
            {
                Application.logMessageReceived -= ReceiveLog;
            }
        }

        public override void OnCreate()
        {
            if (!Application.isEditor)
            {
                if (DBServer.GetVar("debug_log", false))
                {
                    Application.logMessageReceived += ReceiveLog;
                }
            }
        }
    }
}
