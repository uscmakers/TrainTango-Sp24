using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        None,
        MainMenu,
        Calibration,
        Playing,
        GameOver
    }

    private GameState prevGameState = GameState.None;
    public static GameState gameState = GameState.Playing;
    public static bool isPaused = false;

    public GameObject calibrationPrefab;
    private GameObject calibration;
    public GameObject calibrationUI;

    public List<MovingObject> movingObjects = new List<MovingObject>();
    public float environmentSpeed = 0;
    public float maxEnvironmentSpeed = 10;

    public List<GameObject> hazardPrefabs;
    private float hazardTimer = 0;
    private float totalTime = 0;
    public float minHazardTime = 2f;
    public float maxHazardTime = 3.5f;

    public List<Camera> vrCameras;
    public List<Camera> nonVrCameras;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void Update()
    {
        if (prevGameState != gameState)
        {
            OnStateEnd(prevGameState);
            OnStateStart(gameState);
            prevGameState = gameState;
        }

        if (!isPaused)
        {
            switch (gameState)
            {
                case GameState.GameOver:
                case GameState.Playing:
                    GameplayUpdate();
                    break;
                default:
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            Time.timeScale = !isPaused ? 1 : 0;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (Camera cam in vrCameras)
            {
                cam.enabled = Input.GetKeyDown(KeyCode.Alpha1);
            }
            foreach (Camera cam in nonVrCameras)
            {
                cam.enabled = Input.GetKeyDown(KeyCode.Alpha2);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            vrCameras[0].transform.position = vrCameras[0].transform.position - new Vector3(0.001f, 0, 0);
            vrCameras[1].transform.position = vrCameras[0].transform.position + new Vector3(0.001f, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            vrCameras[0].transform.position = vrCameras[0].transform.position + new Vector3(0.001f, 0, 0);
            vrCameras[1].transform.position = vrCameras[0].transform.position - new Vector3(0.001f, 0, 0);
        }
    }

    public void Restart()
    {
        gameState = GameState.Playing;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void GameplayUpdate()
    {
        totalTime += Time.deltaTime;

        if (gameState == GameState.Playing)
            environmentSpeed = Mathf.Min(maxEnvironmentSpeed, environmentSpeed + Time.deltaTime * 5f);
        else if (gameState == GameState.GameOver)
            environmentSpeed = Mathf.Max(0, environmentSpeed - Time.deltaTime * 5f);

        hazardTimer -= Time.deltaTime;
        if (hazardTimer <= 0)
        {
            hazardTimer = Random.Range(minHazardTime, maxHazardTime);
            GameObject hazard = Instantiate(hazardPrefabs[Random.Range(0, hazardPrefabs.Count)]);
            hazard.GetComponent<MovingObject>().SetSide((PlayerMovement.PlayerSide)Random.Range(-1, 2));
        }

        foreach (MovingObject obj in movingObjects)
        {
            obj.transform.position -= Vector3.forward * environmentSpeed * Time.deltaTime;
            obj.GameplayUpdate();
        }

        movingObjects = movingObjects.FindAll((obj) => !obj.shouldDestroy);

        if (gameState == GameState.Playing)
            PlayerMovement.instance.GameplayUpdate();
    }


    private void OnStateStart(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Calibration:
                calibrationUI.SetActive(true);

                calibration = Instantiate(calibrationPrefab);
                Calibration calibrationScript = calibration.GetComponent<Calibration>();
                calibrationScript.calibrationUI = calibrationUI.GetComponent<CalibrationUI>();
                calibrationScript.StartCalibrations(new List<string> { "Stay", "Jump", "Left", "Right" });
                break;
            case GameState.Playing:
                isPaused = false;
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void OnStateEnd(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Calibration:
                calibrationUI.SetActive(false);

                if (calibration)
                    Destroy(calibration);
                break;
            case GameState.Playing:
                break;
            case GameState.GameOver:
                break;
        }
    }
}
