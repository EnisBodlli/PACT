using Pact.WindowManager.Enum;
using UnityEngine;
namespace Pact.WindowManager.ScriptableObjects.ScriptableEnums
{

    [CreateAssetMenu(
         fileName = "WindowTypeScriptableEnum",
         menuName = "ScriptableEnum" + "/Window Type Scriptable Enum"
    )]

    public class WindowTypeEnumSO : ScriptableObject
    {
        public WindowType windowType;
    }
}
