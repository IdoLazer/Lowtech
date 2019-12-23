using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Loop
{
    public List<Sound> sounds;
    public List<float> timeGap;
    public List<Vector2> moves;
    public GameObject dancer;
    public Vector2 location;

    public Loop()
    {
        this.sounds = new List<Sound>();
        this.timeGap = new List<float>();
        this.moves = new List<Vector2>();
    }
}

