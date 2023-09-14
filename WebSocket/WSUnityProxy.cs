using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UMOD;
using UnityEngine;
using WebSocketSharp;

namespace IOTLib
{
    [BindSystem(typeof(WebSocketSystem))]
    public class WSUnityProxy : DataBehaviour
    {
        /// <summary>
        /// 连接URL
        /// </summary>
        public string m_Url;
        /// <summary>
        /// 是否输出调试信息
        /// </summary>
        public bool m_Debug;
        /// <summary>
        /// 心跳包发送时间
        /// </summary>
        public int m_heartTime = 5;

        private WebSocket client = null;

        public string m_HeartText = "{\"heart\":true}";

        void Start()
        {
            // 开启自动心跳包
            OpenSendHeartInfo();
            OpenRetryConnection();

            OnStart();
        }

        protected virtual void OnStart()
        {

        }

        internal void _Connect(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            m_Url = url;

            if(client != null)
            {
                switch(client.ReadyState)
                {
                    case WebSocketState.Connecting:
                    case WebSocketState.Open:
                        Debug.LogWarning("实例已经在连接中...");
                        return;
                }
            }

            client = new WebSocket(url);
            client.OnOpen += _ConnectSuccess;
            client.OnMessage += _Message;
            client.OnClose += _CloseConnetion;
            client.OnError += _Error;

            client.ConnectAsync(); 
        }

        internal void _CloseConnect()
        {
            if (client != null)
            {
                if(client.ReadyState == WebSocketState.Open || client.ReadyState == WebSocketState.Connecting)
                {
                    client.Close(CloseStatusCode.Abnormal);
                }

                client = null;
            }
        }

        void _ConnectSuccess(object sender, EventArgs e)
        {
            UniTask.Void(async () =>
            {
                await UniTask.SwitchToMainThread();
                OnSuccess();
            });
        }

        void _Message(object sender, MessageEventArgs arg)
        {
            if (arg.IsPing)
            {
                if (m_Debug)
                    Debug.Log("===收到WebSocket心跳包===");

                return;
            }

            UniTask.Void(async () =>
            {
                await UniTask.SwitchToMainThread();

                if (m_Debug)
                {
                    Debug.Log(arg.Data);
                }

                OnMessage(arg.Data);
            });
        }


        void _Error(object sender, ErrorEventArgs e)
        {
            UniTask.Void(async () =>
            {
                await UniTask.SwitchToMainThread();    
                Debug.LogError($"WebSocket发生错误:{e.Message}");
            });
        }

        void _CloseConnetion(object sender, CloseEventArgs arg)
        {
            // 断开链接
            _CloseConnect();

            OnClose();
        }

        /// <summary>
        /// 只会重连一次，并取消上次未触发的重连， Error时需要再次调用
        /// </summary>
        void OpenRetryConnection()
        {
            // 开始自动连接
            UniTask.Void(async (token) =>
            {
                do
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(5), false, PlayerLoopTiming.Update, token);

                    switch(client.ReadyState)
                    {
                        case WebSocketState.Closed:
                            _Connect(m_Url);
                            break;
                    }
                }
                while (!token.IsCancellationRequested);
            }, this.GetCancellationTokenOnDestroy());
        }
        
        /// <summary>
        /// 开始发送健康心跳信息
        /// </summary>
        void OpenSendHeartInfo()
        {
            // 最少5秒一次
            m_heartTime = Mathf.Max(m_heartTime, 5);

            UniTask.Void(async (token) =>
            {
                do
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(m_heartTime), false, PlayerLoopTiming.Update, token);

                    if (client != null && client.ReadyState == WebSocketState.Open)
                    {
                        if(!client.Ping(m_HeartText))
                        {
                            Debug.LogWarning($"发送心跳包失败...!{m_Url}");
                        }
                    }                    
                } while (!token.IsCancellationRequested);
            }, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="text"></param>
        public void SendText(string text)
        {
            if (client != null && client.IsAlive)
            {
                client.Send(text);
            }
        }

        protected override void OnDrop()
        {
            _CloseConnect();
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        protected virtual void OnSuccess() {}

        /// <summary>
        /// 收到消息通知
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnMessage(string msg) {}

        /// <summary>
        /// 断开连接，已经处理资源，不需要再次释放
        /// </summary>
        protected virtual void OnClose() {}
    }
}