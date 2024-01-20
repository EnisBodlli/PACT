using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PactRandomUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PactRandomUtils;

namespace PactRandomUtils
{
    public class SingleUseAction
    {
        List<System.Action> actions = new List<System.Action>();
        List<KeyValuePair<System.Action, Object>> actionsWithContext = new List<KeyValuePair<System.Action, Object>>();


        public int RegisteredCount
        {
            get
            {
                return actions.Count + actionsWithContext.Count;
            }
        }

        void TryInvokeAction(System.Action action)
        {
            try
            {
                action();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }




        public void InvokeAllAndClear()
        {
            actions.ForEach(TryInvokeAction);
            actions.Clear();
            actionsWithContext.ForEach(x => {
                if (x.Value != null)
                    TryInvokeAction(x.Key);
            });
            actionsWithContext.Clear();
        }

        public void Register(System.Action action)
        {
            actions.Add(action);
        }


        public static SingleUseAction operator +(SingleUseAction target, System.Action action)
        {
            target.actions.Add(action);
            return target;
        }
        public void Register(System.Action action, Object context)
        {
            actionsWithContext.Add(new KeyValuePair<System.Action, Object>(action, context));
        }

        public void Remove(System.Action action)
        {
            actions.Remove(actions.Single(x => x == action));
        }

    }

    public static class RandomUtils
    {
        /// <summary>
        /// Functional log
        /// </summary>
        public static string Flog(this string str, string prefix)
        {
            Debug.Log(string.Format("{0}: {1}", prefix, str));
            return str;
        }
        public static bool IsNullOrEmpty<T>(this IList<T> ilist)
        {
            return ilist == null || !ilist.Any();
        }

        public static bool None(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        //		public static IEnumerator<T> GetComponentFromDragAndDrop<T>(GameObject inspectedGameobject) where T:Component{
        //			Object[] objectReferences = null;
        //			while (Event.current.type != EventType.dragUpdated) {
        //				yield return null;
        //			}
        //			while () {
        //				objectReferences = UnityEditor.DragAndDrop.objectReferences;
        //				yield return null;
        //			}
        //
        //			Event.current.Use();
        //			yield return ; 
        //
        //
        //		}


        /// <summary>
        /// https://medium.com/@bonnotguillaume/software-architecture-id-sequence-number-obfuscation-scrambler-33a00b525459
        /// </summary>
        public static int TauswortheScrambler(int seed)
        {
            seed ^= seed >> 13;
            seed ^= seed << 18;
            seed &= 0x7FFFFFFF;
            return seed;
        }


        public static T CycleNextOrDefault<T>(this List<T> list, ref int currentIndex)
        {
            if (list.Count == 0)
            {
                currentIndex = -1;
                return default(T);
            }
            currentIndex++;
            if (list.Count <= currentIndex)
            {
                currentIndex = 0;
            }
            return list[currentIndex];
        }
        public const string SENSIBLE_VERBOSE_DATETIME_STR = "dd MMMM yyyy hh:mm tt";
        public const string LOGGABLE_DATETIME_STR = "dd_MM_yy___HHmm_ss";
        /// <summary>
        /// more perf heavy than the foreach one, only use id you need to remove from the collction
        /// </summary>
        public static void ForEachToListed<T>(this IEnumerable<T> ienum, System.Action<T> action)
        {
            foreach (var item in ienum.ToList())
            {
                action(item);
            }
        }

        public static float RelationToPercentage(float value, float reference)
        {
            return (value / reference) * 100;
        }
        public static void ForEach<T>(this IEnumerable<T> ienum, System.Action<T> action)
        {
            foreach (var item in ienum)
            {
                action(item);
            }
        }

#if UNITY_EDITOR
        public static void BeginSpamLayoutHorizontal()
        {
            if (Event.current.keyCode == KeyCode.KeypadPlus)
            {
                Debug.Log("BeginSpamLayoutHorizontal");
            }
            UnityEditor.EditorGUILayout.BeginHorizontal();
        }
        public static void EndSpamLayoutHorizontal()
        {
            if (Event.current.keyCode == KeyCode.KeypadPlus)
            {
                Debug.Log("EndSpamLayoutHorizontal");
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
        }

        public static string GetFolderPath(UnityEngine.Object obj)
        {
            var toPathInsideAssets = AssetDatabase.GetAssetPath(obj);
            // Application.dataPath goes inside asset folder, which we don't want
            var datapath = new System.IO.DirectoryInfo(Application.dataPath).Parent;
            List<KeyValuePair<string, string>> operations = new List<KeyValuePair<string, string>>();
            return $"{datapath}/{toPathInsideAssets}";
        }
#endif

        public static string GetScenePath(Transform tr)
        {
            var ts = tr.GetRecurseWhileNotNull(x => x.parent);
            ts.Reverse();
            return tr.gameObject.scene.name + "." + ts.Aggregate("", (x, y) => x + "." + y.gameObject.name);
        }

        /// <summary>
        ///  funfact: does not work if you call it syncronously on from OnEnable when level loads (unity 2019.4.18f1) - start does work though
        /// </summary>
        public static List<T> GetComponentInChildrenFromRootGOs<T>(bool includeDisabled = true) where T : class
        {
            var list = new List<T>();
            GetComponentInChildrenFromRootGOs(list, includeDisabled);
            return list;
        }




        public static System.Type GetElementType(this IList ilist)
        {
            System.Type elementType;
            if (ilist is System.Array)
            {
                elementType = ilist.GetType().GetElementType();
            }
            else
            {
                elementType = ilist.GetType().GetGenericArguments()[0];
            }
            return elementType;
        }
        static char[] alphabet = new char[] { 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z', 'z' };

        public static string RandomString(int lenght)
        {
            string str = "";
            for (int i = 0; i < lenght; i++)
            {
                str += alphabet.RandomItem();
            }
            return str;
        }

#if UNITY_EDITOR
        public static string EditorGetAbsoluteFilepath(UnityEngine.Object obj)
        {
            return Application.dataPath + UnityEditor.AssetDatabase.GetAssetPath(obj).Replace("Assets", "");
        }
#endif
        public static float RunningAverage(float oldAverage, float newValue, int sampleCount)
        {
            return (oldAverage * sampleCount + newValue) / (sampleCount + 1);
        }
        /// <summary>
        /// </summary>
        /// <param name="oldAverage"></param>
        /// <param name="newPosition"></param>
        /// <param name="prevSampleCount"> </param>
        /// <returns></returns>
        public static Vector3 RunningAveragePosition(Vector3 oldAverage, Vector3 newPosition, int prevSampleCount)
        {
            Vector3 value = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                value[i] = (oldAverage[i] * prevSampleCount + newPosition[i]) / (prevSampleCount + 1);
            }
            return value;
        }
        public static IList CreateCopyList(this IList target)
        {

            var iList = target as IList;
            if (iList == null) throw new System.NotSupportedException();
            var newIList = System.Activator.CreateInstance(iList.GetType()) as IList;
            for (int i = 0; i < iList.Count; i++)
            {
                newIList.Add(iList[i]);
            }
            return newIList;
        }


        public static void ExecuteWhenAnimated(this Animator anmtr, string stateName, int layer, System.Action doOnComplete)
        {
            var taskHandler = InvokeAction.CustomCoroutine(ExecutingWhenAnimated(anmtr, stateName, layer, doOnComplete));
            taskHandler.transform.SetParent(anmtr.transform);
        }
        static IEnumerator ExecutingWhenAnimated(Animator anmtr, string stateName, int layer, System.Action doOnComplete)
        {
            while (!anmtr.GetCurrentAnimatorStateInfo(layer).IsName(stateName)) yield return null;
            while (anmtr.GetCurrentAnimatorStateInfo(layer).IsName(stateName) && anmtr.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f) yield return null;
            doOnComplete();
        }

        public static bool LazyEditorCheck(ref double lastCheck, double interval = 1)
        {
#if UNITY_EDITOR
            if (EditorApplication.timeSinceStartup - lastCheck > interval)
            {
                lastCheck = EditorApplication.timeSinceStartup;
                return true;
            }
#endif
            return false;

        }
        public static string GetAnimatedThreePeriods()
        {
            string str = "";
            for (int i = 0; i < (Time.realtimeSinceStartup * 3) % 3; i++)
            {
                str += '.';
            }
            return str;
        }
        public static float GetValue(this System.Random random)
        {
            return random.Next(0, 1001) * 0.001f;
        }

        public static T TryGetAssetWithName<T>(string assetName, string prefix = ".prefab", bool hardFail = true) where T : UnityEngine.Object
        {
            T unityEngineObject = default(T);
#if UNITY_EDITOR
            var assetPaths = UnityEditor.AssetDatabase.FindAssets(assetName).Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x));
            try
            {
                unityEngineObject = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPaths.Single(x => System.IO.Path.GetFileName(x) == assetName + prefix));

            }
            catch (System.Exception)
            {
                if (hardFail)
                {
                    Debug.LogError("diddv find " + assetName + prefix + " . heres what i found tho " + assetPaths.GivePrintOut());
                    throw;
                }
            }
#endif
            return unityEngineObject;
        }
        /// <summary>
        /// thank you unity 2019
        /// </summary>
        public static void OptimizedSetActive(this GameObject target, bool setTo)
        {
            if (target.activeSelf != setTo) target.SetActive(setTo);
        }

        /// <summary>
        /// serves no other purpose than to get undorecord object on a single line
        /// </summary>
        public static System.Action FunctionalUndoRecord(this UnityEngine.Object toBeRecorded, System.Action returnedAction)
        {

            return () =>
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(toBeRecorded, "");
#endif
                returnedAction();
            };
        }

