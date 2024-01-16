using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using PactRandomUtils;

namespace PactRandomUtils
{
    public class ActionTaskHandler : MonoBehaviour
    {

        [TextArea(10, 20)]
        public string callStack;
        public System.Action action;
        InvokeData invokeData;

        float startStamp;

        [DrawnNonSerialized]
        public IEnumerator frameSlicedIenumerator;
        public int debugframeSliceMovenextsRun;

        public void Invoke(InvokeData invokeData)
        {
            this.invokeData = invokeData;
            startStamp = GetTime();
        }

        public void StartSomeCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(Please(coroutine));
        }

        public void RunFrameSlicedIenumerator(IEnumerator frameSlicedIenumerator)
        {
            this.frameSlicedIenumerator = frameSlicedIenumerator;
        }
        bool unfinishedCoroutine;
        IEnumerator Please(IEnumerator ienum)
        {
            unfinishedCoroutine = true;
            yield return ienum;
            unfinishedCoroutine = false;
            Destroy(gameObject);
        }


        float GetTime()
        {
            return (invokeData.ignoreTimeScale ? Time.unscaledTime : Time.time);
        }

        void OnDisable()
        {
            Destroy(gameObject);
        }



        private void OnDestroy()
        {
            if (unfinishedCoroutine)
            {
                UnityEngine.Debug.Log($"Invoke Action - Destroying Action Task Handler with unfinished rountine {Time.time} in scene: {gameObject.scene.name} \n{callStack}", gameObject);
            }
        }

        void TryDestroy(bool invoked)
        {
            if (invoked)
            {
                if (!invokeData.infinite/*&& invokeData.destroyAfter*/)
                {
                    Destroy(gameObject);
                }
                else
                {
                    startStamp = GetTime();
                }
            }
        }

        void Update()
        {
            if (invokeData != null && invokeData.hasScope && invokeData.scope == null)
            {
                UnityEngine.Debug.Log($"Invoke Action - destryoing action task handler that references a gameobject that no longer exists {Time.time} \n{callStack}", gameObject);
                Destroy(gameObject);
                return;
            }

            if (frameSlicedIenumerator != null)
            {
                try
                {
                    if (!frameSlicedIenumerator.MoveNext())
                    {
                        Destroy(gameObject);
                    }
                    debugframeSliceMovenextsRun++;
                }
                catch (System.Exception)
                {
                    Destroy(gameObject);
                    throw;
                }

            }
            //			if (coroutineRunner && coroutine == null) {
            //				Destroy (gameObject);
            //			}

            if (invokeData == null)
                return;
            bool invoked = false;
            if (invokeData.condition != null)
            {
                if (invokeData.condition())
                {
                    InvokeTheAction();
                    invoked = true;
                }
            }
            else
            {
                if (startStamp + invokeData.duration <= GetTime())
                {
                    InvokeTheAction();
                    invoked = true;
                }
            }
            invokeData.repeatAction?.Invoke();
            TryDestroy(invoked);
        }

