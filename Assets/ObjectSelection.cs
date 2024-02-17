using Pact.Events.Registry;
using Pact.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    [SerializeField]
    public int selectedIndex = 0;

    void Start()
    {
        UpdateCharacterSelection();

        GlobalEventManager.Instance.Listen<ObjectSelectionReference>(this, OnRequestGameManagerEVTEvent);
    }
    public void OnRequestGameManagerEVTEvent(ObjectSelectionReference objectSelection)
    {
        objectSelection.ResponseAction?.Invoke(this);
    }


    private void OnDisable()
    {
        if (GlobalEventManager.Instance != null)
        {
            GlobalEventManager.Instance.RemoveAllListenersFor(this);
        }

    }
    public void OnLeftButton()
    {
        // Decrement selectedIndex and wrap around if it goes below 0
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = transform.childCount - 1;
        }
        UpdateCharacterSelection();
    }

    public void OnRightButton()
    {
        // Increment selectedIndex and wrap around if it exceeds the number of children
        selectedIndex++;
        if (selectedIndex >= transform.childCount)
        {
            selectedIndex = 0;
        }
        UpdateCharacterSelection();
    }

    private void UpdateCharacterSelection()
    {
        // Loop through all children and activate only the selected one
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            child.SetActive(i == selectedIndex);
        }
    }
}
