using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave")]
public class Wave : ScriptableObject
{
    public string type;

    public float[] timeStamps;

    public GameObject prefab;

    public void SendPulse()
    {
        prefab.GetComponent<Animator>().SetTrigger(0);
    }
}
