using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{

    public WaveManager waveManager;
    public GameObject playerPrefab;
    public GameObject startCanvas;
    public GameObject gameBoard;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public MusicPlayer musicPlayer;
    public UnityEngine.Events.UnityEvent gameEndedEvent;

    private int state = 0;
    private PlayerController playerController;

    void Awake()
    {
        gameEndedEvent = new UnityEngine.Events.UnityEvent();
        startCanvas.SetActive(true);
        gameBoard.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        waveManager.Init();
        waveManager.pulseFinishedEvent.AddListener(OnPulseFinished);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        switch (state)
        {
            case 0: StartScreen();
                break;
            case 1: GameOn();
                break;
            case 2: EndScreen();
                break;
            default:
                break;
        }
    }

    private void EndScreen()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            winCanvas.SetActive(false);
            loseCanvas.SetActive(false);
            StartGame();
        }
    }

    private void GameOn()
    {
        if (playerController.health <= 0)
        {
            StopGame();
        }
    }

    private void StopGame()
    {
        waveManager.StopGame();
        gameBoard.SetActive(false);
        Destroy(playerController.gameObject);
        state = 2;
        gameEndedEvent.Invoke();
        if (playerController.health <= 0)
        {
            loseCanvas.SetActive(true);
        }
        else
        {
            winCanvas.SetActive(true);
        }
    }

    private void StartScreen()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            startCanvas.SetActive(false);
            StartGame();
        }
    }

    private void StartGame()
    {
        gameBoard.SetActive(true);
        waveManager.StartGame();
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        playerController = player.GetComponent<PlayerController>();
        playerController.animated = true;
        musicPlayer.Play();
        state = 1;
    }

    void OnPulseFinished(String type)
    {
        if (playerController.PressedCorrectKeyForPulseType(type))
        {
            playerController.Success();
        }
        else
        {
            playerController.Damage();
        }
        if (waveManager.finished)
        {
            StopGame();
        }
    }
}
