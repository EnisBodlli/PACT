using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTemplate : MonoBehaviour
{

    public TextMeshProUGUI Name;
    public TextMeshProUGUI RCS;
    public Toggle C2;
    public Toggle Gps;
    public Toggle Noise;


    public void Deletebtn()
    {
        Destroy(this.gameObject); 
    }
}
