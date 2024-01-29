using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TimeWarpRandomUtils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        private static object _lock = new object();
        private static bool _applicationQuitting;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_applicationQuitting) return null;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject obj = new GameObject();
                            obj.name = typeof(T).Name;
                            _instance = obj.AddComponent<T>();
                        }
                    }
                    Assert.IsNotNull(_instance);
                    return _instance;
                }
            }
        }

        /// <summary>
        /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
            _applicationQuitting = false;
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

        private void OnApplicationQuit()
        {
            _applicationQuitting = true;
        }
    }
}


