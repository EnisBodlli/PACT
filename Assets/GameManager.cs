using Microsoft.Maps.Unity;
using Pact.Events.Registry;
using Pact.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PactRandomUtils;

public class GameManager : MonoBehaviour
{
    public List<PathFollower> objectsCreated = new List<PathFollower>();
    GameObject mapRenderer;


    private void Start()
    {
        mapRenderer = GameObject.FindGameObjectWithTag("MapRenderer");
        ListenEvents();
    }

    public void CreateObjects3d(ObjectSelection objectSelection,string Name, string RCS,bool C2, bool Gps, bool Noise)
    {
       PathFollower pathFollower = Instantiate(objectSelection.transform.GetChild(objectSelection.selectedIndex)).GetComponent<PathFollower>();

        pathFollower.Name = Name;
        pathFollower.name = Name;
        pathFollower.RCS = RCS;
        pathFollower.Noise = Noise;
        pathFollower.C2 = C2;
        pathFollower.Gps = Gps;
        pathFollower.Noise = Noise;
        pathFollower.transform.SetParent(mapRenderer.transform);
        pathFollower.transform.position = Vector3.zero;
        objectsCreated.Add(pathFollower);
    }

    public void RemoveListeners()
    {
        if (GlobalEventManager.Instance != null)
        {
            GlobalEventManager.Instance.RemoveAllListenersFor(this);
        }
    }
    private void OnDisable()
    {
        RemoveListeners();
    }

    public void ListenEvents()
    {

        GlobalEventManager.Instance.Listen<GameManagerEVT>(this, OnRequestGameManagerEVTEvent);
    }


    private void OnRequestGameManagerEVTEvent(GameManagerEVT evt)
    {
        evt.ResponseAction?.Invoke(this);
    }
}
