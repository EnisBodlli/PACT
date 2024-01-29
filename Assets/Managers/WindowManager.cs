using System;
using System.Collections;
using System.Collections.Generic;
using Pact.WindowManager.Enum;
using Pact.WindowManager.Windows;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using TimeWarpRandomUtils;
using Pact.WindowManager.ScriptableObjects.ScriptableEnums;

namespace Pact.WindowManager
{
    public class WindowManager : Singleton<WindowManager>
    {
        [SerializeField] private Transform windowsParentInGame;
        [SerializeField] private Transform windowsParentOverlay;

        [SerializeField] private List<GameObject> windows;


        private Dictionary<WindowType, GameObject> windowPrefabsDict = new Dictionary<WindowType, GameObject>();
        private Dictionary<WindowType, WindowBase> instantiatedWindowsDict = new Dictionary<WindowType, WindowBase>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var (window, windowType) in from window in windows
                                                 let windowType = window.GetComponent<WindowBase>().type.windowType
                                                 select (window, windowType))
            {
                if (windowPrefabsDict.ContainsKey(windowType))
                {
                    Debug.LogError($"there is a duplicate window {windowType}");
                }

                windowPrefabsDict[windowType] = window;
            }
        }
        public bool IsWindowOpen(WindowType windowType)
        {
            return GetOpenedWidows().Any(x => x.type.windowType == windowType);
        }
        public void CloseAllWindows()
        {
            foreach (var windowBase in GetOpenedWidows())
            {
                CloseWindow(windowBase.type);
            }
            //TimeMachineButtonToggle(WindowType.RunMenu,false);
        }
        public WindowBase CloseWindow(WindowTypeEnumSO windowTypeEnum)
        {
            var windowType = windowTypeEnum.windowType;
            return CloseWindow(windowType);
        }

        private IEnumerable<WindowBase> GetOpenedWidows()
        {
            return instantiatedWindowsDict.Values;
        }
        private WindowBase GetOrCreateWindow(WindowType windowType, Transform parent = null)
        {
            WindowBase windowBase;
            Assert.IsTrue(windowPrefabsDict.ContainsKey(windowType));

            if (instantiatedWindowsDict.TryGetValue(windowType, out var window))
            {
                windowBase = window;
                if (parent != null)
                {
                    windowBase.transform.SetParent(parent, false);
                }
            }
            else
            {
                if (parent == null)
                {
                    parent = windowsParentInGame;
                }
                var windowGO = Instantiate(windowPrefabsDict[windowType], parent);
                //windowBase = windowGO.GetComponent<ChildWindow>();
                //if (windowBase == null)
                //{
                windowBase = windowGO.GetComponent<CompositeWindow>();
                //}

                instantiatedWindowsDict[windowType] = windowBase;
                windowBase.Hide();
            }

            return windowBase;
        }
        public WindowBase OpenWindow(WindowType windowType, Transform parent, Action<WindowBase> openedCallback = null)
        {
            var windowBase = GetOrCreateWindow(windowType, parent);

            windowBase.Open(parent);
            openedCallback?.Invoke(windowBase);

            return windowBase;
        }

        public WindowBase OpenWindow(WindowType windowType, bool isPopup = false)
        {
            if (isPopup)
            {

            }
            return OpenWindow(windowType, isPopup ? windowsParentOverlay : windowsParentInGame);
        }
        public WindowBase CloseWindow(WindowType windowType, bool isInstant = true)
        {
            Assert.IsTrue(windowPrefabsDict.ContainsKey(windowType));

            if (!windowPrefabsDict.ContainsKey(windowType)) return null;

            if (instantiatedWindowsDict.TryGetValue(windowType, out var window))
            {
                if (window.transform.parent == windowsParentOverlay)
                {
                    int numPopupsOpen = 0;
                    foreach (Transform child in windowsParentOverlay)
                    {
                        if (child.gameObject.activeSelf)
                        {
                            numPopupsOpen++;
                        }
                    }


                }
                window.Close();
                return window;
            }
            else
            {
                Debug.LogWarning($"the window {windowType.ToString()} wasn't opened yet");
                return null;
            }
        }
    }

}