        public static void GetComponentInChildrenFromRootGOs<T>(List<T> nonAlloc, bool includeDisabled = true) where T : class
        {
            bool log = true;
#if UNITY_EDITOR
            log = EditorPrefs.GetBool("LOG_FINDOBJECTS_OF_TYPE_PERF_IN_EDITOR");
#endif

            //			return Enumerable.Range (0, UnityEngine.SceneManagement.SceneManager.sceneCount).Select (x => UnityEngine.SceneManagement.SceneManager.GetSceneAt (x)).Where (x => x.isLoaded).ToList ();
            //			var components = GetOpenScenes ().SelectMany (y => y.GetRootGameObjects ().SelectMany (x => x.GetComponentsInChildren (typeof(T), includeDisabled))).ToArray ();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                {
                    continue;
                }
                //				var components = GetOpenScenes ().SelectMany (y => y.GetRootGameObjects ().SelectMany (x => x.GetComponentsInChildren (typeof(T), includeDisabled))).ToArray ();
                var rootGos = scene.GetRootGameObjects();
                for (int j = 0; j < rootGos.Length; j++)
                {
                    //					nonAlloc.AddRange( );
                    var getComp = rootGos[j].GetComponentsInChildren(typeof(T), includeDisabled);
                    for (int k = 0; k < getComp.Length; k++)
                    {
                        nonAlloc.Add(getComp[k] as T);
                    }
                }
            }


        }

#if UNITY_EDITOR
        public static List<T> FindAssetsByType<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
#endif

        public static System.Text.StringBuilder PrintAllMembers(this object obj)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (var fieldInfo in obj.GetType().GetFields())
            {
                sb.Append(fieldInfo.Name + ": ");
                var value = fieldInfo.GetValue(obj);
                if (value == null)
                {
                    sb.Append($"null");

                }
                else if (value is IEnumerable list && fieldInfo.FieldType != typeof(string))
                {
                    foreach (var ob in list)
                    {
                        sb.Append($"\n\t{ob}");
                    }
                }
                else
                {
                    sb.Append(fieldInfo.GetValue(obj));
                }

                sb.Append("\n");
            }

            return sb;
        }
        public static TResult SelectSingleNonNull<T, TResult>(this IEnumerable<T> ienum, System.Func<T, TResult> queryCriteria) where TResult : class
        {
            return ienum.Select(queryCriteria).VerboseSingle(x => x != null);
        }

        public static T VerboseSingle<T>(this IEnumerable<T> ienum, System.Func<T, bool> queryCriteria)
        {
            T foundItem = default(T);
            bool foundOneMatch = false;
            foreach (var item in ienum)
            {
                bool currentIsMatch = queryCriteria(item);
                if (currentIsMatch)
                {
                    if (foundOneMatch)
                    {
                        Debug.LogError("VerboseSingle fail, reason: MULTIPLE MATCHES of type: " + typeof(T) + "  LOGGING MATCHING ITEMS");
                        foreach (var matchingItem in ienum.Where(x => queryCriteria(x)))
                        {
                            Debug.LogError(matchingItem, matchingItem as UnityEngine.Object);
                        }
                        throw new System.InvalidOperationException();
                    }
                    else
                    {
                        foundItem = item;
                        foundOneMatch = true;
                    }
                }

            }
            if (!foundOneMatch)
            {
                throw new System.InvalidOperationException("VerboseSingle fail, reason: NO MATCHES of type" + typeof(T));
            }
            return foundItem;

        }

        public static T WeightedRandomItem<T>(this IList<T> list, System.Func<T, int> getWeight, System.Random sysRandom)
        {
            if (list.Count == 0) throw new System.NotSupportedException("nothing to random from");

            if (list.Count == 1) return list[0];
            int[] weightIntervals = new int[list.Count];
            int totalWeight = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var weight = getWeight(list[i]);
                if (weight < 0)
                {
                    throw new System.Exception("positive vibes only");
                }
                weightIntervals[i] = totalWeight;
                totalWeight += weight;
            }
            var value = sysRandom.Next(0, totalWeight);
            for (int i = weightIntervals.Length - 1; i >= 0; i--)
            {
                if (value >= weightIntervals[i])
                {
                    return list[i];
                }
            }
            throw new System.Exception();
        }


        public static TResult SelectSingleOrDefaultNonNull<T, TResult>(this IEnumerable<T> ienum, System.Func<T, TResult> queryCriteria) where TResult : class
        {
            return ienum.Select(queryCriteria).SingleOrDefault(x => x != null);
        }
        public static void ExecForNonNull<T>(this T obj, System.Action<T> action) where T : class
        {
            if (obj == null)
            {
                return;
            }
            action(obj);

        }
        public static T2 GetFromNonNullOrDefault<T, T2>(this T obj, System.Func<T, T2> action) where T : class
        {
            if (obj == null)
            {
                return default(T2);
            }
            return action(obj);

        }

        /// <summary>
        /// Creates the random file in my documents that contains string.
        /// </summary>
        /// <returns>The PATH to THE file.</returns>
        /// <param name="data">Data.</param>
        /// <param name="subFolder">subfolder inside (no backslashes)</param>
        public static string CreateFileInMyDocumentsThatContainsString(string data, string subFolder = "", string prefix = "")
        {
            string folderName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            string pathString = folderName;
            if (subFolder != "")
            {
                pathString += $"\\{subFolder}";
                System.IO.Directory.CreateDirectory(pathString);
            }


            /*			string pathString = System.IO.Path.Combine(folderName, "SubFolder");                          
                        string pathString2 = @"c:\Top-Level Folder\SubFolder2";  									
                        System.IO.Directory.CreateDirectory(pathString); 											*/

            var regex = new System.Text.RegularExpressions.Regex("[A-Za-z]");
            prefix = regex.Match(prefix).Value;

            string fileName = System.IO.Path.ChangeExtension($"{prefix}_{System.IO.Path.GetRandomFileName()}", ".txt");
            pathString = System.IO.Path.Combine(pathString, fileName);
            System.IO.File.WriteAllText(pathString, data);
            return pathString;
        }

#if UNITY_EDITOR

        public static IEnumerator EditorGUIPickPrefabWithFilter(string filter, System.Action<GameObject> onPicked)
        {
            var controlID = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, filter, controlID);

            Object prefab = null;
            while (EditorGUIUtility.GetObjectPickerControlID() == controlID)
            {
                EditorGUILayout.HelpBox("Pick Tile type", MessageType.Info);
                if (Event.current.commandName == "ObjectSelectorUpdated")
                {
                    prefab = EditorGUIUtility.GetObjectPickerObject();
                    break;
                }
                yield return null;
            }
            onPicked(prefab as GameObject);
        }

