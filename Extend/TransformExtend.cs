using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public static class TransformExtend
{
    #region 变换
    public static void IteratorMat(this Renderer render, System.Action<Material> action)
    {
        foreach (var m in render.materials)
        {
            action(m);
        }
    }

    public static void IteratoChild(this Transform t, System.Action<Transform, int> action, bool inclusionSelf = false)
    {
        if (inclusionSelf) action(t, 0);

        var child = t.childCount;
        for (var i = 0; i < child; i++)
        {
            action(t.GetChild(i), i);
        }
    }

    public static void IteratoAllChild(this Transform t, System.Action<Transform, int> action, bool inclusionSelf = true)
    {
        if (inclusionSelf) action(t, 0);

        var child = t.childCount;
        for (var i = 0; i < child; i++)
        {
            var st = t.GetChild(i);
            if (st.childCount > 0) IteratoAllChild(st, action, inclusionSelf);
            action(st, i);
        }
    }

    /// <summary>
    /// 删除所有子节点
    /// </summary>
    /// <param name="t"></param>
    public static void RemoveAllChild(this Transform t)
    {
        t.IteratoChild((tr, _) =>
        {
            GameObject.Destroy(tr.gameObject);
        });
    }

    public static void ReveseIteratoChild(this Transform t, System.Action<Transform, int> action)
    {
        var child = t.childCount;
        for (var i = child - 1; i >= 0; i--)
        {
            action(t.GetChild(i), i);
        }
    }

    public static void IteratoChild(this RectTransform t, System.Action<Transform, int> action)
    {
        var child = t.childCount;
        for (var i = 0; i < child; i++)
        {
            action(t.GetChild(i), i);
        }
    }

    public static void ReveseIteratoChild(this RectTransform t, System.Action<Transform, int> action)
    {
        var child = t.childCount;
        for (var i = child - 1; i >= 0; i--)
        {
            action(t.GetChild(i), i);
        }
    }

    /// <summary>
    /// 设置一个变换下的所有子物体的激活状态
    /// </summary>
    /// <param name="t"></param>
    /// <param name="enable">激活状态</param>
    public static void SetAllChildEnable(this Transform t, bool enable)
    {
        t.IteratoChild((node, _) =>
        {
            node.gameObject.SetActive(enable);
        });
    }

    /// <summary>
    /// 判断父变换是否是指定变换
    /// </summary>
    /// <param name="t"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool ParentIs(this Transform t, Transform target)
    {
        if (t.parent == null) return false;

        if (t.parent == target)
        {
            return true;
        }
        else
        {
            return t.parent.ParentIs(target);
        }
    }
    #endregion
}
