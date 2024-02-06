using Pact.WindowManager;
using Pact.WindowManager.Enum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SystemUIController : MonoBehaviour
{
    public float range = 0;
    public Slider slider;
    public TextMeshProUGUI text;

    private void Start()
    {
        // Subscribe to the OnValueChanged event of the slider
        slider.onValueChanged.AddListener(UpdateTextValue);
    }

    // Callback function to update the text when the slider value changes
    private void UpdateTextValue(float value)
    {
        // Convert the float value to a string and update the text component


        text.text = "Radius(KM):" + value.ToString();
    }
    public void OnBackBtnClicked()
    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.RadarScreen);
    }

}