#endif


        public static string[] EasySplit(this string input, string splitKey)
        {
            return input.Split(new[] { splitKey }, System.StringSplitOptions.None);
        }
        public static T CyclePreviousOrDefault<T>(this List<T> list, ref int currentIndex)
        {
            if (list.Count == 0)
            {
                currentIndex = -1;
                return default(T);
            }
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = list.Count - 1;
            }
            return list[currentIndex];
        }

        public static void For<T>(int count, System.Action<int> action)
        {
            for (int i = 0; i < count; i++)
            {
                action(count);
            }
        }
        public static void ForEach<T>(this T[] ar, System.Action<T> action)
        {
            foreach (var item in ar)
            {
                action(item);
            }
        }

        /*	public static bool IfNotNullThen<T>(this T target, System.Func<T,bool> returnFunc) where T: class{
			return target == null ? false : returnFunc (target);
		}*/

        public static T[] GarbageHeavyArrayAdd<T>(this T[] ar, T item)
        {
            var list = ar.ToList();
            list.Add(item);
            return list.ToArray();
        }

        public static Vector3 LeaveMarker(this Vector3 pos, string name = "", float time = -1f)
        {

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var collider = go.GetComponent<Collider>();
            collider.enabled = false;
            Object.Destroy(collider);
            go.transform.localScale *= 0.1f;
            go.transform.position = pos;
            if (name == "")
            {
                go.name = string.Format("marker: {0}", pos);
            }
            else
            {
                go.name = string.Format("{0}{1}", name, pos);
            }
            if (time > 0)
            {
                InvokeData invokeData = new InvokeData();
                invokeData.action = () => Object.Destroy(go);
                invokeData.duration = time;
                go.AddComponent<ActionTaskHandler>().Invoke(invokeData);
            }
            return pos;
        }
        public static List<T2> CreateNewListOrAddToOld<T1, T2>(this Dictionary<T1, List<T2>> dic, T1 key, T2 val)
        {
            if (!dic.ContainsKey(key))
            {
                var newList = new List<T2>();
                dic.Add(key, newList);
            }

            List<T2> list = dic[key];
            list.Add(val);
            return list;
        }
        public static Transform CreateZeroPosChild(this Transform trans, string childName)
        {
            var go = new GameObject();
            go.name = childName;
            go.transform.SetParent(trans);
            go.transform.localPosition = Vector3.zero;
            return go.transform;
        }

        public static bool IsApproximitelySame(this Vector3 a, Vector3 b, float tolerance = 0.02f)
        {
            return IsApproximitelySame(a, b, Vector3.one * tolerance);
        }
        public static bool IsApproximitelySame(this Vector3 a, Vector3 b, Vector3 v3Tolerance)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Mathf.Abs(a[i] - b[i]) > v3Tolerance[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static Vector3 Abs(this Vector3 v3)
        {
            return new Vector3(Mathf.Abs(v3.x), Mathf.Abs(v3.y), Mathf.Abs(v3.z));
        }

        public static void DrawDebugLineBox(this Bounds bounds, float time = 5f, Color color = default(Color), bool draw = true)
        {
            DrawDebugLineBox(bounds, Quaternion.identity, time, color, draw);
        }
        public static void DrawDebugLineBox(this Bounds bounds, Quaternion rotation, float time = 5f,
                                                  Color color = default(Color), bool draw = true)
        {
#if !UNITY_EDITOR
			return;
#endif
            if (!draw)
            {
                return;
            }
            var layer = GetBoundsQuadOnXZAxises(bounds);
            for (int i = 1; i < layer.Length; i++)
            {
                Debug.DrawLine(rotation * (layer[i] + (Vector3.up * bounds.extents.y)),
                    rotation * (layer[i - 1] + (Vector3.up * bounds.extents.y)), color, time);
                Debug.DrawLine(rotation * (layer[i] - (Vector3.up * bounds.extents.y)),
                    rotation * (layer[i - 1] - (Vector3.up * bounds.extents.y)), color, time);
            }
            Debug.DrawLine(rotation * (layer[0] + (Vector3.up * bounds.extents.y)),
                rotation * (layer[3] + (Vector3.up * bounds.extents.y)), color, time);
            Debug.DrawLine(rotation * (layer[0] - (Vector3.up * bounds.extents.y)),
                rotation * (layer[3] - (Vector3.up * bounds.extents.y)), color, time);
            for (int i = 0; i < layer.Length; i++)
            {
                Debug.DrawLine(rotation * (layer[i] - (Vector3.up * bounds.extents.y)),
                    rotation * (layer[i] + (Vector3.up * bounds.extents.y)), color, time);
            }
        }

        public static void DrawGizmoLineBounds(this Bounds bounds, Quaternion rotation)
        {
#if UNITY_EDITOR
            //	var layer = GetBoundsQuadOnXZAxises(bounds);
            //	for (int i = 1; i < layer.Length; i++)
            //	{
            //		Gizmos.DrawLine(rotation * (layer[i] + (Vector3.up * bounds.extents.y)),
            //			rotation * (layer[i - 1] + (Vector3.up * bounds.extents.y)) );
            //		Gizmos.DrawLine(rotation * (layer[i] - (Vector3.up * bounds.extents.y)),
            //			rotation * (layer[i - 1] - (Vector3.up * bounds.extents.y)));
            //	}
            //
            //	Gizmos.DrawLine(rotation * (layer[0] + (Vector3.up * bounds.extents.y)),
            //		rotation * (layer[3] + (Vector3.up * bounds.extents.y)));
            //	Gizmos.DrawLine(rotation * (layer[0] - (Vector3.up * bounds.extents.y)),
            //		rotation * (layer[3] - (Vector3.up * bounds.extents.y)));
            //	for (int i = 0; i < layer.Length; i++)
            //	{
            //		Gizmos.DrawLine(rotation * (layer[i] - (Vector3.up * bounds.extents.y)),
            //			rotation * (layer[i] + (Vector3.up * bounds.extents.y)));
            //	}


            //		Vector3 prev = new Vector3();
            //		Vector3 ex = bounds.extents;
            //		bool first = true;
            //		for (int i = -1; i < 2; i++)
            //		{
            //			for (int j = -1; j < 2; j++)
            //			{
            //				for (int k = -1; k < 2; k++)
            //				{
            //					if (i == 0 || j == 0 || k == 0)
            //					{
            //						continue;
            //					}
            //
            //					Vector3 pos = bounds.center;
            //					pos.x += bounds.extents.x * i;
            //					pos.y += bounds.extents.y * j;
            //					pos.z += bounds.extents.z * k;
            //					//pos += new Vector3(i, j, k);
            //					if (!first)
            //					{
            //						Gizmos.DrawLine(prev, pos);
            //					}
            //					prev= pos;
            //					first = false;
            //				}
            //			}
            //		}



            //		Vector3 offset = bounds.extents ;
            //		Vector3 dir = -bounds.size;



            //	
            //	DrawCorner(bounds, offset, dir);

            //	offset = Quaternion.Euler(180, 0, 0) * offset; 
            //	dir = Quaternion.Euler(180, 0, 0) * dir; 
            //	
            //	DrawCorner(bounds, offset, dir);

            //	
            //	offset = Quaternion.Euler(0, 180, 0) * offset;
            //	dir = Quaternion.Euler(0, 180, 0) * dir;
            //	
            //	DrawCorner(bounds, offset, dir);
            //	
            //	offset = Quaternion.Euler(180, 0, 0) * offset;
            //	dir = Quaternion.Euler(180, 0, 0) * dir;
            //DrawCorner(bounds, offset, dir);

            //		for (int i = 0; i < 2; i++)
            //		{
            //			Vector3 prev =bounds.center +   rotation * (cubeCorners[4 * i].MultiplyAxises(bounds.extents));
            //			
            //			for (int j = 1; j < 4; j++)
            //			{
            //				Gizmos.DrawLine(prev,prev =bounds.center +  rotation * cubeCorners[(4*i) + j].MultiplyAxises( bounds.extents) );
            //			}
            //			Gizmos.DrawLine(bounds.center +   rotation *(  cubeCorners[(i*4)+ 0].MultiplyAxises(bounds.extents) ), bounds.center + rotation *  cubeCorners[(i*4)+3].MultiplyAxises(bounds.extents) );
            //		}
            //
            //		for (int i = 0; i < 4; i++)
            //		{
            //			Gizmos.DrawLine(bounds.center +  rotation * (cubeCorners[i].MultiplyAxises(bounds.extents) ),bounds.center +  rotation *( cubeCorners[i+4].MultiplyAxises(bounds.extents) ) );
            //		}

            for (int dir = -1; dir < 2; dir++)
            {
                if (dir == 0)
                {
                    continue;
                }

                var corner1 = bounds.center + (rotation * (bounds.extents * dir));
                for (int i = 0; i < 3; i++)
                {
                    Vector3 offset = new Vector3();
                    offset[i] = bounds.size[i] * -dir;
                    Gizmos.DrawLine(corner1, corner1 + (rotation * offset));
                }
            }

#endif

        }

        private static Vector3[] cubeCorners = new Vector3[]
        {
            new Vector3(1, -1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),

            new Vector3(1, -1, -1),
            new Vector3(-1, -1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(1, 1, -1),

        };
        //	private static void DrawCorner(Bounds bounds, Vector3 offset, Vector3 dir)
        //	{
        //		Gizmos.DrawLine(bounds.center + offset, bounds.center + offset + dir.MultiplyAxises(1, 0, 0));
        //		Gizmos.DrawLine(bounds.center + offset, bounds.center + offset + dir.MultiplyAxises(0, 1, 0));
        //		Gizmos.DrawLine(bounds.center + offset, bounds.center + offset + dir.MultiplyAxises(0, 0, 1));
        //	}
        //
        public static string RemoveStuffBefore(this string str, string removePoint)
        {
            return str.Split(new[] { removePoint }, System.StringSplitOptions.None)[1];
        }


        static Vector3[] GetBoundsQuadOnXZAxises(Bounds bounds)
        {
            Vector3[] layer = new Vector3[4];
            layer[0] = bounds.extents.MultiplyAxises(-1, 0, 1) + bounds.center;
            layer[1] = bounds.extents.MultiplyAxises(1, 0, 1) + bounds.center;
            layer[2] = bounds.extents.MultiplyAxises(1, 0, -1) + bounds.center;
            layer[3] = bounds.extents.MultiplyAxises(-1, 0, -1) + bounds.center;
            return layer;
        }

        public static Vector3 SameButDifferent(this Vector3 target, float? x = null, float? y = null, float? z = null)
        {
            float?[] settings = { x, y, z };
            for (int i = 0; i < 3; i++)
            {
                if (settings[i] != null)
                    target[i] = settings[i].Value;
            }
            return target;

        }
        public static void AddOrChangeDictionaryValue<Tone, Ttwo>(this Dictionary<Tone, Ttwo> dic, Tone key, Ttwo value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }

        public static System.Text.StringBuilder globalStringMaker = new System.Text.StringBuilder();

        public static int Sign(this int i)
        {
            return i >= 0 ? 1 : -1;
        }

        public static void CopyRectTransformTo(this RectTransform rectT, RectTransform target)
        {
            target.pivot = rectT.pivot;
            target.anchorMin = rectT.anchorMin;
            target.anchorMax = rectT.anchorMax;
            target.sizeDelta = rectT.sizeDelta;
            target.anchoredPosition = rectT.anchoredPosition;
        }
        public static List<T> GetRecurseWhileNotNull<T>(this T start, System.Func<T, T> getter) where T : class
        {
            var current = start;
            List<T> path = new List<T>();
            while (current != null)
            {
                path.Add(current);
                current = getter(current);
            }
            return path;
        }
        public static List<T> FlattenTree<T>(this T start, System.Func<T, IEnumerable<T>> getter) where T : class
        {
            List<T> allNodes = new List<T>();
            allNodes.Add(start);
            List<T> unsearched = allNodes.ToList();
            while (true)
            {
                for (int i = unsearched.Count - 1; i >= 0; i--)
                {
                    unsearched.AddRange(getter(unsearched[i]));
                    unsearched.RemoveAt(i);
                }
                if (unsearched.Count == 0)
                    break;
            }
            return allNodes;
        }
        public static List<UnityEngine.SceneManagement.Scene> GetOpenScenes()
        {
            return Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCount).Select(x => UnityEngine.SceneManagement.SceneManager.GetSceneAt(x)).Where(x => x.isLoaded).ToList();
        }

#if UNITY_EDITOR
        public static bool EditorGUIFilterPopUp(string label, string[] displayedOptions, ref string currentSelected, ref string textFieldStr, UnityEngine.Object undoRecordedObject, bool endHorizontal = true)
        {
            int evaluatedIndex;
            return EditorGUIFilterPopUp(label, displayedOptions, ref currentSelected, ref textFieldStr, undoRecordedObject, out evaluatedIndex, endHorizontal);
        }

        public static bool EditorGUIFilterPopUp(string label, string[] displayedOptions, ref string currentSelected, ref string textFieldStr, UnityEngine.Object undoRecordedObject, out int evaluatedIndex, bool endHorizontal = true, EditorGUIFilterPopUpDrawingParams overrideDrawing = null)
        {
            bool result = false;
            var drawingMethod = overrideDrawing == null ? defaultDrawing : overrideDrawing;
            try
            {
                result = DrawFilterPopup(label, displayedOptions, ref currentSelected, ref textFieldStr, undoRecordedObject, out evaluatedIndex, drawingMethod);
            }
            catch (System.Exception ex)
            {
                evaluatedIndex = -1;
                Debug.LogException(ex);
            }
            if (endHorizontal && drawingMethod.UseGUILayout)
                EditorGUILayout.EndHorizontal();
            return result;
        }
        static EditorGUIFilterPopUpDrawingParams defaultDrawing = new EditorGUIFilterPopUpDrawingParams();
        public class PropDrawMethods : PactRandomUtils.RandomUtils.EditorGUIFilterPopUpDrawingParams
        {
            public Rect popupPos;
            public Rect textfieldPos;
            public override bool UseGUILayout
            {
                get
                {
                    return false;
                }
            }
            public override int DrawPopup(string label, int index, string[] displayedOptions)
            {
                return EditorGUI.Popup(popupPos, label, index, displayedOptions);
                //				return index;
            }
            public override string DrawTextField(string value)
            {
                return EditorGUI.TextField(textfieldPos, value);
            }
        }
        public class EditorGUIFilterPopUpDrawingParams
        {
            public virtual bool UseGUILayout
            {
                get
                {
                    return true;
                }
            }
            public virtual int DrawPopup(string label, int index, string[] displayedOptions)
            {
                return EditorGUILayout.Popup(label, index, displayedOptions);

            }
            public virtual string DrawTextField(string value)
            {
                return EditorGUILayout.TextField(value);
            }
        }

        static bool DrawFilterPopup(string label, string[] displayedOptions, ref string currentSelected, ref string textFieldStr, Object undoRecordedObject, out int index, EditorGUIFilterPopUpDrawingParams drawingMethod)
        {
            index = System.Array.IndexOf(displayedOptions, currentSelected);
            if (drawingMethod.UseGUILayout) EditorGUILayout.BeginHorizontal();
            index = drawingMethod.DrawPopup(label, index, displayedOptions);
            EditorGUI.BeginChangeCheck();
            textFieldStr = drawingMethod.DrawTextField(textFieldStr);
            if (string.IsNullOrEmpty(textFieldStr) && index == -1)
            {
                EditorGUI.EndChangeCheck();
                return false;
            }
            if (EditorGUI.EndChangeCheck())
            {
                var resultCopyForAnon = textFieldStr;
                var lowerCased = displayedOptions.Select(x => x.ToLower()).ToArray();
                var ordererd = lowerCased.Where(x => x.Contains(resultCopyForAnon.ToLower())).OrderBy(x => Mathf.Abs(x.Length - resultCopyForAnon.Length)).ToList();
                if (!ordererd.Any())
                {
                    return false;
                }
                else
                {
                    string orderingResult = ordererd.First();
                    index = System.Array.FindIndex(lowerCased, x => x == orderingResult);
                }
            }
            if (index == -1) return false;
            if (currentSelected != displayedOptions[index])
            {
                if (undoRecordedObject != null) Undo.RecordObject(undoRecordedObject, "changed value of " + undoRecordedObject.name);
            }
            if (index > -1)
            {
                currentSelected = displayedOptions[index];
                return true;
            }
            else
            {
                currentSelected = "";
                return false;
            }
        }

#endif
        public static void ResetRectTrasform(this RectTransform target)
        {
            target.localPosition = Vector3.zero;
            target.anchorMin = Vector2.zero;
            target.anchorMax = Vector2.one;
            target.sizeDelta = Vector2.one;
            target.anchoredPosition = Vector2.zero;
        }

        public static void AddToOrCreateNewList<T>(this List<List<T>> listlist, int layer, T value)
        {
            if (listlist.Count == layer)
            {
                listlist.Add(new List<T>());
                listlist[layer].Add(value);
            }
            else if (layer < listlist.Count && layer >= 0)
            {
                if (listlist[layer] == null)
                {
                    listlist[layer] = new List<T>();
                }
                listlist[layer].Add(value);
            }
            else
            {
                Debug.LogError("argument out of range " + layer);
            }
        }

        public static Vector3 MultiplyAxises(this Vector3 v, float x, float y, float z)
        {
            return MultiplyAxises(v, new Vector3(x, y, z));
        }
        /// <summary>
        /// remember to set size to one greater the value you want to index
        /// </summary>
        public static List<T> SetSizeTo<T>(this List<T> list, int size)
        {
            while (list.Count > size)
            {
                list.RemoveAt(list.Count - 1);
            }
            while (list.Count < size)
            {
                list.Add(default(T));
            }
            return list;
        }

        public static T InstantiateReturnNew<T>(this T component) where T : Component
        {
            var go = UnityEngine.Object.Instantiate(component.gameObject) as GameObject;
            return go.GetComponent(typeof(T)) as T;
        }

        public static GameObject InstantiateReturnNew(this GameObject component)
        {
            var go = UnityEngine.Object.Instantiate(component.gameObject) as GameObject;
            return go;
        }

        public static T GetFirst<T>(this IEnumerable<T> list, System.Func<T, bool> queryCriteria) where T : class
        {
            foreach (var item in list)
            {
                if (queryCriteria(item))
                {
                    return item;
                }
            }
            return null;
        }

        public static int GetFirstIndex<T>(this IEnumerable<T> list, System.Func<T, bool> queryCriteria) where T : class
        {
            var enumerator = list.GetEnumerator();
            int indexer = 0;
            while (enumerator.MoveNext())
            {
                if (queryCriteria(enumerator.Current))
                    return indexer;
                indexer++;
            }
            return -1;
        }

        public static string GivePrintOut<T>(this IEnumerable<T> list, System.Func<T, object> query = null)
        {
            globalStringMaker.Length = 0;
            var enumerator = list.GetEnumerator();
            int indexer = 0;
            while (enumerator.MoveNext())
            {
                if (indexer != 0)
                    globalStringMaker.Append("\n");
                globalStringMaker.Append(indexer);
                globalStringMaker.Append(": ");
                if (query == null) globalStringMaker.Append(enumerator.Current.ToString());
                else globalStringMaker.Append(query(enumerator.Current).ToString());
                indexer++;
            }
            globalStringMaker.AppendLine("\nenumerable lenght: " + indexer);
            return globalStringMaker.ToString();
        }
        public static IEnumerable<T> LogPrintOut<T>(this IEnumerable<T> list)
        {
            Debug.Log(GivePrintOut(list));
            return list;
        }
        public static string NeatCombine(this string s1, object s2)
        {
            return string.Format("{0}{1}", s1, s2);
        }

        public static T GetSingleComponent<T>(this Component comp) where T : Component
        {
#if UNITY_EDITOR
            return comp.GetComponents(typeof(T)).Single() as T;
#else
			return comp.GetComponent(typeof(T)) as T;
#endif
        }

        public static Vector3 MultiplyAxises(this Vector3 x, Vector3 multipliers)
        {
            return new Vector3(x.x * multipliers.x, x.y * multipliers.y, x.z * multipliers.z);
        }

        /// <summary>
        /// speeds up and slows down!
        /// unlikely to offshoot
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="distanceToTargetToWantedVelocityCurve"></param>
        /// <param name="timeDeltaTime"></param>
        /// <param name="speedupMul"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static Vector3 Inertiapolate(Vector3 currentPosition, Vector3 targetPosition, AnimationCurve distanceToTargetToWantedVelocityCurve, float timeDeltaTime, float speedupMul, ref float velocity)
        {
            Vector3 delta = targetPosition - currentPosition;
            velocity = Mathf.MoveTowards(velocity, distanceToTargetToWantedVelocityCurve.Evaluate(delta.magnitude), timeDeltaTime * speedupMul);
            return currentPosition + delta.normalized * velocity * timeDeltaTime;
        }

        static bool IsOverTolerance(float distance, float tolerance)
        {
            return (Mathf.Abs(distance) > tolerance);
        }

        public static Color WithTransparencyOf(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        public static string NullSafeUnityObjName(this UnityEngine.Object obj)
        {
            if (obj == null) return "NULL";
            else return obj.name;
        }

        public static string NullSafeToString(this object o)
        {
            if (o == null)
            {
                return "null";
            }
            else
            {
                string toStr = "";
                try
                {
                    toStr = o.ToString();
                }
                catch (System.Exception e)
                {
                    var err = o.GetType().ToString() + " toStr CRASH " + e;
                    Debug.LogError(e);
                    return err;
                }
                return toStr;
            }

        }

        public static string WithACapitalLetter(this string s)
        {
            if (s.Length == 0)
                Debug.LogError("string contains no chars");
            return string.Format("{0}{1}", System.Char.ToUpper(s[0]), s.Substring(1));
        }

        public static List<T> CreateList<T>(params T[] items)
        {
            var newList = new List<T>();
            newList.AddRange(items);
            return newList;
        }

        public static void AddOrSetValue<TOne, TTwo>(this Dictionary<TOne, TTwo> dictionary, TOne key, TTwo value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static TTwo GetIfHasKey<TOne, TTwo>(this Dictionary<TOne, TTwo> dictionary, TOne key)
        {

            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return default(TTwo);
        }

        public static List<T> FlattedList<T>(this T[][] array)
        {
            var rList = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    rList.Add(array[i][j]);
                }
            }
            return rList;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var get = go.GetComponent<T>();
            if (get != null)
            {
                return get;
            }
            else
            {
                return go.AddComponent<T>();
            }
        }

        public static Color SameDifferentAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }

        // use select
        //	public static IEnumerable<T> GetSubData<T, T2> (this IEnumerable<T2> theDataToBeGatheredFrom,
        //	                                                System.Func<T2, T> selection)
        //	{
        //		foreach (var item in theDataToBeGatheredFrom) {
        //			var select = selection (item);
        //			if (select != null)
        //				yield return select;
        //		}
        //	}

        static Vector3[] worldCornersArray = new Vector3[4];

        public static float GetWorldWidth(this RectTransform rectT)
        {
            rectT.GetWorldCorners(worldCornersArray);
            return worldCornersArray[2].x - worldCornersArray[0].x;
        }

        public static float GetWorldHeight(this RectTransform rectT)
        {
            rectT.GetWorldCorners(worldCornersArray);
            return worldCornersArray[2].y - worldCornersArray[0].y;
        }

        /// <summary>
        /// adds to the list if the selection criteria returns not null
        /// </summary>
        public static List<T> AddSubRange<T, T2>(this List<T> thisList, IEnumerable<T2> theDataToBeGatheredFrom,
                                                  System.Func<T2, T> selection)
        {
            foreach (var item in theDataToBeGatheredFrom)
            {
                var select = selection(item);
                if (select != null)
                    thisList.Add(select);
            }
            return thisList;
        }

        public static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list, System.Random random = null)
        {
            if (random == null) random = rng;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void DrawGizmoArrow(Vector3 a, Vector3 b)
        {
            var delta = b - a;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, b - (Quaternion.Euler(0, 45, 0) * delta.normalized));
            Gizmos.DrawLine(b, b - (Quaternion.Euler(0, -45, 0) * delta.normalized));
        }
        public static T RandomItem<T>(this IList<T> list, System.Random seededRandom = null)
        {
            if (list.Count == 0) throw new System.Exception("no items to random from");
            if (seededRandom == null) seededRandom = random;
            return list[seededRandom.Next(0, list.Count)];
        }

        static System.Random random = new System.Random();

        public static T RandomItemFromIENumerable<T>(this IEnumerable<T> ienumerable, System.Random rnd = null)
        {
            //return ienumerable.Take(Random.Range (0, ienumerable.Count()-1)).Last();
            if (rnd == null) rnd = random;
            var index = rnd.Next(0, ienumerable.Count());
            return ienumerable.ElementAtOrDefault(index);
        }

        /// <summary>
        /// Creates and returns a list where are all items are casted.
        /// <returns>The list.</returns>
        /// <param name="listToBeCasted">List to be casted.</param>
        /// <typeparam name="T1">the type to be casted from.</typeparam>
        /// <typeparam name="T2">the type to be casted to</typeparam>
        ///</summary>
        public static List<T> CastedList<T>(this IEnumerable listToBeCasted) where T : class
        {
            List<T> newList = new List<T>();
            foreach (var item in listToBeCasted)
            {
                newList.Add(item as T);
            }
            return newList;
        }

        public static void AddNewOrAddToOld<Tone>(this Dictionary<Tone, float> dictionary, Tone key, float value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static void AddNewOrAddToOld<Tone>(this Dictionary<Tone, int> dictionary, Tone key, int value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static TTwo GetValueOrDefault<TOne, TTwo>(this Dictionary<TOne, TTwo> dictionary, TOne key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return default(TTwo);
        }


        public static Vector3 DrawDebugPoint(this Vector3 point, Color? color = null, float time = 10f,
                                                   float scale = 0.5f, bool draw = true)
        {
            if (!draw)
            {
                return point;
            }

            if (color == null)
                color = new Color(1.0f, 0.0f, 1.0f);
            Debug.DrawLine(point + Vector3.left * scale, point + Vector3.right * scale, (Color)color, time);
            Debug.DrawLine(point + Vector3.up * scale, point + Vector3.down * scale, (Color)color, time);
            Debug.DrawLine(point + Vector3.forward * scale, point + Vector3.back * scale, (Color)color, time);
            return point;
        }


        public static void AddMultiple<T>(this List<T> list, params T[] toBeAdded)
        {
            list.AddRange(toBeAdded);
        }

        public static bool RandomBoolean
        {
            get { return (Random.Range(0, 2) == 1); }
        }
        public static float GetRandomSign(System.Random rng)
        {
            return (rng.Next(0, 2) == 1 ? 1f : -1f);
        }
        /// <summary>
        /// Removes last element
        /// </summary>
        public static T Pop<T>(this List<T> list)
        {
            var index = list.Count - 1;
            var removeditem = list[index];
            if (list.Count > 0)
            {
                list.RemoveAt(index);
            }
            return removeditem;
        }

        [System.Serializable]
        public class TransformData
        {
            public Vector3 worldPosition;
            public Quaternion rotation;
            public Vector3 localScale;


            public TransformData()
            {
            }

            public TransformData(Transform trans)
            {
                worldPosition = trans.position;
                rotation = trans.rotation;
                localScale = trans.localScale;
            }

            public string ToSimpleUnityJson()
            {
                return JsonUtility.ToJson(this);
            }
            public static TransformData FromSimpleJson(string str)
            {
                return JsonUtility.FromJson<TransformData>(str);
            }

            public void LerpTowards(Transform target, float amount)
            {
                target.position = Vector3.Lerp(target.position, worldPosition, amount);
                target.rotation = Quaternion.Lerp(target.rotation, rotation, amount);
                target.localScale = Vector3.Lerp(target.localScale, localScale, amount);
            }

            public static void Lerp(Transform applyTarget, TransformData a, TransformData b, float time)
            {
                applyTarget.position = Vector3.Lerp(a.worldPosition, b.worldPosition, time);
                applyTarget.rotation = Quaternion.Lerp(a.rotation, b.rotation, time);
                applyTarget.localScale = Vector3.Lerp(a.localScale, b.localScale, time);
            }

            public void Apply(Transform transform)
            {
                transform.position = worldPosition;
                transform.rotation = rotation;
                transform.localScale = localScale;
            }
        }




        //		public static T Last<T> (this List<T>list)
        //		{
        //			return list [list.Count - 1];
        //		}

        /// <summary>
        /// Instantiates, adds to possible clones list, sets parent 
        /// </summary>
        /// <returns>The cloned component in the newly made gameobject</returns>
        /// <param name="template">Template object</param>
        /// <param name="clones">Append list for new clones of the template</param>
        /// <param name="parentTrans">Used to specify the parent for the new object, if not specified will be the parent of the template</param>
        public static T DoTheStandardCloningThing<T>(this T template, List<T> clones, Transform parentTrans = null, bool controlActiveState = true)
            where T : Component
        {
            if (template == null)
                Debug.LogError("your template object is null");
            //			if (clones == null)
            //				Debug.LogError ("your clones list is null");
            //			
            var go = UnityEngine.Object.Instantiate(template.gameObject) as GameObject;

            if (controlActiveState)
            {
                go.gameObject.SetActive(true);
            }

            if (parentTrans == null)
            {
                go.transform.SetParent(template.transform.parent, false);
            }
            else
            {
                go.transform.SetParent(parentTrans, false);
            }
            var comp = go.GetComponent<T>();
            if (clones != null)
                clones.Add(comp);

            if (controlActiveState)
            {
                template.gameObject.OptimizedSetActive(false);
            }

            return comp;
        }

        public static void DestroyGameObjectsAndClear<T>(this List<T> list, bool enableInEditmode = false)
            where T : Component
        {
            foreach (var item in list)
            {
                if (item != null)
                {
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.Destroy(item.gameObject);
                    }
                    else if (enableInEditmode)
                    {
                        UnityEngine.Object.DestroyImmediate(item.gameObject);
                    }
                }
            }
            list.Clear();
        }

        public static void DestroyGameObjectsAndClear(this List<GameObject> list)
        {
            foreach (var item in list)
            {
                bool isEditorAndPaused = false;
#if UNITY_EDITOR
                isEditorAndPaused = UnityEditor.EditorApplication.isPaused;
#endif
                if (Application.isPlaying && !isEditorAndPaused)
                {
                    UnityEngine.Object.Destroy(item);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(item);
                }
            }
            list.Clear();
        }

        public static AnimationCurve CreateAnimationCurve(List<float> values)
        {
            List<Keyframe> keyFrames = new List<Keyframe>();
            for (int i = 0; i < values.Count; i++)
            {
                keyFrames.Add(new Keyframe((float)i / (float)values.Count, values[i]));
            }
            return new AnimationCurve(keyFrames.ToArray());
        }

        public static IEnumerator Fader(System.Action<float> a, float time)
        {
            float fadeTime = time;
            float remainingTime = fadeTime;
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                a(remainingTime / fadeTime);
                yield return new WaitForFixedUpdate();
            }
        }

        public static void BatchPingAndRun<T>(IEnumerable<T> targets, System.Action<T> action) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += () => TickPingRoutine(PingingRoutine(targets, action));
