//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UMOD;
//using UnityEngine.Networking;
//using Cysharp.Threading.Tasks;

//namespace IOTLib
//{
//    // HTTP系统。对上级的特性包装。
//    // 回调结果后会释放UWR资源
//    [SystemDescribe(Author = "吴",
//        Describe = "对框架内置HttpSystem的进一步包装，提供简易的获取素材等方法",
//        Name = "EasyHttpSystem",
//        Version = "0.2",
//        Dependent = "HttpSystem")
//    ]
//    [GameSystem]
//    [UpdateInAfter(typeof(HttpSystem))]
//    [UpdateInGroup(typeof(InitializationSystemGroup))]
//    public class EasyHttpSystem : BaseSystem
//    {
//        //private static HttpSystem httpSystem = null;

//        public override void OnCreate()
//        {
//            //httpSystem = SystemManager.GetSystem<HttpSystem>();
//        }

//        /// <summary>
//        /// 获取一个远程素材，如果失败。返回一张空白像素图（类似404），不会奔溃。
//        /// </summary>
//        /// <param name="url">图像地址</param>
//        /// <param name="cancelToken">生命周期(可为空，但是通常是一个BUG的开端)</param>
//        /*public static async UniTask<Texture> GetTexture(string url, System.Threading.CancellationToken? cancelToken = null)
//        {
//            using (var uwr = UnityWebRequestTexture.GetTexture(url))
//            {
//                if (cancelToken.HasValue)
//                {
//                    await uwr.SendWebRequest().WithCancellation(cancelToken.Value);
//                }
//                else
//                {
//                    await uwr.SendWebRequest();
//                }

//                if (uwr.result != UnityWebRequest.Result.Success)
//                {
//                    Debug.LogErrorFormat("获取远程素材失败:{0},原因:{1}", uwr.url, uwr.error);
//                    return Texture2D.whiteTexture;
//                }
//                else
//                {
//                    var texture = DownloadHandlerTexture.GetContent(uwr);
//                    return texture;
//                }
//            }
//        }*/
//    }
//}