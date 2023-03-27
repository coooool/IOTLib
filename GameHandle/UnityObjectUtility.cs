using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace IOTLib
{
    public static class UnityObjectUtility
    {
        public static bool IsDestroyed(this UnityObject target)
        {
            // Checks whether a Unity object is not actually a null reference,
            // but a rather destroyed native instance.

            return !ReferenceEquals(target, null) && target == null;
        }

        public static bool IsUnityNull(this object obj)
        {
            // Checks whether an object is null or Unity pseudo-null
            // without having to cast to UnityEngine.Object manually

            return obj == null || ((obj is UnityObject) && ((UnityObject)obj) == null);
        }

        public static IEnumerable<T> NotUnityNull<T>(this IEnumerable<T> enumerable) where T : UnityObject
        {
            return enumerable.Where(i => i != null);
        }

        public static IEnumerable<T> FindObjectsOfTypeIncludingInactive<T>()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (scene.isLoaded)
                {
                    foreach (var rootGameObject in scene.GetRootGameObjects())
                    {
                        foreach (var result in rootGameObject.GetComponentsInChildren<T>(true))
                        {
                            yield return result;
                        }
                    }
                }
            }
        }
    }
}