#endif
        }
#if UNITY_EDITOR

        static void TickPingRoutine(IEnumerator pingRoutine)
        {
            if (pingRoutine.MoveNext())
            {
                EditorApplication.delayCall += () => TickPingRoutine(pingRoutine);
            }
        }
        static IEnumerator PingingRoutine<T>(IEnumerable<T> objects, System.Action<T> action) where T : UnityEngine.Object
        {
            foreach (var item in objects)
            {
                var startStamp = EditorApplication.timeSinceStartup;
                while (EditorApplication.timeSinceStartup - startStamp < 0.1)
                {
                    yield return null;
                }
                UnityEditor.EditorGUIUtility.PingObject(item);
                action(item);
            }
        }

#endif



        public static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            md5.Dispose();
            return hashString.PadLeft(32, '0');
        }
#if UNITY_EDITOR
        public static string GetGoodNameAndPathForAsset(Object assset, string extension = ".asset", string overrideName = null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(string.Format("{0}/{1}{2}", path, overrideName != null ? overrideName : "New " + assset.GetType().Name, extension));
            return assetPathAndName;
        }
        public static bool ContainsSame<T>(this IList<T> a, IList<T> b) where T : class
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }


        public static T CreateAssetObject<T>(string overrideName = null) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            return CreateAssetObject(asset, overrideName) as T;
        }
        public static System.Diagnostics.Stopwatch StartNewStopWatch()
        {
            return System.Diagnostics.Stopwatch.StartNew();
        }

        public static Object CreateAssetObject(Object asset, string overrideName = null)
        {
            var assetPathAndName = GetGoodNameAndPathForAsset(asset, overrideName: overrideName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(asset);
            return asset;
        }
#endif

        public static string WriteToCSVFormat(Dictionary<string, List<string>> dataDic)
        {
            List<string> header = new List<string>();
            foreach (var item in dataDic)
            {
                header.Add(item.Key);
            }
            StringBuilder headersBuilder = new StringBuilder();
            StringBuilder dataBuilder = new StringBuilder();
            int dataRowsCount = dataDic[header[0]].Count;
            string[] seperators = new string[header.Count];
            for (int i = 0; i < header.Count; i++)
            {
                seperators[i] = ((i < header.Count - 1) ? ";" : System.Environment.NewLine);
                headersBuilder.Append(header[i] + seperators[i]);
            }
            for (int i = 0; i < dataRowsCount; i++)
            {
                for (int i2 = 0; i2 < header.Count; i2++)
                {
                    dataBuilder.Append(string.Format("{0}{1}", dataDic[header[i2]][i], seperators[i2]));
                }
            }
            return (headersBuilder.Append(dataBuilder).ToString());
        }


        public static T[] GetAllValuesOfEnum<T>()
        {
            return System.Enum.GetValues(typeof(T)) as T[];
        }
        public static Vector3 RandomPointInsideBounds(this Bounds bounds)
        {
            var delta = bounds.max - bounds.min;
            Vector3 returnValue = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                returnValue[i] = bounds.min[i] + (delta[i] * Random.value);
            }
            return returnValue;
        }

        public static List<string> CheckMatchBetter(IEnumerable<string> keys, string inputText, bool ignoreCase)
        {
            List<string> valid = new List<string>();
            string possiblyLowered;
            if (ignoreCase)
                inputText = inputText.ToLower();
            foreach (var key in keys)
            {
                possiblyLowered = key;
                if (ignoreCase)
                    possiblyLowered = possiblyLowered.ToLower();
                if (possiblyLowered.All(
                        x => inputText.All(
                            y => x == y)))
                {
                    valid.Add(key);
                }
            }
            return valid;
        }

        public static string CheckMatch(List<string> keys, string inputText, out MatchData.MatchResultType matchResultType,
                                         bool ignoreCase = true)
        {
            var matchData = new MatchData(
                                keys,
                                inputText,
                                ignoreCase
                            );
            var matching = CheckMatching(matchData);
            while (matching.MoveNext())
            {
            }
            matchResultType = matchData.resultData;
            return matching.Current;
        }

        public class MatchData
        {
            public enum MatchResultType
            {
                IsItThisOne,
                MultipleTakingFirst,
                NotFound,
                FilterTextHadNoCharacters
            }
            public List<string> keys;
            public string inputText;
            public MatchResultType resultData;
            public bool ignoreCase = true;

            public MatchData()
            {
            }

            public MatchData(List<string> keys, string inputText, /*string resultData, */bool ignoreCase)
            {
                this.keys = keys;
                this.inputText = inputText;
                this.ignoreCase = ignoreCase;
            }
        }

