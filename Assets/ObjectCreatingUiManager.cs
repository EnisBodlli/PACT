using Pact.WindowManager.Enum;
using Pact.WindowManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreatingUiManager : MonoBehaviour
{
    public void OnBackBtnClicked()
    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.MainMenu);
    }
}
