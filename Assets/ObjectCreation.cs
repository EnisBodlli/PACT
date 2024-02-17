using Pact.Events;
using Pact.Events.Registry;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCreation : MonoBehaviour
{
    public TMP_InputField Name;
    public TMP_InputField RCS;
    public Toggle C2;
    public Toggle Gps;
    public Toggle Noise;
    public Transform Parent;

    public ObjectTemplate objectToSpawn;
    
    ObjectSelection _objSelection;
 

    public List<ObjectTemplate> spawnedObjects = new List<ObjectTemplate>();
    public void SetData()
    {
        ObjectTemplate objectTemplate = Instantiate(objectToSpawn);
        objectTemplate.transform.parent = Parent;
        objectTemplate.Name.text= Name.text;
        objectTemplate.RCS.text = RCS.text;
        objectTemplate.C2.isOn = C2.isOn;
        objectTemplate.Gps.isOn = Gps.isOn;
        objectTemplate.Noise.isOn = Noise.isOn;
        spawnedObjects.Add(objectTemplate);
        GameManager.CreateObjects3d(ObjectSelection, Name.text, RCS.text, C2.isOn, Gps.isOn, Noise.isOn);
    }
    GameManager _gameManager;

    private void OnDisable()
    {
        RemoveListeners();
    }
    public void RemoveListeners()
    {
        if (GlobalEventManager.Instance != null)
        {
            GlobalEventManager.Instance.RemoveAllListenersFor(this);
        }
    }

   
    public GameManager GameManager
    {
        get
        {
            if (_gameManager == null)
            {
                GlobalEventManager.Instance.Trigger(new GameManagerEVT()
                {
                    ResponseAction = combatController => _gameManager = combatController
                });
            }
            return _gameManager;
        }
    }
  
    public ObjectSelection ObjectSelection
    {
        get
        {
            if (_objSelection == null)
            {
                GlobalEventManager.Instance.Trigger(new ObjectSelectionReference()
                {
                    ResponseAction = objSelection => _objSelection = objSelection
                });
            }
            return _objSelection;
        }
    }
    public void OnRightArrowClicked()
    {
        ObjectSelection.OnRightButton();
    }

    public void OnLeftArrowClicked() 
    {
        ObjectSelection.OnLeftButton();
    }
}