#if UNITY_EDITOR
        public static string LoadStringFromFile()
        {
            var str = System.IO.File.ReadAllText(UnityEditor.EditorUtility.OpenFilePanel("load string from file", "", ""));
            Debug.Log(str);
            return str;
        }
#endif

        public static IEnumerator<string> CheckMatching(MatchData data)
        {
            //			List<string > keys = data.keys; string inputText = data.inputText, string resultData = data, bool ignoreCase = true

            if (data.ignoreCase)
                data.inputText = data.inputText.ToLower();

            if (data.inputText.Length == 0)
            {
                data.resultData = MatchData.MatchResultType.FilterTextHadNoCharacters;
                yield return "";
                yield break;
            }
            string r = "";
            //		
            var inputCharAr = data.inputText.ToCharArray().ToList();
            List<string> tempKooditList = data.keys;
            List<int> eliminated = new List<int>();

            for (int i = 0; i < inputCharAr.Count; i++)
            {
                //for looppaa jokasen kirjaimen
                //			Debug.Log(i);
                for (int i2 = 0; i2 < tempKooditList.Count; i2++)
                {
                    //for looppaa jokaisen sanan johon se vertaa
                    if (!eliminated.Contains(i2))
                    {
                        //jos se on eliminoitu skipataan
                        string koodi;
                        if (data.ignoreCase)
                            koodi = tempKooditList[i2].ToLower();
                        else
                            koodi = tempKooditList[i2];

                        //					Debug.Log(koodi.Length+","+i);
                        if (koodi.Length >= i)
                        {
                            if (koodi[i] == inputCharAr[i])
                            {
                                //							Debug.Log("osu")
                                //jos vertaus onnistuu niin saa ell‰‰
                            }
                            else
                            {
                                //							Debug.Log("ei osu");
                                eliminated.Add(i2); //muuten se eliminoidaan
                            }
                        }
                        else
                        {
                            eliminated.Add(i2);
                        }
                    }
                    yield return "";
                }
            }
            var temptempList = tempKooditList.ToList();

            for (int i = 0; i < eliminated.Count; i++)
            {
                //			Debug.Log(temptempList[5]);
                //			Debug.Log(temptempList[i]+","+i);
                tempKooditList.Remove(temptempList[eliminated[i]]);
                yield return "";
            }
            if (tempKooditList.Count > 1)
            {
                data.resultData = MatchData.MatchResultType.MultipleTakingFirst;
                r = tempKooditList.OrderBy(x => Mathf.Abs(x.Length - data.inputText.Length)).First();
            }
            else if (tempKooditList.Count > 0)
            {
                r = tempKooditList[0];
                data.resultData = MatchData.MatchResultType.IsItThisOne;
            }
            else
            {
                data.resultData = MatchData.MatchResultType.NotFound;
            }
            yield return r;
        }

