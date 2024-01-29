using Pact.WindowManager.Enum;
using Pact.WindowManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        WindowManager.Instance.OpenWindow(WindowType.MainMenu);
    }
}
