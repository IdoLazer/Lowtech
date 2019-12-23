using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Wave[] waves;

    public class PulseFinishedEvent : UnityEngine.Events.UnityEvent<string> {}
    public PulseFinishedEvent pulseFinishedEvent;
    public bool finished;

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
    private int curWaveIndex = 0;

    public void Init()
    {
        if (pulseFinishedEvent == null)
        {
            pulseFinishedEvent = new PulseFinishedEvent();
        }
        wavePulses = new List<WavePulse>();
        foreach (Wave wave in waves)
        {
            waveByType.Add(wave.type, wave);
            foreach (float timeStamp in wave.timeStamps)
            {
                wavePulses.Add(new WavePulse(wave.type, timeStamp));
            }
            wave.Init(this);
        }
        wavePulses.Sort((x, y) => x.timeStamp.CompareTo(y.timeStamp));
    }

    public void StartGame()
    {
        curWaveIndex = 0;
        started = true;
        finished = false;
        startTime = Time.time;
        if (wavePulses.Count > 0)
        {
            nextWavePulse = wavePulses[curWaveIndex];
            curWaveIndex += 1;
        }
    }

    public void StopGame()
    {
        started = false;
    }

    internal void PulseFinished(string type, bool isLastPulse)
    {
        if (isLastPulse)
        {
            finished = true;
        }
        pulseFinishedEvent.Invoke(type);
    }

    // Update is called once per frame
    void Update()
    {
        if (!started || finished || nextWavePulse == null)
            return;
        if (Time.time - startTime >= nextWavePulse.timeStamp)
        {
            SendPulse(nextWavePulse.type, wavePulses.Count == curWaveIndex);
            if (wavePulses.Count > curWaveIndex)
            {
                nextWavePulse = wavePulses[curWaveIndex];
                curWaveIndex += 1;
            }
            else
            {
                nextWavePulse = null;
            }
        }
    }

    public void SendPulse(string type, bool isLastPulse)
    {
        waveByType[type].SendPulse(isLastPulse);
    }
}
