using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using PactRandomUtils;
namespace Pact.Events
{
    public class GlobalEventManager : PactRandomUtils.Singleton<GlobalEventManager>
    {
        // Listener map: <Listener, <Event, List<Action>>;
        private Dictionary<object, Dictionary<Type, List<object>>> _listenerMap =
            new Dictionary<object, Dictionary<Type, List<object>>>();

        public void Listen<T>(object listener, Action<T> action)
        {
            Listen(listener, typeof(T), (x) => action((T)x));
        }

        public void Listen(object listener, System.Type type, Action<object> action)
        {
            // check if we have something in the listener map
            if (_listenerMap.ContainsKey(listener))
            {
                // get the Event map
                var eventMap = _listenerMap[listener];
                // check if there is something under Type
                if (eventMap.ContainsKey(type))
                {
                    var actionList = eventMap[type];
                    // check if the action is there
                    if (!actionList.Contains(action))
                    {
                        actionList.Add(action);
                    }
                }
                else
                {
                    eventMap.Add(type, new List<object>() { action });
                }
            }
            else // no listener
            {
                _listenerMap.Add(listener,
                    new Dictionary<Type, List<object>>() { { type, new List<object>() { action } } });
            }
        }


        public void Trigger<T>(T @event, bool tooHotForDebugLogs = false)
        {
            // we work on a copy of the map to avoid race condition
            foreach (var lPair in _listenerMap.ToList())
            {
                // lPair <object, List<Type, List<object>>>
                var listener = lPair.Key;
                var actionMap = lPair.Value;
                foreach (var aPair in actionMap)
                {
                    Type actionType = aPair.Key;
                    if (typeof(T) == actionType)
                    {
                        var actions = aPair.Value;
                        foreach (object action in actions)
                        {
                            try
                            {
                                ((Action<object>)action).Invoke(@event);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"#GlobalEventsManager# Failed to run global event {ex}", listener as UnityEngine.Object);
                                Debug.LogException(ex);
                            }
                        }
                    }
                }
            }
        }

        public void RemoveListener<T>(object listener, Action<T> action)
        {
            var actionMapForListener = _listenerMap.GetIfHasKey(listener);
            if (actionMapForListener != null)
            {
                var actionList = actionMapForListener.GetIfHasKey(typeof(T));
                if (actionList != null)
                {
                    var res = actionList.Remove((object)action);
                }
            }
        }

        public void RemoveAllListenersFor(object listener)
        {
            var res = _listenerMap.Remove(listener);
        }

        public void RemoveAllListenersFor<T>(object listener)
        {
            var actionMap = _listenerMap.GetIfHasKey(listener);
            if (actionMap == null)
            {
                return;
            }
            var res = actionMap.Remove(typeof(T));
            if (res)
            {
                Debug.Log($"#GlobalEventsManager# Action {typeof(T)} was removed for {listener}");
            }
        }
    }
}


