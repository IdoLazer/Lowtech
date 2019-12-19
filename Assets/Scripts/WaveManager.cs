using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;

    private class WavePulse
    {
        public string type;
        public float timeStamp;

        public WavePulse(string type, float timeStamp)
        {
            this.type = type;
            this.timeStamp = timeStamp;
        }
    }

    private Dictionary<string, Wave> waveByType = new Dictionary<string, Wave>();
    private float startTime;
    private bool started;
    private WavePulse nextWavePulse;
    private List<WavePulse> wavePulses;

    void OnAwake()
    {
        foreach (Wave wave in waves)
        {
            waveByType.Add(wave.type, wave);
            foreach (float timeStamp in wave.timeStamps)
            {
                wavePulses.Add(new WavePulse(wave.type, timeStamp));
            }
        }
        wavePulses.Sort((x, y) => x.timeStamp.CompareTo(y.timeStamp));
        if (wavePulses.Count > 0)
        {
            nextWavePulse = wavePulses[0];
            wavePulses.RemoveAt(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
            return;
        if (Time.time - startTime >= nextWavePulse.timeStamp)
        {
            SendPulse(nextWavePulse.type);
            if (wavePulses.Count > 0)
            {
                nextWavePulse = wavePulses[0];
                wavePulses.RemoveAt(0);
            }
        }
    }

    public void SendPulse(string type)
    {
        waveByType[type].SendPulse();
    }

    public void Start()
    {
        started = true;
        startTime = Time.time;
    }
}
