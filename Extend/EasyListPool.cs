using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Pool;

namespace IOTLib
{
    public class EasyListPool<T> : IDisposable, IEnumerable<T>
    {
        private readonly List<T> m_list;

        public static EasyListPool<T> Get()
        {
            return new EasyListPool<T>();
        }

        public EasyListPool()
        {
            m_list= ListPool<T>.Get();
        }

        public void Add(T item)
        {
            m_list.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            m_list.AddRange(items);
        }

        public void Remove(T item)
        {
            m_list.Remove(item);
        }

        public bool Contains(T item) 
        { 
            return m_list.Contains(item);
        }

        public void Clear()
        {
            m_list.Clear();
        }

        public int Count()
        {
            return m_list.Count;
        }

        public void Dispose()
        {
           ListPool<T>.Release(m_list);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}
