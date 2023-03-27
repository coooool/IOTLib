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
    [SystemDescribe(Author = "吴", Dependent = "无", Describe = "一个线性宏观层次的状态作业系统，用于业务逻辑开发。", Name = "GameHandle", Version = "0.5")]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [FeatureSystem(255, 100, 0)]
    public class GameHandleSystem : BaseSystem
    {
        /// <summary>
        /// 任务数据
        /// </summary>
        private struct TaskNode
        {
            public IFlowStateGraph FlowGraph { get; set; }
            public CancellationToken? CancellationToken { get; set; }
            public GameObject LifecycleRef { get; set; }
        }

        /// <summary>
        /// 任务列表
        /// </summary>
        private Queue<TaskNode> _taskList = new Queue<TaskNode>(8);

        /// <summary>
        /// 引用UMOD的模块挂载点。切入更新时机使用。
        /// </summary>
        private MODHandle Ref_GlobalModuleSys;

        public override void OnCreate()
        {
            Ref_GlobalModuleSys = MODHandle.UnityInstall.GetComponent<MODHandle>();
            if(Ref_GlobalModuleSys == null) 
                throw new InvalidOperationException("找不到MODHandle组件");

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
            // 为节约分配每次处理一个流时调用Reset清空栈数据
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

                        // 取消该任务？
                        if (currentEnterGraph.CancellationToken.HasValue && currentEnterGraph.CancellationToken.Value.IsCancellationRequested)
                        {
                            Debug.LogWarning($"任务被用户取消:{currentEnterGraph.FlowGraph.GUID}");
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
                    Debug.LogError("发生错误");
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
        /// 切入Unity主线程并在Update位置开始更新
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
        /// 添加一个作业任务。当前帧添加的任务在下一帧被处理,
        /// </summary>
        /// <param name="flowGraph"></param>
        /// <param name="LifecycleRef">生命周期，如果为NULL则自动管理</param>
        public static void PushGraph(IFlowStateGraph flowGraph, GameObject LifecycleRef = null)
        {
            var ghs = SystemManager.GetSystem<GameHandleSystem>();
            var node = new TaskNode { FlowGraph = flowGraph, LifecycleRef = LifecycleRef, CancellationToken = null };
            ghs._taskList.Enqueue(node);
        }

        /// <summary>
        /// 退出一个作业任务。当前帧添加的任务在下一帧被处理,
        /// </summary>
        /// <param name="id">ID可是名称也可以GUID</param>
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

            Debug.LogError($"找不到目标图状态:{name}");

            return null;
        }
    }
}