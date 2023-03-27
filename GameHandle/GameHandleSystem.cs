using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMOD;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Assertions;
using System;

namespace IOTLib
{
    [GameSystem(AlwaysRun = true)]
    [SystemDescribe(Author = "��", Dependent = "��", Describe = "һ�����Ժ�۲�ε�״̬��ҵϵͳ������ҵ���߼�������", Name = "GameHandle", Version = "0.5")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [FeatureSystem(255, 100, 0)]
    public class GameHandleSystem : BaseSystem
    {
        /// <summary>
        /// ��������
        /// </summary>
        private struct TaskNode
        {
            public IFlowStateGraph FlowGraph { get; set; }
            public CancellationToken? CancellationToken { get; set; }
            public GameObject LifecycleRef { get; set; }
        }

        /// <summary>
        /// �����б�
        /// </summary>
        private Queue<TaskNode> _taskList = new Queue<TaskNode>(8);

        /// <summary>
        /// ����UMOD��ģ����ص㡣�������ʱ��ʹ�á�
        /// </summary>
        private MODHandle Ref_GlobalModuleSys;

        public override void OnCreate()
        {
            Ref_GlobalModuleSys = MODHandle.UnityInstall.GetComponent<MODHandle>();
            if(Ref_GlobalModuleSys == null) 
                throw new InvalidOperationException("�Ҳ���MODHandle���");

            UniTask.Void(UpdateTaskLoop, GetDropCancellationToken());

            PushGraph(new CameraHandle());
        }

        public override void OnDrop()
        {
            _taskList.Clear();
        }

        private async UniTaskVoid UpdateTaskLoop(CancellationToken cancellationToken)
        {
            await UniTask.WaitForEndOfFrame(Ref_GlobalModuleSys);
            // Ϊ��Լ����ÿ�δ���һ����ʱ����Reset���ջ����
            var flow = new Flow();

            while (cancellationToken.IsCancellationRequested == false)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    while (_taskList.Count > 0)
                    {
                        await UniTask.Yield(PlayerLoopTiming.PreUpdate, cancellationToken);

                        var currentEnterGraph = _taskList.Dequeue();

                        // ȡ��������
                        if (currentEnterGraph.CancellationToken.HasValue && currentEnterGraph.CancellationToken.Value.IsCancellationRequested)
                        {
                            Debug.LogWarning($"�����û�ȡ��:{currentEnterGraph.FlowGraph.GUID}");
                            continue;
                        }

                        if (currentEnterGraph.LifecycleRef == null)
                        {
                            var newLifecycleRef = new GameObject(currentEnterGraph.FlowGraph.GUID);
                            newLifecycleRef.hideFlags = HideFlags.HideInHierarchy;
                            newLifecycleRef.AddComponent<FlowStateGraphLifecycle>().Init(currentEnterGraph.FlowGraph);
                        }
                        else
                        {
                            currentEnterGraph.LifecycleRef.AddComponent<FlowStateGraphLifecycle>().Init(currentEnterGraph.FlowGraph);
                        }

                        await currentEnterGraph.FlowGraph.StartListener(flow);
                    }

                    await UpdateUnityPlayerLoop(flow, cancellationToken);
                }
                catch (System.Exception ex) when (!(ex is System.OperationCanceledException))
                {
                    _taskList.Clear();
                    Debug.LogError("��������");
                    Debug.LogException(ex);
                }
                finally
                {
                    flow.Reset();
                }

                await UniTask.WaitForEndOfFrame(Ref_GlobalModuleSys);
            }
        }

        /// <summary>
        /// ����Unity���̲߳���Updateλ�ÿ�ʼ����
        /// </summary>
        /// <returns></returns>
        private async UniTask UpdateUnityPlayerLoop(Flow flow, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            foreach (var bindData in this)
            {
                if (bindData is FlowStateGraphLifecycle ml)
                {
                    if (ml.isActiveAndEnabled == false) continue;

                    if (ml.TryGetTarget(out var target))
                    {
                        await target.OnLoopUpdateState(flow);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            await UniTask.Yield(PlayerLoopTiming.LastPreLateUpdate, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var bindData in this)
            {
                if (bindData is FlowStateGraphLifecycle ml)
                {
                    if (ml.isActiveAndEnabled == false) continue;

                    if (ml.TryGetTarget(out var target))
                    {
                        await target.OnLoopUpdateTransition(flow);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// ���һ����ҵ���񡣵�ǰ֡��ӵ���������һ֡������,
        /// </summary>
        /// <param name="flowGraph"></param>
        /// <param name="LifecycleRef">�������ڣ����ΪNULL���Զ�����</param>
        public static void PushGraph(IFlowStateGraph flowGraph, GameObject LifecycleRef = null)
        {
            var ghs = SystemManager.GetSystem<GameHandleSystem>();
            var node = new TaskNode { FlowGraph = flowGraph, LifecycleRef = LifecycleRef, CancellationToken = null };
            ghs._taskList.Enqueue(node);
        }

        /// <summary>
        /// �˳�һ����ҵ���񡣵�ǰ֡��ӵ���������һ֡������,
        /// </summary>
        /// <param name="id">ID��������Ҳ����GUID</param>
        public static void PopGraph(string id)
        {
            var graphComponent = GetFlowGraphFromName(id);
            if (graphComponent != null)
            {
                GameObject.DestroyImmediate(graphComponent.gameObject, true);
            }
        }

        public static FlowStateGraphLifecycle? GetFlowGraphFromName(string name)
        {
            foreach (var bindData in SystemManager.GetSystem<GameHandleSystem>())
            {
                if (bindData is FlowStateGraphLifecycle ml)
                {
                    if (ml.TryGetTarget(out var target))
                    {
                        if (target.Name == name || target.GUID == name)
                        {
                            return ml;
                        }
                    }
                }
            }

            return null;
        }

        public GameObject? GetGraphComponent(string name)
        {
            var com = GetFlowGraphFromName(name);

            if (com != null) return com.gameObject;

            Debug.LogError($"�Ҳ���Ŀ��ͼ״̬:{name}");

            return null;
        }
    }
}