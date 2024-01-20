using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PactRandomUtils;
using TimeWarp.Events;
using Pact.Events.Registry;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalEventManager.Instance.Trigger<Test1>(
            new Test1()
            { Enis = "enis" }
        );
    }
    private void OnEnable()
    {
        GlobalEventManager.Instance.Listen<Test1>(this, Test2);
    }

    private void Test2(Test1 evt)
    {
        Debug.Log(evt.Enis);
    }
    // Update is called once per frame
    private void OnDisable()
    {
        GlobalEventManager.Instance.RemoveAllListenersFor(this);
    }
}
