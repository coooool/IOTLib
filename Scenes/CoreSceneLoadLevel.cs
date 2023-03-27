using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace IOTLib.Scenes
{
    [MonoHeaderHelp("1、作为第一个驱动运行的脚本，挂载在Core场景中，加载你的业务场景")]
    [DefaultExecutionOrder(-100)]
    public class CoreSceneLoadLevel : MonoBehaviour, ISceneLoadProgressEvent
    {
        [Header("初始化完成加载的您的业务场景")]
        public string m_StartLoadLevelName;

        [Header("以下3个方法用来侦听加载你场景进度")]
        public UnityEvent m_OnBeginLoad;
        public UnityEvent m_OnEndClose;
        public UnityEvent<float> m_OnProgress;

        public void BeginLoad()
        {
            m_OnBeginLoad?.Invoke();
        }

        public void LoadClose()
        {
            m_OnEndClose?.Invoke();
        }

        public void Report(float value)
        {
            m_OnProgress?.Invoke(value);
        }

        async void Start()
        {
            if (!string.IsNullOrEmpty(m_StartLoadLevelName))
            {
                await SceneLoader.LoadAddtiveSceneAsync(m_StartLoadLevelName);
            }
            else
            {
                Debug.Log("没有设置初始加载的场景");
            }
        }
    }
}
