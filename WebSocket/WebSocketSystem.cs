using System.Collections;
using System.Collections.Generic;
using UMOD;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "��", Dependent = "��", Describe = "WebSocketʵ������", Name = "WebSocket", Version = "0.1")]
    public class WebSocketSystem : BaseSystem
    {
        /// <summary>
        /// ��ʼ����һ��ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        public static void Connection<T>(string url) where T: WSUnityProxy
        {
            Assert.IsFalse(string.IsNullOrEmpty(url), "���ӵ�Ŀ�겻��Ϊ��");

            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"Ŀ����������ڳ�����,����غ�����:{typeof(T).Name}");
            }
            else
            {
                target.Connect(url);
            }
        }

        /// <summary>
        /// �ر�һ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Close<T>() where T:WSUnityProxy
        {
            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"Ŀ����������ڳ�����,����غ�����:{typeof(T).Name}");
            }
            else
            {
                target.CloseConnect();
            }
        }

        public static void SendText<T>(string text) where T : WSUnityProxy
        {
            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"Ŀ����������ڳ�����,����غ�����:{typeof(T).Name}");
            }
            else
            {
                target.SendText(text);
            }
        }

        /// <summary>
        /// ���Ի�ȡһ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">�ɹ�����True</param>
        /// <returns>�ɹ�����True</returns>
        public static bool TryGetConnection<T>(out T result) where T:WSUnityProxy
        {
            result = null;

            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if(target == null)
            {
                return false;
            }
            else
            {
                result = target;
                return true;
            }
        }
    }
}