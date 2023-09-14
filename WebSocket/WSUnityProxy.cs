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
        /// ����URL
        /// </summary>
        public string m_Url;
        /// <summary>
        /// �Ƿ����������Ϣ
        /// </summary>
        public bool m_Debug;
        /// <summary>
        /// ����������ʱ��
        /// </summary>
        public int m_heartTime = 5;

        private WebSocket client = null;

        public string m_HeartText = "{\"heart\":true}";

        void Start()
        {
            // �����Զ�������
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
                        Debug.LogWarning("ʵ���Ѿ���������...");
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
                    Debug.Log("===�յ�WebSocket������===");

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
                Debug.LogError($"WebSocket��������:{e.Message}");
            });
        }

        void _CloseConnetion(object sender, CloseEventArgs arg)
        {
            // �Ͽ�����
            _CloseConnect();

            OnClose();
        }

        /// <summary>
        /// ֻ������һ�Σ���ȡ���ϴ�δ������������ Errorʱ��Ҫ�ٴε���
        /// </summary>
        void OpenRetryConnection()
        {
            // ��ʼ�Զ�����
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
        /// ��ʼ���ͽ���������Ϣ
        /// </summary>
        void OpenSendHeartInfo()
        {
            // ����5��һ��
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
                            Debug.LogWarning($"����������ʧ��...!{m_Url}");
                        }
                    }                    
                } while (!token.IsCancellationRequested);
            }, this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        /// ��������
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
        /// ���ӳɹ�
        /// </summary>
        protected virtual void OnSuccess() {}

        /// <summary>
        /// �յ���Ϣ֪ͨ
        /// </summary>
        /// <param name="msg"></param>
        protected virtual void OnMessage(string msg) {}

        /// <summary>
        /// �Ͽ����ӣ��Ѿ�������Դ������Ҫ�ٴ��ͷ�
        /// </summary>
        protected virtual void OnClose() {}
    }
}