using Microsoft.Maps.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pact.Events.Registry
{
 
    public struct Test1
    {
        public string Enis;
    }
    public struct ObjectSelectionReference
    {
        public Action<ObjectSelection> ResponseAction;
       
    }
    public struct GameManagerEVT
    {
        public Action<GameManager> ResponseAction;

    }
}
