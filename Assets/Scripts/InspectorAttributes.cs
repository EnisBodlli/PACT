using UnityEngine;
using System.Collections;

namespace PactRandomUtils
{
    public class InspectorButton : System.Attribute
    {
        public float spaceBefore = 0f;

        public InspectorButton(float spaceBefore)
        {
            this.spaceBefore = spaceBefore;
        }

        public InspectorButton()
        {
        }
    }


    public class HideFromInspectorIf : PropertyAttribute
    {
        public string hidingFieldName;
        public HideFromInspectorIf(string hidingFieldName)
        {
            this.hidingFieldName = hidingFieldName;
        }

    }


    public class DrawnNonSerialized : System.Attribute
    {
    }
}