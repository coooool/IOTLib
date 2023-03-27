using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
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

        internal void Connect(string url)
        {
            if (client != null)
            {
                client.Close();
                client = null;
            }

            try
            {
                m_Url = url;

                client = new WebSocket(url);
                client.OnOpen += OnConnectSuccess;
                client.OnMessage += OnRecvData;
                client.OnClose += OnCloseConnetion;
                client.OnError += OnRecvError;
                client.Connect();
            }
            catch(Exception e)
            {
                Debug.LogErrorFormat("WebSocket连接时发生错误:{0}", e.Message);
            }
        }

        internal void CloseConnect()
        {
            this.OnDrop();
        }

        void OnConnectSuccess(object sender, EventArgs e)
        {
            // 连接成功后关闭自动重连
            CancelInvoke("AutoRetryConnect");

            if (m_heartTime > 0 && !string.IsNullOrEmpty(m_HeartText))
            {
                // 发送心跳包
                InvokeRepeating("OnSendHeart", m_heartTime, m_heartTime);
            }

            OnSuccess();
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        protected virtual void OnSuccess()
        {

        }

        void AutoRetryConnect()
        {
            if (client == null)
            {
                Connect(m_Url);
            }
        }

        protected virtual void OnSendHeart()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                if (client != null && client.IsAlive)
                {
                    client.Send(m_HeartText);
                }
            });
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
            if (client != null)
            {
                client.Close();
            }
        }

        void OnRecvData(object sender, MessageEventArgs arg)
        {
            UniTask.Void(async() =>
            {
                await UniTask.SwitchToMainThread();
                OnMessage(arg.Data);
            });
        }

        protected virtual void OnMessage(string msg)
        {
            if (m_Debug)
            {
                Debug.Log(msg);
            }
        }

        void OnRecvError(object sender, ErrorEventArgs e)
        {
            Debug.LogError($"WS发生错误:{e.Message}");
            this.CloseConnect();
        }

        void OnCloseConnetion(object sender, CloseEventArgs arg)
        {
            client = null;

            // 关闭心跳包
            CancelInvoke("OnSendHeart");
            // 每5秒开自动重试
            InvokeRepeating("AutoRetryConnect", 0.1f, 5);

            OnClose();
        }

        /// <summary>
        /// 断开连接，已经处理资源，不需要再次释放
        /// </summary>
        protected virtual void OnClose()
        {

        }
    }
}