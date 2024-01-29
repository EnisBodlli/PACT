using System.Collections;
using System.Collections.Generic;
using Pact.WindowManager.ScriptableObjects.ScriptableEnums;
using Pact.WindowManager.Windows;
using UnityEngine;

namespace Pact.WindowManager.Windows
{
    public class CompositeWindow : WindowBase
    {
        [SerializeField] private List<WindowTypeEnumSO> childWindows;

        public override void Open(Transform parent)
        {
            base.Open(parent);
            foreach (var childWindow in childWindows)
            {
                WindowManager.Instance.OpenWindow(childWindow.windowType, parent);
            }
        }

        public override void Close()
        {
            if (!CanClose()) return;

            base.Close();
            foreach (var childWindow in childWindows)
            {
                WindowManager.Instance.CloseWindow(childWindow);
            }
        }
    }

}
