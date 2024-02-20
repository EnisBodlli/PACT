using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Pact.WindowManager.Enum;
using Pact.WindowManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathSystemController : MonoBehaviour
{
    public MapRenderer mapRenderer;
    public TMP_InputField lat;
    public TMP_InputField lon;
    public Slider zoomLevel;


    private void Awake()
    {
        mapRenderer = GameObject.FindGameObjectWithTag("MapRenderer").GetComponent<MapRenderer>();
        Debug.Log(mapRenderer.ComputeUnityToMapScaleRatio());

        zoomLevel.onValueChanged.AddListener(UpdateTextValue);
    }


    // Callback function to update the text when the slider value changes
    private void UpdateTextValue(float value)
    {
        mapRenderer.ZoomLevel = zoomLevel.value;
    }
    public void OnBackBtnClicked()

    {
        WindowManager.Instance.CloseAllWindows();
        WindowManager.Instance.OpenWindow(WindowType.MainMenu);
    }

    public void OnSearchBtnClicked()
    {
        LatLon latLon = new LatLon(float.Parse(lat.text), float.Parse(lon.text));

        mapRenderer.Center = latLon;
        mapRenderer.ZoomLevel = zoomLevel.value;
    }
}
