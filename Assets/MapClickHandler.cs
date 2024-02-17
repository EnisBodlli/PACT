using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Microsoft.Maps.Unity;

public class MapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera topDownCamera; // Assign your top-down camera in the inspector
    public GameObject prefab3D; // Assign your 3D prefab in the inspector
    public GameObject prefab2D; // Assign your 2D UI prefab in the inspector
    public RectTransform uiImageRectTransform; // Assign the RectTransform of the UI Image that displays the render texture
    public PathFollower pathFollower;
    private List<Vector3> waypoints = new List<Vector3>();
    private void Awake()
    {
        topDownCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();
        uiImageRectTransform = this.GetComponent<RectTransform>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Convert the click position to be relative to the UI Image
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(uiImageRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // Normalize the local point to a value between 0 and 1
            Vector2 normalizedPoint = new Vector2(
                (localPoint.x + uiImageRectTransform.rect.width * 0.5f) / uiImageRectTransform.rect.width,
                (localPoint.y + uiImageRectTransform.rect.height * 0.5f) / uiImageRectTransform.rect.height);

            // Convert this normalized point to a viewport point for the camera
            Ray ray = topDownCamera.ViewportPointToRay(new Vector3(normalizedPoint.x, normalizedPoint.y, 0));

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Instantiate the 3D object at the hit point in the world
                GameObject waypoint = Instantiate(prefab3D, hitInfo.point, Quaternion.identity);
                waypoints.Add(waypoint.transform.position);
            }

            // Instantiate the 2D UI object at the clicked position within the UI Image
            Instantiate2DUIPrefabAtClickPosition(localPoint);
        }
    }
    public void CreateAndFollowPath()
    {
        pathFollower.SetPath(waypoints, true);
        waypoints.Clear();
    }
    private void Instantiate2DUIPrefabAtClickPosition(Vector2 localPoint)
    {
        // Instantiate the 2D UI prefab as a child of the UI Image (or another appropriate parent within the UI)
        GameObject instantiated2DObject = Instantiate(prefab2D, uiImageRectTransform);

        // Set the local position of the instantiated 2D object to the click position
        // The prefab's RectTransform anchor should be set to the bottom left (0,0) if you want it to be positioned exactly at the click point
        RectTransform instantiated2DObjectRectTransform = instantiated2DObject.GetComponent<RectTransform>();
        instantiated2DObjectRectTransform.anchoredPosition = localPoint;
    }
}
