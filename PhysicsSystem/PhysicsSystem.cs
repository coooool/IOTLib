using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "吴",
       Dependent = "无",
       Describe = "对场景中的设备等物体进行物理模拟管理",
       Name = "物理模拟系统",
       Version = "0.1")]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateInAfter(typeof(W2SPosSystem))]
    public class PhysicsSystem : BaseSystem
    {
        RaycastHit[] m_RayHits = new RaycastHit[6];

        public override void OnCreate()
        {
          
        }

        public override void OnStartRun()
        {
            UniTask.Void(LastUpdate, GetDropCancellationToken());
        }

        void RayTest(Vector3 origin, PositionToGroundData a)
        {
            var minDistance = float.MaxValue;
            Vector3 selectPoint = Vector3.zero;
            bool havePoint = false;

            var hitCount = Physics.RaycastNonAlloc(origin + a.transform.TransformDirection(Vector3.up), Vector3.down, m_RayHits, Mathf.Infinity, a.layer);
            if (hitCount > 0)
            {
                for (var i = 0; i < hitCount; i++)
                {
                    if (m_RayHits[i].collider.gameObject == a.gameObject)
                    {
                        continue;
                    }
                    else if (m_RayHits[i].transform.ParentIs(a.transform))
                    {
                        continue;
                    }

                    if (m_RayHits[i].distance < minDistance)
                    {
                        havePoint = true;
                        minDistance = m_RayHits[i].distance;
                        selectPoint = m_RayHits[i].point;
                    }
                }
            }
  
            if(!havePoint)
            {
                hitCount = Physics.RaycastNonAlloc(origin + (a.transform.TransformDirection(Vector3.up) * Camera.main.farClipPlane),
                    Vector3.down, m_RayHits, Mathf.Infinity, a.layer);
                if (hitCount > 0)
                {
                    for (var i = 0; i < hitCount; i++)
                    {
                        if (m_RayHits[i].collider.gameObject == a.gameObject)
                        {
                            continue;
                        }
                        else if (m_RayHits[i].transform.ParentIs(a.transform))
                        {
                            continue;
                        }

                        if (m_RayHits[i].distance < minDistance)
                        {
                            havePoint = true;
                            minDistance = m_RayHits[i].distance;
                            selectPoint = m_RayHits[i].point;
                        }
                    }
                }
            }

            if (havePoint)
            {
                origin.y = selectPoint.y;
                //origin.y += a.m_Offset;
                a.transform.position = origin;
            }
        }

        async UniTaskVoid LastUpdate(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, token);

                foreach (var a in GetBindDatas<PositionToGroundData>().Where(p => p.enabled))
                {
                    RayTest(a.transform.position, a);
                }

                token.ThrowIfCancellationRequested();
            }
        }
    }
}