        void InvokeTheAction()
        {
            try
            {
                invokeData.action();
            }
            catch (System.Exception ex)
            {
                var actionstr = action == null ? "NULL ACTION" : action.Target + "." + action.Method;
                UnityEngine.Debug.LogError(string.Format("ActionTaskHandler " + actionstr + " cached callstack: {0}", callStack));
                TryDestroy(true);
                //throw ex;
                UnityEngine.Debug.LogException(ex);
            }
        }

    }

    public class InvokeData
    {
        public string taskName;
        public System.Func<bool> condition;
        public System.Action action;
        public float duration;
        public bool infinite = false;
        public bool ignoreTimeScale;
        internal GameObject scope;
        internal bool hasScope;
        public Action repeatAction;

        //public bool destroyAfter = false; //set true fromcreatetaskhandler, but otherwise false if used on objects also used for other things
    }


    public static class InvokeAction
    {
        public static Transform persistentTaskHandlers = null;
        static Dictionary<string, GameObject> taskHandlerParents = new Dictionary<string, GameObject>();
        static ActionTaskHandler CreateTaskHandler(bool dontDestroyOnLoad, GameObject scope)
        {
            GameObject go = new GameObject();
            go.name = "TaskHandler";
            if (dontDestroyOnLoad)
            {

                // dont parent dont destroy on load taskhanlder, there must be some bug in unity that causes it not to work
                //	if (persistentTaskHandlers == null)
                //    {
                //        GameObject pgo = GameObject.Find("Persistent Task Handlers");
                //        if (pgo) persistentTaskHandlers = go.transform;
                //        if (persistentTaskHandlers == null)
                //        {
                //			UnityEngine.Debug.Log("creating new persistent taskhandler");
                //			persistentTaskHandlers = new GameObject("Persistent Task Handlers").transform;
                //        }
                //		UnityEngine.Object.DontDestroyOnLoad(persistentTaskHandlers.gameObject);
                //	}
                //	go.transform.SetParent(persistentTaskHandlers);
                UnityEngine.Object.DontDestroyOnLoad(go);
            }
            else
            {
                Scene parentScene = scope == null ? UnityEngine.SceneManagement.SceneManager.GetActiveScene() : scope.scene;
                if (!taskHandlerParents.ContainsKey(parentScene.name)) taskHandlerParents.Add(parentScene.name, GetNewParent(parentScene));
                GameObject parentgo = taskHandlerParents[parentScene.name];
                if (parentgo == null) parentgo = taskHandlerParents[parentScene.name] = GetNewParent(parentScene);
                go.transform.parent = parentgo.transform;
            }
            var actionTaskHandler = AddActionTaskHandlerToGo(go);
            //	if (dontDestroyOnLoad) UnityEngine.Object.DontDestroyOnLoad(actionTaskHandler);
            //	if (dontDestroyOnLoad) UnityEngine.Object.DontDestroyOnLoad(go);
            return actionTaskHandler;
        }

        private static GameObject GetNewParent(Scene scene)
        {
            UnityEngine.Debug.Log("ActionTaskHanlder - creating new non persistent parent. t: " + Time.time);
            GameObject go = new GameObject("action task handler parent, created" + Time.time);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);
            return go;
        }
        public static ActionTaskHandler AddActionTaskHandlerToGo(GameObject go)
        {
            var actionTaskHandler = go.AddComponent<ActionTaskHandler>();
#if UNITY_EDITOR
            StackTrace stackTrace = new StackTrace();
            // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();
            // get method calls (frames)
            foreach (StackFrame stackFrame in stackFrames)
            {
                actionTaskHandler.callStack += stackFrame.GetMethod().DeclaringType.Name + " " + (stackFrame.GetMethod().Name) + "\n";
            }
#endif
            return actionTaskHandler;
        }

        private static ActionTaskHandler Invoke(InvokeData invokeData, bool dontDestroyOnLoad)
        {
            var handler = CreateTaskHandler(dontDestroyOnLoad, invokeData.scope);
            handler.Invoke(invokeData);
            return handler;
        }

        public static ActionTaskHandler CustomCoroutine(IEnumerator coroutine, bool dontDestroyOnLoad = false)
        {
            if (!Application.isPlaying) UnityEngine.Debug.LogError("ActionTaskHandler - don't use this in editmode");

            var actionTaskHandler = CreateTaskHandler(dontDestroyOnLoad, null);
            actionTaskHandler.StartSomeCoroutine(coroutine);

            return actionTaskHandler;
        }


        public static ActionTaskHandler Invoke(System.Func<bool> condition, System.Action action, bool infinite = false, bool ignoreTimeScale = false, GameObject scope = null, bool dontDestroyOnLoad = false)
        {
            if (condition == null)
                throw new System.NotSupportedException();
            if (!Application.isPlaying) UnityEngine.Debug.LogError("ActionTaskHandler - don't use this in editmode");

            var invkData = new InvokeData();
            invkData.action = action;
            invkData.condition = condition;
            invkData.infinite = infinite;
            invkData.ignoreTimeScale = ignoreTimeScale;
            invkData.hasScope = scope != null;
            invkData.scope = scope;
            return Invoke(invkData, dontDestroyOnLoad);
        }

        public static ActionTaskHandler Invoke(System.Action action, float duration, bool infinite = false, bool ignoreTimeScale = false, GameObject scope = null, bool dontDestroyOnLoad = false)
        {
            if (!Application.isPlaying) UnityEngine.Debug.LogError("ActionTaskHandler - don't use this in editmode");

            var invkData = new InvokeData();
            invkData.hasScope = scope != null;
            invkData.scope = scope;

            invkData.infinite = infinite;
            invkData.duration = duration;
            invkData.action = action;
            invkData.ignoreTimeScale = ignoreTimeScale;

            return Invoke(invkData, dontDestroyOnLoad);
        }
    }
}


