using Pact.WindowManager.Enum;
using Pact.WindowManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial;
using UnityEngine.UI;
using TMPro;
public class RadarViewManager : MonoBehaviour
{
    public MapRenderer mapRenderer;
    public TMP_InputField lat;
    public TMP_InputField lon;
    public Slider zoomLevel;
    
    public TextMeshProUGUI text;
    private void Awake()
    {
        mapRenderer = GameObject.FindGameObjectWithTag("MapRenderer").GetComponent<MapRenderer>();
        zoomLevel.onValueChanged.AddListener(UpdateTextValue);
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
        WindowManager.Instance.OpenWindow(WindowType.MainMenu);
    }
    public void OnSystemClicked()
    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.SystemUI);
    }

    public void OnSearchBtnClicked()
    {
        LatLon latLon = new LatLon(float.Parse(lat.text), float.Parse(lon.text));
        
       mapRenderer.Center= latLon;
        mapRenderer.ZoomLevel = zoomLevel.value;
    }
}
