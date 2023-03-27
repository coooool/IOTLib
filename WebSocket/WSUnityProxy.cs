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
                Debug.LogErrorFormat("WebSocket����ʱ��������:{0}", e.Message);
            }
        }

        internal void CloseConnect()
        {
            this.OnDrop();
        }

        void OnConnectSuccess(object sender, EventArgs e)
        {
            // ���ӳɹ���ر��Զ�����
            CancelInvoke("AutoRetryConnect");

            if (m_heartTime > 0 && !string.IsNullOrEmpty(m_HeartText))
            {
                // ����������
                InvokeRepeating("OnSendHeart", m_heartTime, m_heartTime);
            }

            OnSuccess();
        }

        /// <summary>
        /// ���ӳɹ�
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
            Debug.LogError($"WS��������:{e.Message}");
            this.CloseConnect();
        }

        void OnCloseConnetion(object sender, CloseEventArgs arg)
        {
            client = null;

            // �ر�������
            CancelInvoke("OnSendHeart");
            // ÿ5�뿪�Զ�����
            InvokeRepeating("AutoRetryConnect", 0.1f, 5);

            OnClose();
        }

        /// <summary>
        /// �Ͽ����ӣ��Ѿ�������Դ������Ҫ�ٴ��ͷ�
        /// </summary>
        protected virtual void OnClose()
        {

        }
    }
}