#if UNITY_EDITOR
        [MenuItem("Time/Toggle timescale up _%T")]
#endif
        public static void ToggleTimeScaleUp()
        {
            if (GetIsEditorMode())
                return;

            if (Time.timeScale != 5f)
            {
                Time.timeScale = 5f;
                Time.fixedDeltaTime = 0.05f; //20 fixeds per sec
                /*CopyPastedMaxRandomUtils.FindObjectsOfTypePerfLogged<RivalGames.ActorController>().ForEach (x => x.actorDriver.animator.updateMode = AnimatorUpdateMode.Normal);
				CopyPastedMaxRandomUtils.FindObjectsOfTypePerfLogged<RivalGames.ActorController>().ForEach (x => x.actorDriver.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate);*/
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f; //50 fixeds per sec
                                             //MaxinRandomUtils.FindComponentsOfType<ActorController>().ForEach (x => x.actorDriver.animator.updateMode = AnimatorUpdateMode.AnimatePhysics);
            }
            Debug.Log("timescale: " + Time.timeScale);
        }

#if UNITY_EDITOR
        [MenuItem("Time/Toggle timescale down _#%T")]
#endif
        public static void ToggleTimeScaleDown()
        {
            if (GetIsEditorMode())
                return;

            if (Time.timeScale != 0.35f)
                Time.timeScale = 0.35f;
            else
                Time.timeScale = 1f;
            Debug.Log("timescale: " + Time.timeScale);
        }



        public static IEnumerator WaitForTime(this float time)
        {
            float startTime = Time.time;
            while (Time.time - startTime < time)
            {
                yield return null;
            }
        }

        public static void LogFullStackTraceInBuild(string reason, bool logNormallyInEditor = false)
        {
#if !UNITY_XBOXONE
            if (!Application.isEditor)
            {
                var trace = new System.Diagnostics.StackTrace();
                Debug.Log(string.Format("{0} logstacktraceinbuild: {1}", reason, trace.ToStringLong()));
            }
            else if (logNormallyInEditor)
            {
                Debug.Log(reason);
            }
#endif
        }

        public static string ToStringLong(this System.Diagnostics.StackTrace trace)
        {
            return trace.GetFrames().Aggregate("", (x, y) =>
            {
                var method = y.GetMethod();
                var methodReflectedType = method.ReflectedType;
                return x + "\t ||| \t." + methodReflectedType + "." + method.Name;
            });
        }



        static bool GetIsEditorMode()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return true;
