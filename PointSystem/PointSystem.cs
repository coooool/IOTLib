using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UMOD;
using UnityEngine;
using DG.Tweening;

using UCamera = UnityEngine.Camera;
using System.Data;

namespace IOTLib.PointSystem
{
    [GameSystem]
    [SystemDescribe(Author = "吴",
      Dependent = "无",
      Describe = "视点辅助系统，能解决摄像头移动等常见问题",
      Name = "PointSystem",
      Version = "0.1")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateInAfter(typeof(GameHandleSystem))]
    public class PointSystem : SingleBaseSystem<PointSystem>
    {
        /// <summary>
        /// 从DBServer或绑定在本系统的物体中获取一个点位置
        /// </summary>
        /// <param name="pointName">点名</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector3 GetPos(string pointName)
        {
            if (DBServer.TryGetPointData(pointName, out var point))
            {
                var dataTmp = new { pos = string.Empty };

                var data = point!.ToAnonymousType(dataTmp);

                var pos = data.pos.ToVector3();

                return pos;
            }

            foreach (var a in Install)
            {
                if (a.gameObject.name == pointName)
                {
                    return a.transform.position;
                }
            }

            throw new InvalidOperationException("找不到目标点：" + pointName);
        }

        /// <summary>
        /// 获取一个欧拉角
        /// </summary>
        /// <param name="pointName">点名</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector3 GetEulerAngle(string pointName)
        {
            if (DBServer.TryGetPointData(pointName, out var point))
            {
                var dataTmp = new { euler = string.Empty };

                var data = point!.ToAnonymousType(dataTmp);

                var pos = data.euler.ToVector3();

                return pos;
            }

            foreach (var a in Install)
            {
                if (a.gameObject.name == pointName)
                {
                    return a.transform.eulerAngles;
                }
            }

            throw new InvalidOperationException("找不到目标点：" + pointName);
        }

        /// <summary>
        /// 获取点位置和欧拉角
        /// </summary>
        /// <param name="pointName">点名</param>
        /// <returns>vector[]:0是位置，1是角度</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector3[] GetPosAndEulerAngle(string pointName)
        {
            if (DBServer.TryGetPointData(pointName, out var point))
            {
                var dataTmp = new { pos = string.Empty, euler = string.Empty };

                var data = point!.ToAnonymousType(dataTmp);

                var pos = data.euler.ToVector3();
                var euler = data.euler.ToVector3();

                return new Vector3[] {pos, euler};
            }

            foreach (var a in Install)
            {
                if (a.gameObject.name == pointName)
                {
                    var pos = a.transform.position;
                    var euler = a.transform.eulerAngles;

                    return new Vector3[] { pos, euler };
                }
            }

            throw new InvalidOperationException("找不到目标点：" + pointName);
        }

        /// <summary>
        /// 获取一个由点构成的路径
        /// </summary>
        /// <param name="pathName">路径名称</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector3[] GetPointPath(string pathName)
        {
            foreach (var a in Install)
            {
                if (a.gameObject.name == pathName)
                {
                    if(a is IPointPathProvider path)
                    {
                        return path.points;
                    }
                }
            }

            throw new InvalidOperationException($"找不到目标路径：{pathName}");
        }

        //public static bool MoveCamera(string pointName)
        //{
        //    if(DBServer.TryGetPointData(pointName, out var point))
        //    {
        //        var dataTmp = new { pos = string.Empty, format = 1, point_type = 1, duration = 0.65f };

        //        var data = point!.ToAnonymousType(dataTmp);

        //        var pos = data.pos.ToVector3();

        //        var seq = DOTween.Sequence();

        //        seq.Join(UCamera.main.transform.DOMove(pos, dataTmp.duration).SetEase(Ease.InOutQuad));
        //        seq.Join(UCamera.main.transform.DODynamicLookAt(pos, dataTmp.duration).SetEase(Ease.InOutQuad));


        //        return true;
        //    }

        //    Debug.LogErrorFormat("找不到目标点：{0}", pointName);
        //    return false;
        //}
    }
}
