//using System;
//using System.Collections.Generic;
//using UMOD;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//namespace IOTLib
//{
//    [GameSystem]
//    [SystemDescribe(Author = "吴", Dependent = "无", Describe = "提供运行时热插拔框架模块", Name = "RuntimePluginInstall", Version = "0.0")]
//    public class RuntimeMonoEventSystem : BaseSystem
//    {
//        public override void OnCreate()
//        {
//            SceneManager.sceneLoaded += OnApplicationSceneLoad;
//        }

//        ~RuntimeMonoEventSystem()
//        {
//            SceneManager.sceneLoaded -= OnApplicationSceneLoad;
//        }

//        /// <summary>
//        /// 场景加载发生了变化，需要重新收集事件
//        /// </summary>
//        /// <param name="scene"></param>
//        /// <param name="loadSceneMode"></param>
//        void OnApplicationSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
//        {
//            //Debug.Log("场景发生改变,需要重新收集数据");
//            //scene.GetRootGameObjects
//        }
//    }
//}
