using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePulseController : MonoBehaviour
{

    private Wave parent;
    private bool isLastPulse;

    public void Go(Wave parent, bool isLastPulse)
    {
        this.isLastPulse = isLastPulse;
        FindObjectOfType<GameManager>().gameEndedEvent.AddListener(DestroyPulse);
        this.parent = parent;
        GetComponent<Animator>().SetTrigger("sendPulse");
    }

    public void PulseFinished()
    {
        if (parent != null)
        {
            parent.FinishPulse(isLastPulse);
        }
        Destroy(gameObject);
    }

    public void DestroyPulse()
    {
        Destroy(gameObject);
    }
}
