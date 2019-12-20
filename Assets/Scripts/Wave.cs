using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave")]
public class Wave : ScriptableObject
{
    public string type;
    public float[] timeStamps;
    public GameObject prefab;
    public UnityEngine.Events.UnityEvent onPulseEndEvent;

    private WaveManager waveManager;

    public void Init(WaveManager waveManager)
    {
        this.waveManager = waveManager;
    }

    public void SendPulse(bool isLastPulse)
    {
        GameObject wavePulseObject = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        wavePulseObject.GetComponent<WavePulseController>().Go(this, isLastPulse);
    }

    public void FinishPulse(bool isLastPulse)
    {
        if (waveManager == null)
            return;
        waveManager.PulseFinished(type, isLastPulse);
    }
}
