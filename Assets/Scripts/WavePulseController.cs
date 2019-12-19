using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePulseController : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onPulseEndEvent;

    public void SendPulseEndEvent()
    {
        onPulseEndEvent.Invoke();
    }
}
