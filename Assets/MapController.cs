using UnityEngine;
using Microsoft.Maps.Unity;
using Microsoft.Geospatial; // Ensure this namespace is correct for your version of the Bing Maps SDK

public class MapController : MonoBehaviour
{
    public MapRenderer mapRenderer; // Reference to the MapRenderer component

    // Method to place a waypoint on the map
    public void PlaceWaypointOnMap(Vector2 geoCoordinates)
    {
        // Create a new MapPin at the specified geographic coordinates
        MapPin waypoint = new MapPin
        {
            Location = new LatLon(geoCoordinates.x, geoCoordinates.y) // Set the latitude and longitude
        };

        // Optionally set other properties of the MapPin here, such as title, label, or appearance

        // Add the MapPin to the map
       
        // If you need the map to zoom or pan to the newly added pin, you can adjust the map's center and zoom level here
        // For example:
        // mapRenderer.SetMapScene(new MapSceneOfLocationAndZoomLevel(waypoint.Location, 15));
    }
}
