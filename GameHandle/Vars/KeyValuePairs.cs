using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public struct KeyValuePairs
{
    Stack<KeyValuePair<string,object>> kvps;

    public KeyValuePairs(int initialCapacity)
    {
        kvps = new Stack<KeyValuePair<string, object>>(initialCapacity);
    }

    public KeyValuePairs(Dictionary<string,string> pairs) : this(pairs.Count)
    {
        if(pairs.Count > 0)
        {
            foreach(var v in pairs)
            {
                Push(new KeyValuePair<string, object>(v.Key, v.Value));
            }
        }
    }

    /// <summary>
    /// 从一个匿名对象创建键值对
    /// </summary>
    /// <param name="args"></param>
    public KeyValuePairs(System.Object args)
    {
        var ty = args.GetType();

        var propers = ty.GetProperties();

        if (propers.Length > 0)
        {
            kvps = new Stack<KeyValuePair<string, object>>(propers.Length);

            foreach (var a in propers)
            {
                Push(new KeyValuePair<string, object>( a.Name, a.GetValue(args,null)));
            }
        }
        else
        {
            kvps = null;
        }
    }

    public void Push( KeyValuePair<string, object> v)
    {
        if (kvps == null) kvps = new Stack<KeyValuePair<string, object>>(3);

        kvps.Push(v);
    }


    public KeyValuePair<string, object> Peek()
    {
        Assert.IsNotNull(kvps, "未初始化");

        return kvps.Peek();
    }

    public KeyValuePair<string, object> Pop()
    {
        Assert.IsNotNull(kvps, "未初始化");

        return kvps.Pop();
    }

    public bool TryGetValue(string key, out object value)
    {
        value = null;

        foreach(var a in kvps)
        {
            if (a.Key == key)
            {
                value = a.Value;
                return true;
            }
        }

        return false;
    }

    public object GetValue(string key)
    {
        foreach (var a in kvps)
        {
            if (a.Key == key) return a.Value;
        }

        throw new System.InvalidOperationException($"获取变量不存在:{key}");
    }

    public bool GetIndex(int index,out KeyValuePair<string, object> value)
    {
        var tmp = 0;
        value = default(KeyValuePair<string, object>);

        foreach (var a in kvps)
        {
            if (tmp == index)
            {
                value = a;
                return true;
            }

            tmp++;
        }

        return false;
    }

    public void Clear()
    {
        if(kvps != null) kvps.Clear();
    }

    public int Count => kvps == null ? 0 : kvps.Count;
}
