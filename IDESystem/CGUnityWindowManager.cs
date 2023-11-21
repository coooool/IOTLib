using Cysharp.Threading.Tasks;
using IOTLib;
using IOTLib.IDESystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IOTLib
{
    public class CGUnityWindowManager : MonoBehaviour
    {
        static CGUnityWindowManager Instance;

        internal static EditorSetting GSetting;

        public static bool IsShow { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitEditorWindow()
        {
            var newGameObject = new GameObject("__EditorWindow__", typeof(CGUnityWindowManager));
            newGameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public static void Close()
        {
            if (Instance.TryGetComponent<CGPrefabEditorWindow>(out var cgpew))
            {
                IsShow = false;

                foreach (var a in TagSystem.Find<DragGameObject>(true, CGResources.TAGName))
                {
                    a.SendMessage(nameof(IDEStateChangedEvent.OnSceneEditorStateNotify), false, SendMessageOptions.DontRequireReceiver);
                    Destroy(a);
                }

                foreach (var a in TagSystem.Find<DragGameObject2D>(true, CGResources.TAGName))
                {
                    a.SendMessage(nameof(IDEStateChangedEvent.OnSceneEditorStateNotify), false, SendMessageOptions.DontRequireReceiver);
                    Destroy(a);
                }

                Destroy(Instance.GetComponent<CGPrefabWindow>());
                Destroy(Instance.GetComponent<CGSceneToolsWindow>());
                Destroy(Instance.GetComponent<CGHandleDragMouse>());
                Destroy(cgpew);

                // »Ö¸´Ä¬ÈÏ×´Ì¬
                CameraHelpFunc.ToAState();
            }
        }

        public static void Show(params string[] tagTypes)
        {
            if(tagTypes.Length == 0) tagTypes = new[] {CGResources.TAGName};
            
            if (Instance.TryGetComponent<CGPrefabEditorWindow>(out _) == false)
            {
                IsShow = true;

                Instance.gameObject.AddComponent<CGSceneToolsWindow>();
                Instance.gameObject.AddComponent<CGPrefabWindow>();
                Instance.gameObject.AddComponent<CGPrefabEditorWindow>();
                Instance.gameObject.AddComponent<CGHandleDragMouse>();

                foreach (var a in TagSystem.Find<ExportCGPrefab>(true, tagTypes))
                {
                    if(a.IsUGUI)
                        a.gameObject.AddComponent<DragGameObject2D>();
                    else
                        a.gameObject.AddComponent<DragGameObject>();

                    a.SendMessage(nameof(IDEStateChangedEvent.OnSceneEditorStateNotify), true, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public static void Show(EditorSetting setting, params string[] tagTypes)
        {
            GSetting = setting;
            Show(tagTypes);
        }

        public static void Toggle()
        {
            if (Instance.TryGetComponent<CGSceneToolsWindow>(out _))
            {      
                Close();      
            }
            else
            {
                Show();
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F5))
            {
                Toggle();
            }
        }
    }
}