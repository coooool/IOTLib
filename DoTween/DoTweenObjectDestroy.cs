using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public class DoTweenObjectDestroy : MonoBehaviour
    {
        private List<Tween> m_tween;

        public DoTweenObjectDestroy()
        {
            m_tween = new List<Tween>();
        }

        public static void ProcessLoopTween(Tween tween, Component mono)
        {
            if (tween.hasLoops && tween.Loops() == -1)
            {
                DoTweenObjectDestroy target;

                if (!mono.TryGetComponent<DoTweenObjectDestroy>(out target))
                {
                    target = mono.gameObject.AddComponent<DoTweenObjectDestroy>();
                }

                if(target == null)
                {
                    Debug.LogError("目标已经被释放");
                    return;
                }
                target.RegisterTweenData(tween);
            }
        }

        private void IteraAllNode(System.Action<Tween> callBack, bool clear)
        {
            if (m_tween != null)
            {
                foreach (var a in m_tween)
                {
                    callBack?.Invoke(a);
                }

                if (clear)
                    m_tween.Clear();
            }
        }

        private void OnDestroy()
        {
            IteraAllNode(a =>
            {
                a.Kill();
            }, true);
        }

        private void OnDisable()
        {
            IteraAllNode(a =>
            {
                a.Pause();
            }, false);
        }

        private void OnEnable()
        {
            IteraAllNode(a =>
            {
                a.Play();
            }, false);
        }

        public void RegisterTweenData(Tween ani)
        {
            m_tween.Add(ani);
        }
    }
}