#endif
            return false;
        }

#if UNITY_EDITOR
        [MenuItem("Debug/Reload Scene _#%E")]
        public static void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        [MenuItem("Debug/Log Selection")]
        public static void LogSelection()
        {
            Debug.Log(UnityEditor.Selection.objects.Select(x =>
            {
                var assetPath = AssetDatabase.GetAssetPath(x);
                if (x is GameObject go && string.IsNullOrEmpty(assetPath))
                {
                    var list = go.transform.GetRecurseWhileNotNull(y => y.transform.parent);
                    list.Reverse();
                    return list.Aggregate("", (y, z) => y + "." + z.name);
                }
                else
                {
                    return Application.dataPath.Replace("Assets", assetPath);
                }
            }).GivePrintOut());
        }

        [MenuItem("Debug/CreateNewGameObjectParented _#%&N")]
        public static void CreateNewGameObjectParented()
        {
            var created = new GameObject("New gameobject");
            if (Selection.activeObject != null)
                created.transform.SetParent(Selection.activeGameObject.transform, false);
            Selection.activeObject = created;
        }


        [MenuItem("Time/Pause player _%E")]
        public static void TogglePlayerPause()
        {
            UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
        }

        [MenuItem("Tools/Clear Console _%G")]
        public static void ClearConsole()
        {

            Debug.Log("Now CLEARINGLOG_KILINKOLIN_RANDOMSPECIFICSTRING_YEY"); //for logmonitor, won't be visible in unity obviously
                                                                              // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear",
                                  System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        public static bool IsPropertyModificationIgnorable(UnityEditor.PropertyModification mod, bool allowDifferentName = true, bool allowSomeExtras = false)
        {

            switch (mod.propertyPath)
            {
                //we don't care about position related changes made to the root object
                case "m_AnchorMax.x":
                case "m_AnchorMax.y":
                case "m_AnchorMin.x":
                case "m_AnchorMin.y":
                case "m_LocalPosition.x":
                case "m_LocalPosition.y":
                case "m_LocalPosition.z":
                case "m_LocalRotation.x":
                case "m_LocalRotation.y":
                case "m_LocalRotation.z":
                case "m_LocalRotation.w":
                case "m_LocalScale.x":
                case "m_LocalScale.y":
                case "m_LocalScale.z":
                case "m_RootOrder":
                case "m_AnchoredPosition.x":
                case "m_AnchoredPosition.y":
                case "m_LocalEulerAnglesHint.x":
                case "m_LocalEulerAnglesHint.y":
                case "m_LocalEulerAnglesHint.z":
                case "m_havePropertiesChanged":
                case "m_isInputParsingRequired":
                    return true;
            }

            //			case "m_Name":
            //				if(!allowDifferentName)return false;
            //					
            //				//these also have been reported as being different even right after the Revert Prefab button was just pressed
            //			case "m_Enabled":
            //			case "m_OverrideSorting":
            //				if (!allowSomeExtras) return false;
            //				else return true;
            //			default:
            //				return false;

            if (mod.propertyPath == "m_Name")
            {
                if (!allowDifferentName) return false;
                else return true;
            }

            if (allowSomeExtras)
            {
                if (mod.propertyPath == "m_Enabled" || mod.propertyPath == "m_OverrideSorting") return true;
            }

            return false;
        }
#endif

    }



    [System.Serializable]
    public class PairValues<Tone, Ttwo>
    {
        public Tone key;
        public Ttwo value;

        public PairValues(Tone key, Ttwo value)
        {
            this.key = key;
            this.value = value;
        }
    }


    public static class DebugDraw
    {

        public static GameObject CylinderDir(Vector3 startPosition, Vector3 direction, Color color, string name,
                                           float time = 0.1f, float width = 0.1f, GameObject moveOld = null, bool removeCollider = true, bool debugLog = false)
        {
            return Cylinder(startPosition, startPosition + direction, color, name, time, width, moveOld, removeCollider, debugLog);
        }
        public static GameObject Cylinder(Vector3 startPosition, Vector3 endPosition, Color color, string name,
                                           float time = 0.1f, float width = 0.1f, GameObject moveOld = null, bool removeCollider = true, bool debugLog = false, bool fade = false)
        {
            GameObject go;

            if (moveOld == null)
                go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            else
                go = moveOld;

            go.name = "cylinder";
            go.transform.localScale = Vector3.one;
            var currentScale = go.transform.localScale * width;
            currentScale.y = Vector3.Distance(startPosition, endPosition) / 2f;
            go.transform.localScale = currentScale;
            go.transform.position = Vector3.Lerp(startPosition, endPosition, 0.5f);

            go.transform.LookAt(endPosition);
            go.transform.Rotate(90f, 0, 0, Space.Self);


            AddShitToPrimitive(startPosition, color, name, time, removeCollider, go, endPosition, debugLog: debugLog, fade: fade);

            return go;
        }
        [System.Flags]
        public enum SillinessMode
        {
            None = 1,
            RandomRotation = 2,
            RandomColor = 4
        }
        public static GameObject Cube(Vector3 position, float size, Color color, string name, float time = 0f,
                                       GameObject moveOld = null, bool removeCollider = true, SillinessMode sillinessMode = SillinessMode.None, bool debuglog = false, bool fade = false)
        {
            return CreatePrimitive(position, size, color, name, time, moveOld, removeCollider, PrimitiveType.Cube, sillinessMode, debuglog, fade: fade);
        }

        public static GameObject Sphere(Vector3 position, float size, Color color, string name, float time = 0f,
                                         GameObject moveOld = null, bool removeCollider = true, SillinessMode sillinessMode = SillinessMode.None, bool fade = false)
        {
            return CreatePrimitive(position, size, color, name, time, moveOld, removeCollider, PrimitiveType.Sphere, sillinessMode, fade: fade);
        }

        public static void DrawBounds(Bounds bounds, Color color, string name,
            float time = 0.1f, float lineWidth = 0.1f, bool removeCollider = true, bool debugLog = false, bool fade = false)
        {
            float z = bounds.extents.z;
            float x = bounds.extents.x;
            float y = bounds.extents.y;

            {
                // positive Z
                Cylinder(bounds.center + new Vector3(-x, -y, z), bounds.center + new Vector3(-x, y, z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(x, -y, z), bounds.center + new Vector3(x, y, z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, y, z), bounds.center + new Vector3(x, y, z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, -y, z), bounds.center + new Vector3(x, -y, z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
            }
            {
                // negative Z
                Cylinder(bounds.center + new Vector3(-x, -y, -z), bounds.center + new Vector3(-x, y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(x, -y, -z), bounds.center + new Vector3(x, y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, y, -z), bounds.center + new Vector3(x, y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, -y, -z), bounds.center + new Vector3(x, -y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
            }
            {
                // z travel
                Cylinder(bounds.center + new Vector3(x, y, z), bounds.center + new Vector3(x, y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, y, z), bounds.center + new Vector3(-x, y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(x, -y, z), bounds.center + new Vector3(x, -y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
                Cylinder(bounds.center + new Vector3(-x, -y, z), bounds.center + new Vector3(-x, -y, -z), color, name, time, lineWidth, null, removeCollider, debugLog, fade);
            }
        }

        static GameObject CreatePrimitive(Vector3 position, float size, Color color, string name, float time,
                                           GameObject moveOld, bool removeCollider, PrimitiveType primitiveType, SillinessMode sillinessMode = SillinessMode.None, bool debugLog = false, bool fade = false)
        {
            UnityEngine.Profiling.Profiler.BeginSample("debugdraw");
            GameObject go;
            if (moveOld != null)
                go = moveOld;
            else
                go = GameObject.CreatePrimitive(primitiveType);
            go.transform.position = position;
            go.transform.localScale = Vector3.one * size;
            AddShitToPrimitive(position, color, name, time, removeCollider, go, sillinessMode: sillinessMode, debugLog: debugLog, fade: fade);

            go.transform.SetParent(TrashParent, true);
            UnityEngine.Profiling.Profiler.EndSample();
            return go;
        }

        public static Transform TrashParent
        {
            get
            {
                if (m_trashParent == null)
                {
                    m_trashParent = new GameObject("TrashParent_TimeWraprandom").transform;
                    // m_trashParent.gameObject.AddComponent<DestroyerOfTheImmortals>();
                }
                return m_trashParent;
            }
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit raycast, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, string name = null, float drawForTime = .1f, float width = .1f, bool actuallyDraw = true, bool drawOnlyWhenFailed = false)
        {
            bool hit = Physics.Raycast(origin, direction, out raycast, distance, layerMask, queryTriggerInteraction);
            if (actuallyDraw && (!drawOnlyWhenFailed || !hit)) Cylinder(origin, origin + (direction * distance), hit ? Color.green : Color.red, name, drawForTime, width);
            return hit;
        }
        public static List<T> GetComponentsOnlyFromChildren<T>(this GameObject go, bool includeInactive)
        {
            var vals = new List<T>();
            go.GetComponentsInChildren<T>(includeInactive, vals);
            var comps = go.GetComponents<T>();
            vals.RemoveAll(x => comps.Contains(x));
            return vals;
        }
        public static List<Transform> GetChilds(this Transform trans)
        {
            var list = new List<Transform>();
            for (int i = 0; i < trans.childCount; i++)
            {
                list.Add(trans.GetChild(i));
            }
            return list;
        }


        private static Transform m_trashParent;
        static Shader unlitColorShader;
        static bool triedToFindUnlitColorShader = false;
        static Shader transparentShader;
        static FindingStateType hderpTransparentShaderFindingState = FindingStateType.NotSearched;
        public enum FindingStateType
        {
            NotSearched,
            SearchedNotFound,
            SearchedFound
        }
        static void AddShitToPrimitive(Vector3 startPosition, Color color, string name, float time, bool removeCollider,
                            GameObject go, Vector3? endPosition = null, SillinessMode sillinessMode = SillinessMode.None, bool debugLog = false, bool fade = false)
        {

            PactRandomUtils.RandomUtils.LogFullStackTraceInBuild("debugdraw in build");
            var collider = go.GetComponent<Collider>();
            if (collider != null && removeCollider)
            {
                collider.enabled = false;
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(collider);
                }
                else
                {
                    Object.Destroy(collider);
                }
            }
            go.name = string.Format("{0} start:{1} end:{2}", name, startPosition, endPosition);
            if (debugLog) Debug.Log(go.name, go);
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.material.color = color;
            if (fade)
            {

                if (hderpTransparentShaderFindingState == FindingStateType.NotSearched)
                {
                    if (transparentShader = Shader.Find("HDRP/Unlit"))
                    {
                        hderpTransparentShaderFindingState = FindingStateType.SearchedFound;
                    }
                    else
                    {
                        hderpTransparentShaderFindingState = FindingStateType.SearchedNotFound;
                    }
                    //	if (transparentShader == null) Debug.LogError("no transparent shader");
                }
                if (hderpTransparentShaderFindingState == FindingStateType.SearchedFound)
                {
                    // set surface type to transparent
                    meshRenderer.material = Resources.Load("transparentHDRPMaterial") as Material;
                }
                else
                {
                    var mat = meshRenderer.material;
                    mat.SetColor("_Color", color);
                    // props 
                    // https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.DisableKeyword("_ALPHABLEND_ON");
                    mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                    meshRenderer.material = mat;
                }
            }
            else
            {
                if (!triedToFindUnlitColorShader)
                {
                    unlitColorShader = Shader.Find("Unlit/Color");
                    triedToFindUnlitColorShader = true;
                }
                if (unlitColorShader != null)
                {
                    meshRenderer.material.shader = unlitColorShader;
                }
            }

            if ((sillinessMode & SillinessMode.RandomColor) != 0)
            {
                go.GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
            }
            if ((sillinessMode & SillinessMode.RandomRotation) != 0)
            {
                go.transform.localRotation = Quaternion.LookRotation(Random.insideUnitSphere);
            }
            float createdStamp = Time.time;
            if (time > 0)
            {
                var invokeData = new InvokeData();
                invokeData.action = () => { };
                invokeData.repeatAction = () =>
                {
                    if (fade)
                    {
                        var sameDifferentAlpha = color.SameDifferentAlpha(1f - ((Time.time - createdStamp) / time));
                        //meshRenderer.material.color = color.SameDifferentAlpha(1f - ( (Time.time - createdStamp ) / time));
                        if (hderpTransparentShaderFindingState == FindingStateType.SearchedFound)
                        {
                            meshRenderer.material.SetColor("_UnlitColor", sameDifferentAlpha);
                        }
                        else
                        {
                            meshRenderer.material.SetColor("_Color", sameDifferentAlpha);
                        }
                    }
                };

                invokeData.duration = time;
                go.GetOrAddComponent<ActionTaskHandler>().Invoke(invokeData);
            }
            else if (time == 0f)
            {
                go.GetOrAddComponent<ActionTaskHandler>().StartSomeCoroutine(DestroyAfterTwoFrames(go));
            }

            //go.GetOrAddComponent<ActionTaskHandler>().unResolvedCallStack = new System.Diagnostics.StackTrace(true);

            //DebugVisualisationRecorder.TryRegisterVisualisation(go);
        }

        static IEnumerator DestroyAfterTwoFrames(GameObject go)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            Object.Destroy(go);
        }
    }




}


public struct DebugSphere
{
    public string name;
    public float size;
    public Vector3 position;

    public DebugSphere(string name, float size, Vector3 position)
    {
        this.name = name;
        this.size = size;
        this.position = position;
    }
}

[System.Serializable]
public struct DebugVisualisationData
{
    public string state;

    public DebugSphere[] debugSpheres;

    public Color? color;
    public bool? assertColor;
    public Vector3[] line;
    public Bounds[] boxes;


    public DebugVisualisationData(string state = null, DebugSphere[] debugSpheres = null, Color? color = null,
                              bool? assertColor = null,
                              Vector3[] line = null, Bounds[] boxes = null)
    {
        this.state = state;
        this.debugSpheres = debugSpheres;
        this.color = color;
        this.assertColor = assertColor;
        this.line = line;
        this.boxes = boxes;
    }

    public void Visualise(float waitTime)
    {
        if (color == null)
        {
            color = Color.white;
        }
        if (assertColor != null)
        {
            color = assertColor.Value ? Color.green : Color.red;
        }

        if (line != null)
        {
            for (int i = 0; i < line.Length - 1; i++)
            {
                Debug.DrawLine(line[i], line[i + 1], color.Value, waitTime);
            }
        }

        if (!string.IsNullOrEmpty(state))
        {
            Debug.Log(state);
        }
        if (debugSpheres != null)
        {
            for (int i = 0; i < debugSpheres.Length; i++)
            {
                DebugDraw.Sphere(debugSpheres[i].position, debugSpheres[i].size, color.Value, debugSpheres[i].name,
                    waitTime);
            }
        }
        if (boxes != null)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].DrawDebugLineBox(waitTime, color.Value);
            }
        }
    }
}
[System.Serializable]
public class BangBangInertiaMover
{
    public float currentValue;
    public float acceleration = 1;
    public float maxSpeed = 4;
    public float speed;
    public float deaccelMul = 8;
    // dir, stamp
    //KeyValuePair<float, float> lastDirChangeStamp = new KeyValuePair<float, float>(-1, -1);

    public void Update(float targetValue)
    {
        // this would cause thing to never deaccelerate which often most likely not wanted if time scale is 0
        if (Time.timeScale == 0)
        {
            return;
        }
        var delta = targetValue - currentValue;
        float dir = Mathf.Sign(delta);
        float distance = Mathf.Abs(delta);
        float wantedSpeedMul = Mathf.Clamp01(distance / (maxSpeed * deaccelMul));
        float targetSpeed = maxSpeed * wantedSpeedMul * dir;
        speed = Mathf.MoveTowards(speed, targetSpeed, acceleration * Time.deltaTime);
        currentValue += speed;
    }

}

public static class TImeWarpUnityJsonUtility
{
    public static void FromJsonOverwriteValueFields<T>(string json, T target)
    {
        var nonvalueTypes = target.GetType().GetFields().Where(x => !x.FieldType.IsValueType).Select(x => x.Name).ToList();
        List<string> jsonLines = json.Split('\n').ToList();
        //jsonLines.RemoveAll(x => nonvalueTypes.Contains(x));
        for (int i = jsonLines.Count - 1; i >= 0; i--)
        {
            for (int j = nonvalueTypes.Count - 1; j >= 0; j--)
            {
                if (jsonLines[i].Contains(nonvalueTypes[j]))
                {
                    jsonLines.RemoveAt(i);
                    break;
                }
            }
        }
        JsonUtility.FromJsonOverwrite(jsonLines.Aggregate("", (x, y) => x + "\n" + y), target);
    }
}

public abstract class WaitingIEnumeratorBase : IEnumerator
{
    #region IEnumerator implementation

    bool notFinished = true;
    public float lastTime;
    public float waitTime;

    float time;

    public bool MoveNext()
    {
        if (!notFinished)
            return false;
        time = Time.time;
        if (time - lastTime > waitTime)
        {
            lastTime = time;
            return notFinished = SecretMoveNext();
        }
        else
        {
            return true;
        }
    }

    protected abstract bool SecretMoveNext();

    public abstract void Reset();

    public abstract object Current { get; }

    #endregion
}

public class WaitingIEnumerator : WaitingIEnumeratorBase
{
    public IEnumerator ienum;

    public WaitingIEnumerator(IEnumerator ienum, float waitTime)
    {
        this.waitTime = waitTime;
        this.ienum = ienum;
    }

    #region implemented abstract members of WaitingIEnumeratorBase

    protected override bool SecretMoveNext()
    {
        return ienum.MoveNext();
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }

    public override object Current
    {
        get { throw new System.NotImplementedException(); }
    }

    #endregion
}

public class VisualisingIenumerator : WaitingIEnumeratorBase
{
    public IEnumerator<DebugVisualisationData> ienum;

    public VisualisingIenumerator(IEnumerator<DebugVisualisationData> ienum, float waitTime)
    {
        this.waitTime = waitTime;
        this.ienum = ienum;
    }

    #region implemented abstract members of WaitingIEnumeratorBase

    protected override bool SecretMoveNext()
    {
        var val = ienum.MoveNext();
        ienum.Current.Visualise(waitTime);
        return val;
    }

    public override void Reset()
    {
        throw new System.NotImplementedException();
    }

    public override object Current
    {
        get { throw new System.NotImplementedException(); }
    }

    #endregion

}