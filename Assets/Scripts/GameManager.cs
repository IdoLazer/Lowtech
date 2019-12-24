using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{

    public WaveManager waveManager;
    public GameObject playerPrefab;
    public GameObject startCanvas;
    public GameObject gameBoard;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public MusicPlayer musicPlayer;
    public Canvas comboCanvas;
    public UnityEngine.UI.Image image;
    public UnityEngine.Events.UnityEvent gameEndedEvent;


    private int state = 0;
    private PlayerController playerController;
    private float DESIGN_WIDTH = 1920f;
    private float DESIGN_HEIGHT = 1080f;
    private float CAMERA_HEIGHT_FACTOR = 216f;
    private int streak = 0;
    private string curWaveType;
    private bool waveWaiting = false;
    private bool firstRound = true;

    void Awake()
    {
        SetResolution();
        gameEndedEvent = new UnityEngine.Events.UnityEvent();
        startCanvas.SetActive(true);
        gameBoard.SetActive(true);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        playerController = player.GetComponent<PlayerController>();
        playerController.animated = true;
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        waveManager.Init();
        waveManager.pulseFinishedEvent.AddListener(OnPulseFinished);
    }

    private void SetResolution()
    {
        // Set resolution
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = DESIGN_WIDTH / DESIGN_HEIGHT;
        float scaleHeight = windowAspect / targetAspect;

        Debug.Log("windowAspect: " + windowAspect + ", targetAspect: " + targetAspect);

        if (windowAspect < targetAspect)
        {
            Camera.main.orthographicSize = (DESIGN_HEIGHT / CAMERA_HEIGHT_FACTOR) / scaleHeight;
        }
        else
        {
            Camera.main.orthographicSize = DESIGN_HEIGHT / CAMERA_HEIGHT_FACTOR;
        }
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
            firstRound = false;
            winCanvas.SetActive(false);
            loseCanvas.SetActive(false);
            StartGame();
        }
    }

    private void GameOn()
    {
        CheckPlayerSuccess();
        if (playerController.health <= 0)
        {
            StopGame();
        }
    }

    private void CheckPlayerSuccess()
    {
        if (!waveWaiting)
            return;
        if (playerController.PressedCorrectKeyForPulseType(curWaveType))
        {
            waveWaiting = false;
            playerController.Success();
            streak += 1;
            Canvas combo = Instantiate(comboCanvas, new Vector3(0, 0, 0), Quaternion.identity);
            TextMeshProUGUI comboText = combo.GetComponentInChildren<TextMeshProUGUI>();
            comboText.text = '+' + streak.ToString();
            if (streak % 10 == 0)
            {
                comboText.GetComponent<Animator>().SetTrigger("FinishStreak");
            }
            else
            {
                comboText.GetComponent<Animator>().SetTrigger("Appear");
            }
            StartCoroutine(DestroyWithDelay(combo.gameObject, 1));
        }
        if (waveManager.finished)
        {
            StopGame();
        }
    }

    private void StopGame()
    {
        StartCoroutine(DelayBeforeEndGame());
    }

    private IEnumerator DelayBeforeEndGame()
    {
        bool lose = playerController.health <= 0;
        yield return new WaitForSeconds(0.5f);
        waveManager.StopGame();
        gameBoard.SetActive(false);
        Destroy(playerController.gameObject);
        state = 2;
        gameEndedEvent.Invoke();
        if (lose)
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
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            image.GetComponent<Animator>().SetTrigger("Red");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            image.GetComponent<Animator>().SetTrigger("Blue");
        }
        if (Input.GetKey(KeyCode.Space))
        {
            image.GetComponent<Animator>().SetTrigger("Green");
        }
        if (Input.GetKey(KeyCode.Return))
        {
            Destroy(playerController.gameObject);
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
        if (!firstRound)
            playerController.GetComponent<Animator>().SetTrigger("Entry");
        musicPlayer.Play();
        state = 1;
    }

    void OnPulseFinished(String type)
    {
        StartCoroutine(WaitForPlayerInput(type));
    }

    private IEnumerator WaitForPlayerInput(string type)
    {
        curWaveType = type;
        waveWaiting = true;
        yield return new WaitForSeconds(0.3f);
        if(waveWaiting)
        {
            playerController.Damage();
            streak = 0;
        }
        waveWaiting = false;
    }

    static public IEnumerator DestroyWithDelay(GameObject o, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(o);
    }
}
