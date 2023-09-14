using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace IOTLib.Scenes
{
    public class SceneLoader
    {
        private static bool _loaded = false;

        // 设置激活的场景名。即将切换场景
        public static UnityEvent<string,string> PreSetActiveScene = new ();

        /// <summary>
        /// 加载一个附加场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="progress">进度侦听(不监听可以为null)</param>
        /// <returns></returns>
        public static void LoadAddtiveScene(string sceneName, ISceneLoadProgressEvent progress = null)
        {
            UniTask.Void(async () => await LoadAddtiveSceneAsync(sceneName, progress));
        }

        public static async UniTask LoadAddtiveSceneAsync(string sceneName, ISceneLoadProgressEvent progress = null)
        {
            if (_loaded)
            {
                Debug.LogError($"系统已经处于加载场景中,当前正在重试继续加载，这通常是异常操作被阻止了。");
                throw new OperationCanceledException($"系统繁忙时继续加载了{sceneName}");
            }

            // Core 名称
            var CoreSceneName = DBServer.GetVar("SCENELOADER_CORE_NAME", "Core");
            var OldSceneName = SceneManager.GetActiveScene().name;

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var node = SceneManager.GetSceneAt(i);

                if (node.isLoaded)
                {
                    if (node.name == CoreSceneName)
                    {
                        continue;
                    }
                    else
                    {
                        await SceneManager.UnloadSceneAsync(node);
                    }
                }
            }

            progress?.BeginLoad();

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(progress);

            PreSetActiveScene?.Invoke(OldSceneName, sceneName);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            DynamicGI.UpdateEnvironment();

            progress?.LoadClose();
        }
    }
}
