using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{

    public Loop curLoop;
    public bool record;
    public AudioManager audioManager;
    public GameObject dancerPrefab;

    private bool IsTimerOn;
    private float lastTimeAdded;
    private List<Loop> PlayerLoops;
    private float dancerLength;
    private const int SCREEN_HEIGHT = 18;
    private const int SCREEN_WIDTH = 32;
    private List<Vector2> availableLocations;
    private GameObject curDancer;


    void Start()
    {
        CreateLocations();
        PlayerLoops = new List<Loop>();
        record = false;
        curLoop = null;
    }

    private void CreateLocations()
    {
        availableLocations = new List<Vector2>();
        for (int i = 1; i < SCREEN_HEIGHT - 1; i++)
            for (int j = 1; j < SCREEN_WIDTH - 1; j++)
            {
                if (i%3 == 0 && j%3 == 0)
                {
                    availableLocations.Add(new Vector2(j, i));
                }
            }
    }

    void Update()
    {
        // Space key - if not recording, start recording, if recording, stop recording and start playing loop.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!record)
            {
                if (curLoop == null)
                {
                    curLoop = new Loop();
                }
                CreateNewDancer();
                record = true;
                lastTimeAdded = Time.time;
            }
            else
            {
                record = false;
                // If the player created a new loop, insert it to the other loops list and reset curLoop.
                if (curLoop != null && curLoop.sounds.Count > 0)
                {
                    curLoop.timeGap.Add(Time.time - lastTimeAdded);
                    PlayerLoops.Add(curLoop);
                    StartCoroutine(PlayLoop(curLoop));
                    curLoop = null;
                }
            }
        }

        if (record)
        // Add one sound to the recorded loop.
        {
            // Adding the sound element accourding to the choice of the player.
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                AddSoundToCurLoop("Element2");
                curLoop.moves.Add(Vector2.up);
                MoveDancer(curDancer, curLoop.location, Vector2.up);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                AddSoundToCurLoop("Element3");
                curLoop.moves.Add(Vector2.down);
                MoveDancer(curDancer, curLoop.location, Vector2.down);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AddSoundToCurLoop("Element4");
                curLoop.moves.Add(Vector2.left);
                MoveDancer(curDancer, curLoop.location, Vector2.left);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                AddSoundToCurLoop("Element5");
                curLoop.moves.Add(Vector2.right);
                MoveDancer(curDancer, curLoop.location, Vector2.right);
            }

            return;
        }

        // "If" conditions to make unrecorded sounds:
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("up");
            GetSound("Element2").Play();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("down");
            GetSound("Element3").Play();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("left");
            GetSound("Element4").Play();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("right");
            GetSound("Element5").Play();
        }

    }

    private void MoveDancer(GameObject dancer, Vector2 originalPos, Vector2 direction)
    {
        StartCoroutine(MoveDancerCoroutine(dancer, originalPos, direction));
    }

    private IEnumerator MoveDancerCoroutine(GameObject dancer, Vector2 originalPos, Vector2 direction)
    {
        dancer.transform.position = originalPos + direction;
        yield return new WaitForSeconds(0.3f);
        dancer.transform.position = originalPos;
    }

    private void CreateNewDancer()
    {
        int index = UnityEngine.Random.Range(0, availableLocations.Count - 1);
        Vector2 newDancerLocation = availableLocations[index];
        availableLocations.RemoveAt(index);
        curDancer = Instantiate(dancerPrefab, newDancerLocation, Quaternion.identity) as GameObject;
        curLoop.dancer = curDancer;
        curLoop.location = newDancerLocation;
    }

    private void AddSoundToCurLoop(string name)
    {
        Sound s = GetSound(name);
        s.Play();
        curLoop.sounds.Add(s);
        curLoop.timeGap.Add(Time.time - lastTimeAdded);
        lastTimeAdded = Time.time;
    }

    private IEnumerator PlayLoop(Loop loop)
    {
        int numOfSounds = loop.sounds.Count;
        while(true)
        {
            for(int i = 0; i < numOfSounds; i++)
            {
                yield return new WaitForSeconds(loop.timeGap[i]);
                MoveDancer(loop.dancer, loop.location, loop.moves[i]);
                loop.sounds[i].Play();
            }
            yield return new WaitForSeconds(loop.timeGap[numOfSounds]);
        }
    }

    // Get Sound from AudioManager according to it's name.
    Sound GetSound(string soundName)
    {
        return Array.Find(audioManager.Sounds, Sound => Sound.name == soundName);
    }

}
