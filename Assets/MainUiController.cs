using Pact.WindowManager;
using Pact.WindowManager.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUiController : MonoBehaviour
{
   public void OnRadarScreenClicked()
    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.RadarScreen);
    }
    public void OnObjCreatingBrnClicked()
    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.ObjectCreating);
    }
}
